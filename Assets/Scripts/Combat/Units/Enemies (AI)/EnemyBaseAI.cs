using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBaseAI : Enemy
{
    [HideInInspector] public BoardTile OptimalTile;
    [HideInInspector] public Unit Target;
    [HideInInspector] public SO_EnemySkill CurrentSkill;

    EnemySkillSequencer sequencer = new EnemySkillSequencer();

    public override void Init()
    {
        base.Init();
        // Pick the initial skill so intent can be displayed from the start
        CurrentSkill = sequencer.Pick(enemySO?.Skills, enemySO != null && enemySO.AlwaysStartWithFirstSkill);
        ApplySkillCombatStyle(CurrentSkill);
    }

    void ApplySkillCombatStyle(SO_EnemySkill skill)
    {
        // Enemy skills no longer carry a CombatStyle field; stance is set via SO_MainSkill.SkillCombatStyle
    }

    public override List<SO_SKillVFX> GetSkillVFXList()
    {
        var result = new List<SO_SKillVFX>();
        if (enemySO != null)
            foreach (var s in enemySO.Skills)
                if (s?.Skill != null)
                    foreach (var part in s.Skill)
                        if (part?.SkillVFX != null)
                            result.AddRange(part.SkillVFX);
        return result;
    }

    public override IEnumerator StartTurn()
    {
        // CurrentSkill is already picked (either in Init or at the end of the previous turn)
        if (CurrentSkill == null)
        {
            EndTurn();
            yield break;
        }

        yield return StartCoroutine(base.StartTurn());

        bool isSelfTargeting = CurrentSkill.TargetPreference == TargetEnum.Self;

        // Phase 1: Move to the closest tile within skill range (skip for self-targeting skills).
        // Ties are broken by proximity to OptimalRange to minimise Phase 3 repositioning.
        if (!isSelfTargeting)
        {
            BoardTile engageTile = FindEngageTile();
            if (engageTile != null && engageTile != Tile)
            {
                boardManager.SetMovementLine(engageTile, false);
                yield return StartCoroutine(boardManager.MoveToTile());
            }
        }

        yield return new WaitForSeconds(0.3f);

        // Phase 2: Use skill if now in range (or targeting self).
        if (isSelfTargeting || IsInSkillRange())
            yield return StartCoroutine(ExecuteSkill());

        // Phase 3: Use remaining movement to reposition at OptimalRange.
        if (MoveSpeedLeft > 0 && UnitData.Characters.Count > 0)
        {
            PossibleMovementTiles.Clear();
            boardManager.Clear();
            boardManager.SetAOE(MoveSpeedLeft, Tile, null);
            if (!PossibleMovementTiles.Contains(Tile))
                PossibleMovementTiles.Add(Tile);

            BoardTile repositionTile = FindOptimalRepositionTile();
            if (repositionTile != null && repositionTile != Tile)
            {
                boardManager.SetMovementLine(repositionTile, false);
                yield return StartCoroutine(boardManager.MoveToTile());
            }
        }

        EndTurn();
    }

    // Executes the current skill without ending the turn. Called by StartTurn (Phase 2) and Attack().
    protected virtual IEnumerator ExecuteSkill()
    {
        var skillParts = CurrentSkill.Skill;
        var firstPart  = CurrentSkill.FirstPart;

        if (CurrentSkill.TargetPreference == TargetEnum.Self)
        {
            StartCoroutine(uiManager.ShowActivityText(CurrentSkill.SkillName));
            yield return new WaitForSeconds(0.3f);

            for (int i = 0; i < skillParts.Count; i++)
            {
                var part = skillParts[i];
                part.PartData = new SkillPartData();
                part.PartData.TargetsHit.Add(this);
                StartCoroutine(skillsManager.CastSkillsPart(part, this));
                if (i < skillParts.Count - 1)
                    yield return new WaitForSeconds(0.2f);
            }

            yield return new WaitForSeconds(2);

            CurrentSkill = sequencer.Pick(enemySO?.Skills, false);
            ApplySkillCombatStyle(CurrentSkill);
            (ThisHealthbar as FloatingHealthbar)?.UpdateIntent(CurrentSkill);
            yield break;
        }

        Unit singleTarget = null;
        float closestTargetRange = 0;

        // Taunt: force targeting the taunting character if they are in range
        var tauntOverride = GetTauntTarget();
        if (tauntOverride != null)
        {
            var tauntRange = boardManager.GetRangeBetweenTiles(Tile, tauntOverride.Tile);
            if (!boardManager.TileIsBehindClosedTile(Tile, tauntOverride.Tile) &&
                tauntRange >= firstPart.MinRange && tauntRange <= firstPart.MaxRange)
            {
                singleTarget = tauntOverride;
            }
        }

        if (singleTarget == null)
        {
            foreach (var character in UnitData.Characters)
            {
                var blocked = boardManager.TileIsBehindClosedTile(Tile, character.Tile);
                if (blocked)
                    continue;

                var range = boardManager.GetRangeBetweenTiles(Tile, character.Tile);
                if (range < firstPart.MinRange || range > firstPart.MaxRange)
                    continue;

                if (singleTarget == null)
                {
                    singleTarget = character;
                    closestTargetRange = range;
                }
                else if (range < closestTargetRange)
                {
                    singleTarget = character;
                    closestTargetRange = range;
                }
            }
        }

        // Determine whether any skill parts actually have effects we care about for multi-target optimization
        bool hasRelevantParts = skillParts.Any(p => (p.DamageEffects != null && p.DamageEffects.Count > 0)
                                                   || (p.StatusEffects != null && p.StatusEffects.Count > 0)
                                                   || (p.displacementEffect != null && p.displacementEffect.UseDisplacement));

        // If no relevant multi-target parts, fall back to single-target behaviour (fast path)
        if (!hasRelevantParts)
        {
            if (singleTarget == null)
                yield break;

            StartCoroutine(uiManager.ShowActivityText(CurrentSkill.SkillName));
            yield return new WaitForSeconds(0.3f);

            Rotate(singleTarget.transform.position);
            for (int i = 0; i < skillParts.Count; i++)
            {
                var part = skillParts[i];
                part.PartData = new SkillPartData();
                part.PartData.TargetsHit.Add(singleTarget);
                StartCoroutine(skillsManager.CastSkillsPart(part, this));
                if (i < skillParts.Count - 1)
                    yield return new WaitForSeconds(0.2f);
            }

            yield return new WaitForSeconds(2);
        }
        else
        {
            // Multi-target-aware selection: simulate previews from each reachable movement tile
            // Prepare candidate movement tiles
            var candidateMovementTiles = new List<BoardTile>(PossibleMovementTiles);
            if (!candidateMovementTiles.Contains(Tile)) candidateMovementTiles.Add(Tile);

            // Helper to create fresh SkillPartData list for simulation
            System.Func<List<SO_Skillpart>, List<SkillPartData>> CreateFreshPartDatas = (parts) =>
            {
                var list = new List<SkillPartData>();
                for (int i = 0; i < parts.Count; i++)
                {
                    var spd = new SkillPartData { PartIndex = i, GroupIndex = 0 };
                    list.Add(spd);
                }
                return list;
            };

            int bestHits = 0;
            BoardTile bestMoveTile = Tile;
            BoardTile bestTargetTile = null;
            Unit bestTargetUnit = null;
            List<SkillPartData> bestSimulatedPartDatas = null;

            // Iterate movement options
            foreach (var moveTile in candidateMovementTiles)
            {
                // Build candidate target tiles/units based on firstPart targeting
                var candidateTargetTiles = new List<BoardTile>();
                var candidateTargetUnits = new List<Unit>();

                // If the first part expects a tile target (AOE / line / cone / board-tile), iterate tiles
                if (firstPart is SO_TargetBoardtileSkill || firstPart.TargetTileKind != TargetTileEnum.None)
                {
                    // tiles within max range from moveTile
                    var tiles = boardManager.GetTilesWithinDirectRange(moveTile, firstPart.MaxRange, true);
                    if (tiles != null)
                    {
                        candidateTargetTiles.AddRange(tiles.Where(t => boardManager.GetRangeBetweenTiles(moveTile, t) >= firstPart.MinRange));
                    }
                }
                else
                {
                    // unit targets: characters
                    candidateTargetUnits.AddRange(UnitData.Characters);
                }

                // Ensure at least one candidate to evaluate
                if (candidateTargetTiles.Count == 0 && candidateTargetUnits.Count == 0)
                {
                    // still consider casting with no explicit target (e.g. targeted at caster)
                    candidateTargetTiles.Add(null);
                }

                // Evaluate candidates
                foreach (var candTile in candidateTargetTiles)
                {
                    // Reset SkillData context
                    SkillData.Reset();
                    SkillData.Caster = this;
                    SkillData.SkillPartGroupDatas = new List<SkillPartGroupData> { new SkillPartGroupData() };
                    SkillData.SkillPartGroupDatas[0].SkillPartDatas = CreateFreshPartDatas(skillParts);

                    // Assign fresh PartData instances to skill parts
                    for (int i = 0; i < skillParts.Count; i++)
                    {
                        var part = skillParts[i];
                        part.SkillPartIndex = i;
                        part.PartData = SkillData.GetCurrentSkillPartData(i);
                    }

                    // Preview each part with overwrite origin/target
                    for (int i = 0; i < skillParts.Count; i++)
                    {
                        var part = skillParts[i];
                        part.Preview(candTile, skillParts, this, moveTile, candTile, null);
                    }

                    // Score: unique characters hit by relevant parts
                    var hits = new HashSet<Unit>();
                    for (int i = 0; i < skillParts.Count; i++)
                    {
                        var part = skillParts[i];
                        bool relevant = (part.DamageEffects != null && part.DamageEffects.Count > 0)
                                        || (part.StatusEffects != null && part.StatusEffects.Count > 0)
                                        || (part.displacementEffect != null && part.displacementEffect.UseDisplacement);
                        if (!relevant) continue;

                        if (part.PartData?.TargetsHit != null)
                            foreach (var u in part.PartData.TargetsHit)
                                if (u != null && u is Character)
                                    hits.Add(u);
                    }

                    int score = hits.Count;
                    if (score > bestHits)
                    {
                        bestHits = score;
                        bestMoveTile = moveTile;
                        bestTargetTile = candTile;
                        bestTargetUnit = null;

                        // clone part data for execution later
                        bestSimulatedPartDatas = new List<SkillPartData>();
                        foreach (var p in skillParts)
                        {
                            var src = p.PartData;
                            var clone = new SkillPartData { PartIndex = src.PartIndex, GroupIndex = src.GroupIndex };
                            if (src.TilesHit != null) clone.TilesHit = new List<BoardTile>(src.TilesHit);
                            if (src.TargetsHit != null) clone.TargetsHit = new List<Unit>(src.TargetsHit);
                            bestSimulatedPartDatas.Add(clone);
                        }
                    }
                }

                foreach (var candUnit in candidateTargetUnits)
                {
                    // Skip invalid units too far from the candidate move tile
                    if (candUnit == null) continue;
                    if (boardManager.TileIsBehindClosedTile(moveTile, candUnit.Tile)) continue;
                    var range = boardManager.GetRangeBetweenTiles(moveTile, candUnit.Tile);
                    if (range < firstPart.MinRange || range > firstPart.MaxRange) continue;

                    // Reset SkillData context
                    SkillData.Reset();
                    SkillData.Caster = this;
                    SkillData.SkillPartGroupDatas = new List<SkillPartGroupData> { new SkillPartGroupData() };
                    SkillData.SkillPartGroupDatas[0].SkillPartDatas = CreateFreshPartDatas(skillParts);

                    for (int i = 0; i < skillParts.Count; i++)
                    {
                        var part = skillParts[i];
                        part.SkillPartIndex = i;
                        part.PartData = SkillData.GetCurrentSkillPartData(i);
                    }

                    // Preview using overwrite target unit for unit-target parts
                    for (int i = 0; i < skillParts.Count; i++)
                    {
                        var part = skillParts[i];
                        part.Preview(null, skillParts, this, moveTile, null, candUnit);
                    }

                    // Score unique character hits for relevant parts
                    var hits = new HashSet<Unit>();
                    for (int i = 0; i < skillParts.Count; i++)
                    {
                        var part = skillParts[i];
                        bool relevant = (part.DamageEffects != null && part.DamageEffects.Count > 0)
                                        || (part.StatusEffects != null && part.StatusEffects.Count > 0)
                                        || (part.displacementEffect != null && part.displacementEffect.UseDisplacement);
                        if (!relevant) continue;

                        if (part.PartData?.TargetsHit != null)
                            foreach (var u in part.PartData.TargetsHit)
                                if (u != null && u is Character)
                                    hits.Add(u);
                    }

                    int score = hits.Count;
                    if (score > bestHits)
                    {
                        bestHits = score;
                        bestMoveTile = moveTile;
                        bestTargetTile = null;
                        bestTargetUnit = candUnit;

                        bestSimulatedPartDatas = new List<SkillPartData>();
                        foreach (var p in skillParts)
                        {
                            var src = p.PartData;
                            var clone = new SkillPartData { PartIndex = src.PartIndex, GroupIndex = src.GroupIndex };
                            if (src.TilesHit != null) clone.TilesHit = new List<BoardTile>(src.TilesHit);
                            if (src.TargetsHit != null) clone.TargetsHit = new List<Unit>(src.TargetsHit);
                            bestSimulatedPartDatas.Add(clone);
                        }
                    }
                }
            }

            // If we found no advantageous multi-target cast, fallback to single target
            if (bestHits == 0)
            {
                if (singleTarget == null)
                    yield break;

                StartCoroutine(uiManager.ShowActivityText(CurrentSkill.SkillName));
                yield return new WaitForSeconds(0.3f);

                Rotate(singleTarget.transform.position);
                for (int i = 0; i < skillParts.Count; i++)
                {
                    var part = skillParts[i];
                    part.PartData = new SkillPartData();
                    part.PartData.TargetsHit.Add(singleTarget);
                    StartCoroutine(skillsManager.CastSkillsPart(part, this));
                    if (i < skillParts.Count - 1)
                        yield return new WaitForSeconds(0.2f);
                }

                yield return new WaitForSeconds(2);
            }
            else
            {
                // Execute the best simulated cast
                StartCoroutine(uiManager.ShowActivityText(CurrentSkill.SkillName));
                yield return new WaitForSeconds(0.3f);

                // Move to best movement tile if needed
                if (bestMoveTile != null && bestMoveTile != Tile)
                {
                    boardManager.SetMovementLine(bestMoveTile, false);
                    yield return StartCoroutine(boardManager.MoveToTile());
                }

                // Apply simulated PartData to real parts and cast
                // If we have a preferred unit target, rotate towards it; otherwise rotate towards first hit tile/unit
                if (bestTargetUnit != null)
                    Rotate(bestTargetUnit.transform.position);
                else if (bestSimulatedPartDatas != null)
                {
                    var firstTargets = bestSimulatedPartDatas.SelectMany(x => x.TargetsHit).FirstOrDefault();
                    if (firstTargets != null)
                        Rotate(firstTargets.transform.position);
                }

                for (int i = 0; i < skillParts.Count; i++)
                {
                    var part = skillParts[i];
                    // assign the simulated partdata (or empty if missing)
                    if (bestSimulatedPartDatas != null && i < bestSimulatedPartDatas.Count)
                        part.PartData = bestSimulatedPartDatas[i];
                    else
                        part.PartData = new SkillPartData();

                    StartCoroutine(skillsManager.CastSkillsPart(part, this));
                    if (i < skillParts.Count - 1)
                        yield return new WaitForSeconds(0.2f);
                }

                yield return new WaitForSeconds(2);
            }
        }

        // Pick next skill and update intent for next turn
        CurrentSkill = sequencer.Pick(enemySO?.Skills, false);
        ApplySkillCombatStyle(CurrentSkill);
        (ThisHealthbar as FloatingHealthbar)?.UpdateIntent(CurrentSkill);
    }

    public virtual IEnumerator Attack()
    {
        yield return StartCoroutine(ExecuteSkill());
        EndTurn();
    }

    public void FindOptimalTile()
    {
        boardManager.SetAOE(MoveSpeedLeft, Tile, null);

        var preferredTarget = GetTargetPreference(CurrentSkill.TargetPreference, UnitData.Characters);

        PossibleMovementTiles.Add(Tile);

        PossibleMovementTiles.ForEach(x => x.EnemyPreferenceRating = 0);

        foreach (var tile in PossibleMovementTiles)
        {
            foreach (var character in UnitData.Characters)
            {
                int preferedBonus = character == preferredTarget ? 50 : 0;

                var rangeToCharacter = boardManager.GetRangeBetweenTiles(tile, character.Tile);

                float skillMinRange = CurrentSkill.FirstPart?.MinRange ?? 0f;
                if (rangeToCharacter >= skillMinRange && rangeToCharacter <= CurrentSkill.OptimalRange)
                    tile.EnemyPreferenceRating += 200 + preferedBonus;
                else if (rangeToCharacter < skillMinRange)
                    tile.EnemyPreferenceRating -= 300; // Inside minimum range, can't attack from here

                // Prefer tiles closest to the optimal range
                float distanceFromOptimal = Mathf.Abs(rangeToCharacter - CurrentSkill.OptimalRange);
                tile.EnemyPreferenceRating -= 10 * distanceFromOptimal;
            }

            foreach (var enemy in UnitData.Enemies)
            {
                if (enemy == this)
                    continue;

                var rangeToEnemy = boardManager.GetRangeBetweenTiles(tile, enemy.Tile);

                if (rangeToEnemy <= 5) // TODO Edit this value based on something. Do enemies want to be close to each other? Or far away?
                    tile.EnemyPreferenceRating += 50; // currently it's a bonus to be close to other enemies
            }
        }

        OptimalTile = PossibleMovementTiles.OrderByDescending(x => x.EnemyPreferenceRating).First();

        // Fallback: if the best tile is where we already stand AND we are not already
        // in a valid attack position, approach the preferred target to close the distance.
        float fallbackMinRange = CurrentSkill.FirstPart?.MinRange ?? 0f;
        bool alreadyInRange = UnitData.Characters.Any(c =>
        {
            var r = boardManager.GetRangeBetweenTiles(Tile, c.Tile);
            return r >= fallbackMinRange && r <= CurrentSkill.OptimalRange;
        });
        if (OptimalTile == Tile && !alreadyInRange && UnitData.Characters.Count > 0)
        {
            var approachTarget = preferredTarget ?? GetTargetPreference(TargetEnum.closestTarget, UnitData.Characters);
            if (approachTarget != null)
            {
                var approachTile = PossibleMovementTiles
                    .OrderBy(t => boardManager.GetRangeBetweenTiles(t, approachTarget.Tile))
                    .First();

                if (approachTile != Tile)
                    OptimalTile = approachTile;
            }
        }
    }

    public Unit SetTarget()
    {
        List<Character> targetsInRange = new List<Character>();

        foreach (var character in UnitData.Characters)
        {
            var rangeToCharacter = boardManager.GetRangeBetweenTiles(Tile, character.Tile);

            if (rangeToCharacter <= CurrentSkill.OptimalRange)
                targetsInRange.Add(character);
        }

        var preferedTarget = GetTargetPreference(CurrentSkill.TargetPreference, targetsInRange);
        if (preferedTarget != null)
            return preferedTarget;
        else
            return targetsInRange.FirstOrDefault();
    }

    // Returns the taunting character if this enemy is taunted and the current skill
    // is a single-target offensive skill; null otherwise.
    private Character GetTauntTarget()
    {
        if (CurrentSkill == null) return null;

        // Only override targeting for single-target offensive skills
        if (CurrentSkill.TargetPreference != TargetEnum.closestTarget &&
            CurrentSkill.TargetPreference != TargetEnum.LowestHealthTarget)
            return null;

        var taunt = statusEffects.Find(e => e.statusEfectType == StatusEffectEnum.Taunt);
        if (taunt?.Caster is Character tauntCaster && UnitData.Characters.Contains(tauntCaster))
            return tauntCaster;

        return null;
    }

    public Unit GetTargetPreference(TargetEnum targetPreference, List<Character> targetList)
    {
        // Taunt overrides single-target offensive skill targeting
        var tauntTarget = GetTauntTarget();
        if (tauntTarget != null && targetList.Contains(tauntTarget))
            return tauntTarget;

        switch (targetPreference)
        {
            case TargetEnum.closestTarget:
                Character closestCharacter = null;
                float shortestDistance = float.MaxValue;

                foreach (var character in targetList)
                {
                    var rangeToCharacter = boardManager.GetRangeBetweenTiles(Tile, character.Tile);

                    if (rangeToCharacter < shortestDistance)
                    {
                        shortestDistance = rangeToCharacter;
                        closestCharacter = character;
                    }
                }
                return closestCharacter;

            case TargetEnum.LowestHealthTarget:
                Character lowestHealthCharacter = targetList.OrderBy(x => x.Hitpoints).First();
                return lowestHealthCharacter;
        }

        return null;
    }

    public IEnumerator MoveToTile()
    {
        if (OptimalTile == Tile)
            yield break;

        boardManager.SetMovementLine(OptimalTile, false);
        yield return StartCoroutine(boardManager.MoveToTile());
    }

    // Returns the closest reachable tile within skill range of the preferred target.
    // When multiple tiles share the same move cost, prefers the one already at OptimalRange
    // so Phase 3 repositioning needs less (or no) movement.
    // Returns the current Tile if already in range, or null if skill range is unreachable.
    public BoardTile FindEngageTile()
    {
        boardManager.SetAOE(MoveSpeedLeft, Tile, null);
        if (!PossibleMovementTiles.Contains(Tile))
            PossibleMovementTiles.Add(Tile);

        // Already in range — no Phase 1 movement needed.
        if (IsInSkillRange())
            return Tile;

        var preferredTarget = GetTargetPreference(CurrentSkill.TargetPreference, UnitData.Characters);
        var engageTarget = preferredTarget ?? (UnitData.Characters.Count > 0 ? UnitData.Characters[0] : null);
        if (engageTarget == null) return null;

        float minRange    = CurrentSkill.FirstPart?.MinRange ?? 0f;
        float maxRange    = CurrentSkill.FirstPart?.MaxRange ?? CurrentSkill.OptimalRange;
        float optimalRange = CurrentSkill.OptimalRange;

        var validTiles = PossibleMovementTiles
            .Where(t => t != Tile)
            .Where(t =>
            {
                var r = boardManager.GetRangeBetweenTiles(t, engageTarget.Tile);
                return r >= minRange && r <= maxRange;
            })
            .ToList();

        if (validTiles.Count == 0)
            return null;

        // Least movement first (highest movementLeft = fewest steps taken),
        // then closest to OptimalRange for best Phase 3 setup.
        return validTiles
            .OrderByDescending(t => t.movementLeft)
            .ThenBy(t => Mathf.Abs(boardManager.GetRangeBetweenTiles(t, engageTarget.Tile) - optimalRange))
            .First();
    }

    // Returns true if any player character can be targeted with the current skill from the current tile.
    private bool IsInSkillRange()
    {
        var firstPart = CurrentSkill.FirstPart;
        float minRange = firstPart?.MinRange ?? 0f;
        float maxRange = firstPart?.MaxRange ?? CurrentSkill.OptimalRange;

        return UnitData.Characters.Any(c =>
        {
            if (boardManager.TileIsBehindClosedTile(Tile, c.Tile)) return false;
            var r = boardManager.GetRangeBetweenTiles(Tile, c.Tile);
            return r >= minRange && r <= maxRange;
        });
    }

    // Finds the best reachable tile (within remaining MoveSpeedLeft) to stand at OptimalRange
    // from the preferred target. Used in Phase 3 repositioning.
    public BoardTile FindOptimalRepositionTile()
    {
        if (PossibleMovementTiles.Count == 0) return null;

        var preferredTarget  = GetTargetPreference(CurrentSkill.TargetPreference, UnitData.Characters);
        var repositionTarget = preferredTarget ?? (UnitData.Characters.Count > 0 ? UnitData.Characters[0] : null);
        if (repositionTarget == null) return null;

        float optimalRange = CurrentSkill.OptimalRange;

        return PossibleMovementTiles
            .OrderBy(t => Mathf.Abs(boardManager.GetRangeBetweenTiles(t, repositionTarget.Tile) - optimalRange))
            .First();
    }
}
