using System.Collections.Generic;
using UnityEngine;

/// <summary>Result returned by EnemySkillSimulator for a single skill evaluation.</summary>
public class SimulationResult
{
    /// <summary>Best tile to target (for tile-targeted skills). Null for unit-targeted or self-targeted.</summary>
    public BoardTile BestTargetTile;
    /// <summary>Best primary unit target (for unit-targeted skills).</summary>
    public Unit BestTargetUnit;
    /// <summary>All units that would be hit by this skill with the selected targeting.</summary>
    public List<Unit> AllTargetsHit = new List<Unit>();
    /// <summary>All tiles that would be highlighted by this skill.</summary>
    public List<BoardTile> AllTilesHit = new List<BoardTile>();
    /// <summary>Overall score of this targeting option (higher = better for the enemy).</summary>
    public float Score;
    /// <summary>True if at least one valid target was found.</summary>
    public bool HasValidTarget => AllTargetsHit.Count > 0 || BestTargetTile != null;
}

/// <summary>
/// Evaluates skill targeting for enemies using pure spatial queries — no Preview() calls,
/// no visual side effects, no mouse input. Works entirely from the board state.
/// </summary>
public static class EnemySkillSimulator
{
    /// <summary>
    /// Simulates the given skill from the given caster tile and returns the best targeting option.
    /// If multiple targets are allowed (MaxTargetCount > 1) all valid targets are sorted and collected.
    /// </summary>
    public static SimulationResult SimulateSkill(
        EnemyBaseAI enemy,
        SO_EnemySkill enemySkill,
        BoardTile casterTile,
        SO_EffectWeightConfig weights = null)
    {
        if (enemySkill?.Skill == null)
            return new SimulationResult();

        // Walk the first SkillPartGroup to determine targeting shape
        var firstGroup = enemySkill.Skill.SkillPartGroups.Count > 0
            ? enemySkill.Skill.SkillPartGroups[0]
            : null;

        if (firstGroup == null || firstGroup.skillParts.Count == 0)
            return new SimulationResult();

        var firstPart = firstGroup.skillParts[0];

        if (firstPart is SO_TargetSelfSkill)
            return SimulateSelfWithFollowups(enemy, enemySkill, firstGroup, casterTile, weights);

        if (firstPart is SO_TargetUnitSkill)
            return SimulateTargetUnit(enemy, enemySkill, casterTile, weights);

        if (firstPart is SO_TargetBoardtileSkill)
            return SimulateTargetTile(enemy, enemySkill, firstPart, firstGroup, casterTile, weights);

        if (firstPart is SO_LineSkill lineSkill)
            return SimulateLine(enemy, enemySkill, lineSkill, casterTile, weights);

        if (firstPart is SO_ConeSkill coneSkill)
            return SimulateDirectional(enemy, enemySkill, coneSkill.MaxRange, casterTile, weights, wide: coneSkill.isWide);

        if (firstPart is SO_HalfCircleSkill halfCircleSkill)
            return SimulateHalfCircle(enemy, enemySkill, halfCircleSkill, casterTile, weights);

        if (firstPart is SO_AOE_Skill)
            return SimulateAoEFromCaster(enemy, enemySkill, firstPart, casterTile, weights);

        return new SimulationResult();
    }

    // -----------------------------------------------------------------------
    // Self-targeting — also evaluates any follow-up AoE / directional parts
    // -----------------------------------------------------------------------

    /// <summary>
    /// Handles SO_TargetSelfSkill as first part. Inspects the rest of the group for
    /// AoE / Cone / HalfCircle / Line follow-ups and delegates to the appropriate
    /// shape simulator so the full effect area is returned, not just the caster tile.
    /// </summary>
    private static SimulationResult SimulateSelfWithFollowups(
        EnemyBaseAI enemy, SO_EnemySkill skill, SkillPartGroup group,
        BoardTile casterTile, SO_EffectWeightConfig weights)
    {
        for (int p = 1; p < group.skillParts.Count; p++)
        {
            var part = group.skillParts[p];
            if (part == null) continue;

            if (part is SO_AOE_Skill)
                return SimulateAoEFromCaster(enemy, skill, part, casterTile, weights);

            if (part is SO_HalfCircleSkill halfPart)
                return SimulateHalfCircle(enemy, skill, halfPart, casterTile, weights);

            if (part is SO_ConeSkill conePart)
                return SimulateDirectional(enemy, skill, conePart.MaxRange, casterTile, weights, wide: conePart.isWide);

            if (part is SO_LineSkill linePart)
                return SimulateLine(enemy, skill, linePart, casterTile, weights);
        }

        // No AoE / directional follow-up — pure self-buff/heal
        return SimulateSelf(enemy, skill, casterTile, weights);
    }

    private static SimulationResult SimulateSelf(
        EnemyBaseAI enemy, SO_EnemySkill skill, BoardTile casterTile, SO_EffectWeightConfig weights)
    {
        var result = new SimulationResult
        {
            BestTargetUnit = enemy,
            BestTargetTile = casterTile
        };
        result.AllTargetsHit.Add(enemy);
        result.AllTilesHit.Add(casterTile);
        result.Score = SkillEffectEvaluator.EvaluateEffects(skill, result.AllTargetsHit, weights);
        return result;
    }

    // -----------------------------------------------------------------------
    // TargetUnit — single or multi-target unit selection
    // -----------------------------------------------------------------------

    private static SimulationResult SimulateTargetUnit(
        EnemyBaseAI enemy, SO_EnemySkill skill, BoardTile casterTile, SO_EffectWeightConfig weights)
    {
        float range = skill.Skill.GetAttackRange();

        // Fallback: if no part has IncludeInAutoMove set, use the first part's MaxRange directly.
        if (range <= 0f && skill.Skill.SkillPartGroups?.Count > 0)
        {
            var parts = skill.Skill.SkillPartGroups[0].skillParts;
            if (parts != null && parts.Count > 0 && parts[0] != null)
                range = parts[0].MaxRange;
        }

        var candidates = GetValidCharactersInRange(enemy, skill, casterTile, range);

        if (candidates.Count == 0)
            return new SimulationResult();

        var result = new SimulationResult();
        int maxTargets = Mathf.Max(1, skill.MaxTargetCount);

        for (int i = 0; i < Mathf.Min(maxTargets, candidates.Count); i++)
        {
            result.AllTargetsHit.Add(candidates[i]);
            result.AllTilesHit.Add(candidates[i].Tile);
        }

        result.BestTargetUnit = result.AllTargetsHit[0];
        result.BestTargetTile = result.BestTargetUnit.Tile;
        result.Score = SkillEffectEvaluator.EvaluateEffects(skill, result.AllTargetsHit, weights);
        return result;
    }

    // -----------------------------------------------------------------------
    // TargetBoardTile — pick best tile, then simulate following AoE parts
    // -----------------------------------------------------------------------

    private static SimulationResult SimulateTargetTile(
        EnemyBaseAI enemy, SO_EnemySkill skill,
        SO_Skillpart targetTilePart, SkillPartGroup firstGroup,
        BoardTile casterTile, SO_EffectWeightConfig weights)
    {
        float tileRange = targetTilePart.MaxRange;
        var candidateTiles = BoardManager.Instance.GetTilesWithinDirectRange(casterTile, tileRange, false);

        SimulationResult best = null;

        foreach (var candidate in candidateTiles)
        {
            // Evaluate any following AoE parts from this candidate tile
            var aoeTargets = new List<Unit>();
            var aoeTiles = new List<BoardTile>();
            aoeTiles.Add(candidate);

            float aoeRange = 0f;
            for (int p = 1; p < firstGroup.skillParts.Count; p++)
            {
                var part = firstGroup.skillParts[p];
                if (part is SO_AOE_Skill || part is SO_HalfCircleSkill)
                {
                    aoeRange = part.MaxRange;
                    var expanded = BoardManager.Instance.GetTilesWithinDirectRange(candidate, aoeRange, false);
                    aoeTiles.AddRange(expanded);
                }
            }

            foreach (var tile in aoeTiles)
                if (tile.currentUnit != null && IsValidTarget(enemy, skill, tile.currentUnit))
                    aoeTargets.Add(tile.currentUnit);

            // Exclude tiles that would only hit allies (faction safety)
            if (aoeTargets.Count == 0) continue;
            if (WouldHitOnlyAllies(aoeTargets)) continue;

            var candidate_result = new SimulationResult
            {
                BestTargetTile = candidate,
                AllTilesHit = aoeTiles,
                AllTargetsHit = aoeTargets,
                Score = SkillEffectEvaluator.EvaluateEffects(skill, aoeTargets, weights)
            };
            candidate_result.BestTargetUnit = aoeTargets.Count > 0 ? aoeTargets[0] : null;

            if (best == null || candidate_result.Score > best.Score)
                best = candidate_result;
        }

        return best ?? new SimulationResult();
    }

    // -----------------------------------------------------------------------
    // Line — evaluate all 8 directions, pick best
    // -----------------------------------------------------------------------

    private static SimulationResult SimulateLine(
        EnemyBaseAI enemy, SO_EnemySkill skill,
        SO_LineSkill linePart, BoardTile casterTile, SO_EffectWeightConfig weights)
    {
        var board = BoardManager.Instance;
        SimulationResult best = null;

        int numDirections = casterTile.connectedTiles.Length;
        for (int dirIndex = 0; dirIndex < numDirections; dirIndex++)
        {
            var (tiles, targets) = CastLine(casterTile, dirIndex, linePart.MaxRange, linePart.MinRange,
                linePart.PierceAmount, board);

            var validTargets = new List<Unit>();
            foreach (var t in targets)
                if (IsValidTarget(enemy, skill, t)) validTargets.Add(t);

            if (validTargets.Count == 0) continue;

            float score = SkillEffectEvaluator.EvaluateEffects(skill, validTargets, weights);
            if (best == null || score > best.Score)
            {
                best = new SimulationResult
                {
                    BestTargetTile = tiles.Count > 0 ? tiles[tiles.Count - 1] : null,
                    BestTargetUnit = validTargets[0],
                    AllTargetsHit = validTargets,
                    AllTilesHit = tiles,
                    Score = score
                };
            }
        }

        return best ?? new SimulationResult();
    }

    // -----------------------------------------------------------------------
    // Cone — evaluate all directions, pick best
    // -----------------------------------------------------------------------

    private static SimulationResult SimulateDirectional(
        EnemyBaseAI enemy, SO_EnemySkill skill,
        float range, BoardTile casterTile, SO_EffectWeightConfig weights, bool wide = false)
    {
        var board = BoardManager.Instance;
        SimulationResult best = null;

        int numDirections = casterTile.connectedTiles.Length;
        for (int dirIndex = 0; dirIndex < numDirections; dirIndex++)
        {
            var tiles = GetConeTiles(casterTile, dirIndex, range, wide, board);
            var validTargets = new List<Unit>();
            var allTilesHit = new List<BoardTile>();
            foreach (var tile in tiles)
            {
                allTilesHit.Add(tile);
                if (tile.currentUnit != null && IsValidTarget(enemy, skill, tile.currentUnit))
                    validTargets.Add(tile.currentUnit);
            }

            if (validTargets.Count == 0) continue;

            float score = SkillEffectEvaluator.EvaluateEffects(skill, validTargets, weights);
            if (best == null || score > best.Score)
            {
                var firstTarget = validTargets[0];
                best = new SimulationResult
                {
                    BestTargetTile = firstTarget.Tile,
                    BestTargetUnit = firstTarget,
                    AllTargetsHit = validTargets,
                    AllTilesHit = allTilesHit,
                    Score = score
                };
            }
        }

        return best ?? new SimulationResult();
    }

    // -----------------------------------------------------------------------
    // HalfCircle — 180° arc in each direction
    // -----------------------------------------------------------------------

    private static SimulationResult SimulateHalfCircle(
        EnemyBaseAI enemy, SO_EnemySkill skill,
        SO_HalfCircleSkill part, BoardTile casterTile, SO_EffectWeightConfig weights)
    {
        // HalfCircle covers 5 out of 8 directions centered on the facing direction.
        return SimulateDirectional(enemy, skill, part.MaxRange, casterTile, weights, wide: true);
    }

    // -----------------------------------------------------------------------
    // AoE from caster tile
    // -----------------------------------------------------------------------

    private static SimulationResult SimulateAoEFromCaster(
        EnemyBaseAI enemy, SO_EnemySkill skill,
        SO_Skillpart aoePart, BoardTile casterTile, SO_EffectWeightConfig weights)
    {
        // Include the caster's own tile — caster-origin AoE hits the caster's cell too.
        var tiles = BoardManager.Instance.GetTilesWithinDirectRange(casterTile, aoePart.MaxRange, false);
        if (!tiles.Contains(casterTile))
            tiles.Add(casterTile);

        var validTargets = new List<Unit>();
        foreach (var tile in tiles)
            if (tile.currentUnit != null && IsValidTarget(enemy, skill, tile.currentUnit))
                validTargets.Add(tile.currentUnit);

        // Always return the area tiles for preview purposes — even if no valid targets are
        // currently in range (e.g. the enemy hasn't moved to the engage tile yet).
        return new SimulationResult
        {
            BestTargetTile = casterTile,
            BestTargetUnit = validTargets.Count > 0 ? validTargets[0] : null,
            AllTargetsHit = validTargets,
            AllTilesHit = tiles,
            Score = SkillEffectEvaluator.EvaluateEffects(skill, validTargets, weights)
        };
    }

    // -----------------------------------------------------------------------
    // Helpers
    // -----------------------------------------------------------------------

    /// <summary>
    /// Returns valid characters in range sorted by the skill's target priority.
    /// Applies AvoidBuffed / AvoidDebuffed filters if configured.
    /// </summary>
    public static List<Unit> GetValidCharactersInRange(
        EnemyBaseAI enemy, SO_EnemySkill skill, BoardTile casterTile, float range)
    {
        var board = BoardManager.Instance;
        var priority = skill.TargetPriority;

        var candidates = new List<Unit>();
        foreach (var character in UnitData.Characters)
        {
            if (character == null || character.Tile == null || character.Hitpoints <= 0) continue;
            if (board.GetRangeBetweenTiles(casterTile, character.Tile) > range) continue;
            if (!IsValidTarget(enemy, skill, character)) continue;
            candidates.Add(character);
        }

        if (candidates.Count == 0) return candidates;

        TargetPriorityKindEnum priorityKind = priority != null ? priority.Priority : TargetPriorityKindEnum.Closest;

        switch (priorityKind)
        {
            case TargetPriorityKindEnum.Closest:
                candidates.Sort((a, b) =>
                    board.GetRangeBetweenTiles(casterTile, a.Tile)
                        .CompareTo(board.GetRangeBetweenTiles(casterTile, b.Tile)));
                break;

            case TargetPriorityKindEnum.LowestHealth:
                candidates.Sort((a, b) => a.Hitpoints.CompareTo(b.Hitpoints));
                break;

            case TargetPriorityKindEnum.HighestHealth:
                candidates.Sort((a, b) => b.Hitpoints.CompareTo(a.Hitpoints));
                break;

            case TargetPriorityKindEnum.Random:
                for (int i = candidates.Count - 1; i > 0; i--)
                {
                    int j = Random.Range(0, i + 1);
                    (candidates[i], candidates[j]) = (candidates[j], candidates[i]);
                }
                break;

            case TargetPriorityKindEnum.Sequenced:
                // Keep list order (matches UnitData.Characters initiative order)
                break;
        }

        return candidates;
    }

    private static bool IsValidTarget(EnemyBaseAI enemy, SO_EnemySkill skill, Unit target)
    {
        // Enemies never intentionally target other enemies for offensive effects,
        // or characters for defensive effects. The basic check: characters are valid
        // offensive targets; other enemies are not.
        if (target == null) return false;

        // Characters (player units) are always valid offensive targets
        if (target.Friendly) return true;

        // Another enemy — only valid if AllowTargetingSelf is set and it's the caster
        var priority = skill.TargetPriority;
        if (!target.Friendly && target == enemy && priority != null && priority.AllowTargetingSelf)
            return true;

        return false;
    }

    private static bool WouldHitOnlyAllies(List<Unit> targets)
    {
        foreach (var t in targets)
            if (t.Friendly) return false;
        return targets.Count > 0;
    }

    // -----------------------------------------------------------------------
    // Spatial calculation helpers
    // -----------------------------------------------------------------------

    private static (List<BoardTile> tiles, List<Unit> targets) CastLine(
        BoardTile origin, int dirIndex, float maxRange, float minRange, int pierceAmount,
        BoardManager board)
    {
        var tiles = new List<BoardTile>();
        var targets = new List<Unit>();

        if (origin.connectedTiles == null || dirIndex >= origin.connectedTiles.Length) return (tiles, targets);
        var neighbor = origin.connectedTiles[dirIndex];
        if (neighbor == null) return (tiles, targets);

        Vector2Int direction = neighbor.Coordinates - origin.Coordinates;
        Vector2Int current = origin.Coordinates;
        float distanceTraveled = 0f;
        int pierce = pierceAmount;

        while (distanceTraveled < maxRange)
        {
            Vector2Int next = current + direction;
            float cost = board.GetRangeReduction(current, next);
            distanceTraveled += cost;
            if (distanceTraveled > maxRange) break;

            var tile = board.GetBoardTile(next);
            if (tile == null) break;
            if (tile.IsBlocked) break;

            current = next;

            if (distanceTraveled >= minRange)
                tiles.Add(tile);

            if (tile.currentUnit != null && distanceTraveled >= minRange)
            {
                targets.Add(tile.currentUnit);
                if (pierce == 0) break;
                if (pierce > 0) pierce--;
            }
        }

        return (tiles, targets);
    }

    private static List<BoardTile> GetConeTiles(BoardTile origin, int facingDir, float range, bool wide, BoardManager board)
    {
        var result = new List<BoardTile>();
        int numDirs = origin.connectedTiles.Length; // typically 8

        // Narrow cone: facing dir ± 1; Wide: facing ± 2
        int spread = wide ? 2 : 1;

        // Cast a line in facing and adjacent directions
        for (int offset = -spread; offset <= spread; offset++)
        {
            int dir = ((facingDir + offset) % numDirs + numDirs) % numDirs;
            var (tiles, _) = CastLine(origin, dir, range, 0f, -1, board);
            foreach (var t in tiles)
                if (!result.Contains(t)) result.Add(t);
        }

        return result;
    }

    // -----------------------------------------------------------------------
    // Threat-range footprint — used by BoardManager for threat overlay
    // -----------------------------------------------------------------------

    /// <summary>
    /// Returns every tile this skill could possibly hit when cast from <paramref name="fromTile"/>,
    /// considering all 8 casting directions. Used to build the full threat-range overlay.
    /// </summary>
    public static List<BoardTile> GetSkillFootprintFromTile(SO_EnemySkill skill, BoardTile fromTile)
    {
        if (skill?.Skill?.SkillPartGroups == null || skill.Skill.SkillPartGroups.Count == 0)
            return new List<BoardTile>();

        var firstGroup = skill.Skill.SkillPartGroups[0];
        if (firstGroup == null || firstGroup.skillParts.Count == 0)
            return new List<BoardTile>();

        var board   = BoardManager.Instance;
        var result  = new HashSet<BoardTile>();
        var firstPart = firstGroup.skillParts[0];

        // --- local helpers ---

        void AddCircle(BoardTile center, float radius)
        {
            if (radius <= 0f) { result.Add(center); return; }
            foreach (var t in board.GetTilesWithinDirectRange(center, radius, false))
                if (t != null) result.Add(t);
            result.Add(center);
        }

        void AddLines(BoardTile origin, float maxRange, float minRange, int pierce)
        {
            int numDirs = origin.connectedTiles?.Length ?? 8;
            for (int dir = 0; dir < numDirs; dir++)
            {
                var (tiles, _) = CastLine(origin, dir, maxRange, minRange, pierce, board);
                foreach (var t in tiles) result.Add(t);
            }
        }

        void AddCones(BoardTile origin, float maxRange, bool wide)
        {
            int numDirs = origin.connectedTiles?.Length ?? 8;
            for (int dir = 0; dir < numDirs; dir++)
                foreach (var t in GetConeTiles(origin, dir, maxRange, wide, board))
                    result.Add(t);
        }

        // --- dispatch ---

        if (firstPart is SO_TargetSelfSkill)
        {
            result.Add(fromTile);
            // Extend to follow-up AoE / directional parts
            for (int p = 1; p < firstGroup.skillParts.Count; p++)
            {
                var fp = firstGroup.skillParts[p];
                if (fp == null) continue;
                if (fp is SO_AOE_Skill)             { AddCircle(fromTile, fp.MaxRange); break; }
                if (fp is SO_HalfCircleSkill)        { AddCones(fromTile, fp.MaxRange, true); break; }
                if (fp is SO_ConeSkill cp)           { AddCones(fromTile, fp.MaxRange, cp.isWide); break; }
                if (fp is SO_LineSkill lp)           { AddLines(fromTile, fp.MaxRange, fp.MinRange, lp.PierceAmount); break; }
            }
        }
        else if (firstPart is SO_AOE_Skill)
        {
            AddCircle(fromTile, firstPart.MaxRange);
        }
        else if (firstPart is SO_TargetUnitSkill)
        {
            AddCircle(fromTile, firstPart.MaxRange);
        }
        else if (firstPart is SO_TargetBoardtileSkill)
        {
            // Target tile within range, then follow-ups extend from that tile.
            // Worst-case footprint = tile range + max follow-up range (all directions).
            float followupRange = 0f;
            for (int p = 1; p < firstGroup.skillParts.Count; p++)
                if (firstGroup.skillParts[p] != null)
                    followupRange = Mathf.Max(followupRange, firstGroup.skillParts[p].MaxRange);
            AddCircle(fromTile, firstPart.MaxRange + followupRange);
        }
        else if (firstPart is SO_LineSkill linePart)
        {
            AddLines(fromTile, linePart.MaxRange, linePart.MinRange, linePart.PierceAmount);
        }
        else if (firstPart is SO_ConeSkill conePart)
        {
            AddCones(fromTile, conePart.MaxRange, conePart.isWide);
        }
        else if (firstPart is SO_HalfCircleSkill halfPart)
        {
            AddCones(fromTile, halfPart.MaxRange, true);
        }

        return new List<BoardTile>(result);
    }
}
