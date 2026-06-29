using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBaseAI : Enemy
{
    // -----------------------------------------------------------------------
    // Runtime state
    // -----------------------------------------------------------------------
    [HideInInspector] public BoardTile OptimalTile;
    [HideInInspector] public Unit Target;
    [HideInInspector] public SO_EnemySkill CurrentSkill;

    [HideInInspector] public EnemyAIContext AIContext;

    // Runtime Skill instances keyed by their SO_EnemySkill slot
    private Dictionary<SO_EnemySkill, Skill> _runtimeSkills = new Dictionary<SO_EnemySkill, Skill>();

    // Pre-computed intent data updated whenever CurrentSkill or Target changes
    [HideInInspector] public List<BoardTile> NextSkillPreviewTiles = new List<BoardTile>();
    [HideInInspector] public Unit NextTarget;
    [HideInInspector] public BoardTile NextTargetTile;


    public override void Init()
    {
        base.Init();

        AIContext = new EnemyAIContext { Enemy = this };

        // Build runtime Skill instances for every SO_EnemySkill that has an SO_MainSkill
        _runtimeSkills.Clear();
        if (enemySO != null)
        {
            foreach (var slot in enemySO.Skills)
            {
                if (slot?.Skill == null) continue;
                var runtimeSkill = new Skill();
                runtimeSkill.Init(slot.Skill);
                _runtimeSkills[slot] = runtimeSkill;
            }
        }

        SelectNextSkill();
    }

    public override List<SO_SKillVFX> GetSkillVFXList()
    {
        var result = new List<SO_SKillVFX>();
        foreach (var skill in _runtimeSkills.Values)
            foreach (var spg in skill.SkillPartGroups)
                foreach (var sp in spg.skillParts)
                    if (sp?.SkillVFX != null) result.AddRange(sp.SkillVFX);
        return result;
    }

    // -----------------------------------------------------------------------
    // Skill selection
    // -----------------------------------------------------------------------

    /// <summary>
    /// Chooses the next skill to announce as intent.
    /// Called at Init and at the end of each turn — the only place CurrentSkill changes.
    /// </summary>
    public void SelectNextSkill()
    {
        if (enemySO == null || enemySO.Skills == null || enemySO.Skills.Count == 0) return;

        SO_EnemySkill chosen = null;

        // 1. Forced follow-up always wins
        if (AIContext.ForcedNextSkill != null)
        {
            chosen = AIContext.ForcedNextSkill;
            AIContext.ForcedNextSkill = null;
        }
        else if (enemySO.AlwaysStartWithFirstSkill && AIContext.TurnCount == 0)
        {
            chosen = enemySO.Skills[0];
        }
        else
        {
            // 2. Filter by conditions
            var eligible = new List<SO_EnemySkill>();
            foreach (var slot in enemySO.Skills)
                if (slot != null && slot.AreConditionsMet(AIContext)) eligible.Add(slot);

            // 3. Delegate to strategy
            var strategy = enemySO.SkillSelectionStrategy;
            if (strategy != null && eligible.Count > 0)
                chosen = strategy.SelectNextSkill(AIContext, eligible);

            // 4. Fallback: first eligible or first skill
            if (chosen == null)
                chosen = eligible.Count > 0 ? eligible[0] : enemySO.Skills[0];
        }

        CurrentSkill = chosen;

        // 5. Store forced follow-up for next turn
        if (chosen?.ForcedNextSkill != null)
            AIContext.ForcedNextSkill = chosen.ForcedNextSkill;

        // 6. Pre-compute intent targeting preview
        UpdateIntentPreview();

        // 7. Refresh healthbar intent display
        if (ThisHealthbar is FloatingHealthbar fb)
            fb.InitIntent(CurrentSkill);
    }

    private void UpdateIntentPreview()
    {
        NextSkillPreviewTiles.Clear();
        NextTarget = null;
        NextTargetTile = null;

        if (CurrentSkill?.Skill == null || Tile == null) return;

        var result = EnemySkillSimulator.SimulateSkill(this, CurrentSkill, Tile, enemySO?.EffectWeights);
        NextSkillPreviewTiles = result.AllTilesHit ?? new List<BoardTile>();
        NextTarget = result.BestTargetUnit;
        NextTargetTile = result.BestTargetTile;
    }

    // -----------------------------------------------------------------------
    // Turn flow
    // -----------------------------------------------------------------------

    public override IEnumerator StartTurn()
    {
        AIContext.TurnCount++;
        UnitData.CurrentAction = CurrentActionKind.EnemyTurn;

        PossibleMovementTiles = new List<BoardTile>();
        yield return StartCoroutine(base.StartTurn());

        if (CurrentSkill == null)
        {
            EndTurn();
            yield break;
        }

        // Phase 1: Move toward engage tile
        Target = SetTarget();

        // Populate reachable tiles list
        boardManager.SetAOE(MoveSpeedLeft, Tile, null);

        var engageTile = FindEngageTile();
        if (engageTile != null && engageTile != Tile)
        {
            OptimalTile = engageTile;
            yield return StartCoroutine(MoveToTile());
        }

        // Phase 2: Execute skill if now in range
        if (IsInSkillRange())
            yield return StartCoroutine(ExecuteSkill());

        // Phase 3: Reposition at optimal range using remaining movement
        var repositionTile = FindOptimalRepositionTile();
        if (repositionTile != null && repositionTile != Tile)
        {
            OptimalTile = repositionTile;
            yield return StartCoroutine(MoveToTile());
        }

        // Determine next turn's skill
        SelectNextSkill();

        EndTurn();
    }

    // -----------------------------------------------------------------------
    // Skill execution
    // -----------------------------------------------------------------------

    protected virtual IEnumerator ExecuteSkill()
    {
        if (CurrentSkill?.Skill == null || !_runtimeSkills.TryGetValue(CurrentSkill, out var runtimeSkill))
            yield break;

        // Re-simulate targeting from current tile (enemy may have moved)
        var result = EnemySkillSimulator.SimulateSkill(this, CurrentSkill, Tile, enemySO?.EffectWeights);
        NextTarget     = result.BestTargetUnit;
        NextTargetTile = result.BestTargetTile;
        NextSkillPreviewTiles = result.AllTilesHit ?? new List<BoardTile>();

        if (!result.HasValidTarget && CurrentSkill.MinTargetCount > 0)
            yield break;

        InitSkillDataForEnemy(runtimeSkill);
        SetSkillTargets(runtimeSkill, result);

        yield return StartCoroutine(SkillsManager.Instance.CastSkill(runtimeSkill, this));

        AIContext.RecordSkillUsed(CurrentSkill, AIContext.TurnCount);
    }

    public virtual IEnumerator Attack()
    {
        yield return StartCoroutine(ExecuteSkill());
        EndTurn();
    }

    // -----------------------------------------------------------------------
    // Targeting
    // -----------------------------------------------------------------------

    public Unit SetTarget()
    {
        if (CurrentSkill == null || UnitData.Characters.Count == 0) return null;

        var tauntTarget = GetTauntTarget();
        if (tauntTarget != null) return tauntTarget;

        var candidates = EnemySkillSimulator.GetValidCharactersInRange(
            this, CurrentSkill, Tile, float.MaxValue);
        return candidates.Count > 0 ? candidates[0] : null;
    }

    private Character GetTauntTarget()
    {
        if (CurrentSkill == null) return null;

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
        var tauntTarget = GetTauntTarget();
        if (tauntTarget != null && targetList.Contains(tauntTarget))
            return tauntTarget;

        if (targetList.Count == 0) return null;

        switch (targetPreference)
        {
            case TargetEnum.closestTarget:
                return targetList.OrderBy(c => boardManager.GetRangeBetweenTiles(Tile, c.Tile)).FirstOrDefault();
            case TargetEnum.LowestHealthTarget:
                return targetList.OrderBy(c => c.Hitpoints).FirstOrDefault();
            default:
                return targetList[0];
        }
    }

    // -----------------------------------------------------------------------
    // Movement
    // -----------------------------------------------------------------------

    public IEnumerator MoveToTile()
    {
        if (OptimalTile == null || OptimalTile == Tile)
            yield break;

        boardManager.SetMovementLine(OptimalTile, false);
        yield return StartCoroutine(boardManager.MoveToTile());
    }

    public BoardTile FindEngageTile()
    {
        if (Target == null || CurrentSkill?.Skill == null) return null;

        float attackRange  = CurrentSkill.Skill.GetAttackRange();
        float optimalRange = enemySO?.AIProfile?.OptimalRange ?? 1.5f;

        // Already in range
        if (boardManager.GetRangeBetweenTiles(Tile, Target.Tile) <= attackRange)
            return Tile;

        BoardTile best = null;
        float bestMoveCost   = float.MaxValue;
        float bestRangeDiff  = float.MaxValue;

        foreach (var tile in PossibleMovementTiles)
        {
            if (tile == null || tile.currentUnit != null) continue;
            float distToTarget = boardManager.GetRangeBetweenTiles(tile, Target.Tile);
            if (distToTarget > attackRange) continue;

            // Tiles with higher movementLeft were reached with less movement cost
            float moveCost  = MoveSpeed - tile.movementLeft;
            float rangeDiff = Mathf.Abs(distToTarget - optimalRange);

            if (moveCost < bestMoveCost || (Mathf.Approximately(moveCost, bestMoveCost) && rangeDiff < bestRangeDiff))
            {
                bestMoveCost  = moveCost;
                bestRangeDiff = rangeDiff;
                best = tile;
            }
        }

        return best;
    }

    private bool IsInSkillRange()
    {
        if (Target == null || CurrentSkill?.Skill == null) return false;
        float attackRange = CurrentSkill.Skill.GetAttackRange();
        return boardManager.GetRangeBetweenTiles(Tile, Target.Tile) <= attackRange;
    }

    public void FindOptimalTile()
    {
        OptimalTile = FindOptimalRepositionTile();
    }

    public BoardTile FindOptimalRepositionTile()
    {
        if (Target == null) return null;

        float optimalRange   = enemySO?.AIProfile?.OptimalRange ?? 1.5f;
        float minRange       = enemySO?.AIProfile?.MinRange ?? 0f;
        bool  preferNearAllies = enemySO?.AIProfile?.PreferNearAllies ?? false;

        BoardTile best      = null;
        float     bestScore = float.MaxValue;

        foreach (var tile in PossibleMovementTiles)
        {
            if (tile == null || tile.currentUnit != null) continue;
            float distToTarget = boardManager.GetRangeBetweenTiles(tile, Target.Tile);
            if (distToTarget < minRange) continue;

            float rangeDiff = Mathf.Abs(distToTarget - optimalRange);
            float allyScore = 0f;

            if (preferNearAllies)
            {
                float totalAllyDist = 0f;
                int   allyCount     = 0;
                foreach (var enemy in UnitData.Enemies)
                {
                    if (enemy == this || enemy.Tile == null) continue;
                    totalAllyDist += boardManager.GetRangeBetweenTiles(tile, enemy.Tile);
                    allyCount++;
                }
                allyScore = allyCount > 0 ? totalAllyDist / allyCount : 0f;
            }

            float score = rangeDiff + allyScore * 0.5f;
            if (score < bestScore) { bestScore = score; best = tile; }
        }

        return best;
    }

    // -----------------------------------------------------------------------
    // SkillData setup for enemy execution
    // -----------------------------------------------------------------------

    private void InitSkillDataForEnemy(Skill skill)
    {
        SkillData.CurrentActiveSkill   = skill;
        SkillData.Caster               = this;
        SkillData.SkillPartGroupIndex  = 0;
        SkillData.SkillPartGroupDatas  = new List<SkillPartGroupData>();

        for (int g = 0; g < skill.SkillPartGroups.Count; g++)
        {
            var spg    = skill.SkillPartGroups[g];
            var spgData = new SkillPartGroupData { GroupIndex = g };

            bool castOnTile   = spg.skillParts.Count > 0 && spg.skillParts[0] is SO_TargetBoardtileSkill;
            bool castOnTarget = spg.skillParts.Count > 0 && spg.skillParts[0] is SO_TargetUnitSkill;
            spgData.CastOnTile   = castOnTile;
            spgData.CastOnTarget = castOnTarget;

            for (int p = 0; p < spg.skillParts.Count; p++)
            {
                var sp = spg.skillParts[p];
                if (sp == null) continue;

                var spData = new SkillPartData
                {
                    PartIndex  = p,
                    GroupIndex = g,
                    CanCast    = true,
                    TilesHit   = new List<BoardTile>(),
                    TargetsHit = new List<Unit>()
                };
                sp.PartData       = spData;
                sp.SkillPartIndex = p;
                spgData.SkillPartDatas.Add(spData);
            }

            SkillData.SkillPartGroupDatas.Add(spgData);
        }
    }

    private void SetSkillTargets(Skill runtimeSkill, SimulationResult result)
    {
        if (runtimeSkill.SkillPartGroups.Count == 0) return;

        var firstGroup  = runtimeSkill.SkillPartGroups[0];
        int targetIndex = 0;

        foreach (var sp in firstGroup.skillParts)
        {
            if (sp == null) continue;

            if (sp is SO_TargetUnitSkill targetUnitPart)
            {
                if (targetIndex < result.AllTargetsHit.Count)
                {
                    targetUnitPart.TargetUnit(result.AllTargetsHit[targetIndex]);
                    targetIndex++;
                }
            }
            else if (sp is SO_TargetBoardtileSkill)
            {
                if (result.BestTargetTile != null)
                {
                    sp.PartData.TilesHit = new List<BoardTile> { result.BestTargetTile };
                    sp.PartData.CanCast  = true;
                }
            }
            else if (sp is SO_TargetSelfSkill)
            {
                sp.PartData.TargetsHit = new List<Unit>      { this };
                sp.PartData.TilesHit   = new List<BoardTile> { Tile };
                sp.PartData.CanCast    = true;
            }
            else
            {
                // Directional / AoE parts: point toward best target tile
                var refTile = result.BestTargetTile ?? result.BestTargetUnit?.Tile;
                if (refTile != null && Tile != null)
                    sp.FinalDirection = GetDirectionIndex(Tile, refTile);

                sp.PartData.TilesHit   = result.AllTilesHit  ?? new List<BoardTile>();
                sp.PartData.TargetsHit = result.AllTargetsHit ?? new List<Unit>();
                sp.PartData.CanCast    = true;
            }
        }
    }

    private int GetDirectionIndex(BoardTile from, BoardTile to)
    {
        Vector3 dir     = (to.position - from.position).normalized;
        float bestDot   = float.MinValue;
        int   bestIndex = 0;

        for (int i = 0; i < from.connectedTiles.Length; i++)
        {
            var neighbor = from.connectedTiles[i];
            if (neighbor == null) continue;
            Vector3 neighborDir = (neighbor.position - from.position).normalized;
            float   dot         = Vector3.Dot(dir, neighborDir);
            if (dot > bestDot) { bestDot = dot; bestIndex = i; }
        }

        return bestIndex;
    }
}
