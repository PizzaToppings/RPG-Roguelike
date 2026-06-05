using System.Linq;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class SkillsManager : MonoBehaviour
{
    public UnityEvent OnSkillCastComplete = new UnityEvent();
    public static SkillsManager Instance;
    BoardManager boardManager;
    SkillVFXManager skillVFXManager;
    DamageManager damageManager;
    UIManager uiManager;
    UnitManager unitManager;
    ConsumableManager consumableManager;
    StatusEffectManager statusEffectManager;

    public void CreateInstance()
    {
        Instance = this;
    }

    public void Init()
    {
        boardManager = BoardManager.Instance;
        skillVFXManager = GetComponent<SkillVFXManager>();
        damageManager = DamageManager.Instance;
        uiManager = UIManager.Instance;
        unitManager = UnitManager.Instance;
        consumableManager = ConsumableManager.Instance;
        statusEffectManager = StatusEffectManager.Instance;
    }

    public void SetSkills(Character character)
    {
        if (character.Summon == false)
		{
            character.basicAttack.Init();
            character.basicSkill.Init();
		}

        foreach (var skill in character.skills)
        {
            if (skill != null)
                skill.Init();
        }
    }

    void Update()
    {
        if (UnitData.CurrentAction != CurrentActionKind.CastingSkillshot)
            return;

        if (SkillData.CastOnTile == false && SkillData.CastOnTarget == false)
        {
            ((Character)UnitData.ActiveUnit).PreviewSkills(BoardData.CurrentMouseTile);
        }
    }

    public void StartCasting()
    {
        if (SkillData.CurrentSkillPartGroupData.SkillPartDatas.TrueForAll(spd => spd.CanCast) == false)
            return;

        var skill = SkillData.CurrentActiveSkill;
        SkillData.SkillPartGroupIndex++;
        
        if (SkillData.SkillPartGroupIndex < SkillData.SkillPartGroupDatas.Count)
        {
            boardManager.Clear();
            
            if (NoTargetsInRange(skill) == false)
			{
                skill.Preview(BoardData.CurrentMouseTile, UnitData.ActiveUnit);

                return;
			}
        }

        UnitData.CurrentAction = CurrentActionKind.Animating;
        boardManager.Clear();
        skillVFXManager.EndProjectileLine();

        SkillData.SetCharges(skill, SkillData.GetCharges(skill) - 1);
        var character = UnitData.ActiveUnit as Character;
        //character.ConsumeEnergy(skill.EnergyCost);
        if (character != null) character.HasUsedSkillThisTurn = true;

        StartCoroutine(CastSkill(skill, character));

        if (skill.mainSkillSO.IsConsumable)
            consumableManager.DeleteConsumable(skill);

        uiManager.SetSkillIcons(character);
        uiManager.SetConsumableIcons(character);

        if (skill.mainSkillSO.IsBasic == false)
            uiManager.TriggerActivityText(skill.mainSkillSO.SkillName);
    }

    public IEnumerator CastSkill(Skill skill, Unit caster)
    {
        // Set the caster's combat style to match the skill
        if (skill.mainSkillSO.SkillCombatStyle != CombatStyle.None)
        {
            // Do not immediately change CurrentCombatStyle (info panel should update at end of turn).
            caster.PendingCombatStyle = skill.mainSkillSO.SkillCombatStyle;
        }

        foreach (var spg in skill.SkillPartGroups)
        {
            foreach (var sp in spg.skillParts)
            {
                yield return StartCoroutine(CastSkillsPart(sp, caster));
            }
        }

        var character = caster as Character;
        character?.OnSkillCastEvent.Invoke(skill);

        // Record the list of targets hit by this skill on the caster for later systems
        // (e.g. CombatStyle effects) to reference at end of turn.
        try
        {
            caster.LastSkillTargets.Clear();
            if (SkillData.SkillPartGroupDatas != null)
            {
                foreach (var spg in SkillData.SkillPartGroupDatas)
                {
                    if (spg == null || spg.SkillPartDatas == null) continue;
                    foreach (var spd in spg.SkillPartDatas)
                    {
                        if (spd?.TargetsHit == null) continue;
                        foreach (var u in spd.TargetsHit)
                        {
                            if (u == null) continue;
                            if (!caster.LastSkillTargets.Contains(u))
                                caster.LastSkillTargets.Add(u);
                        }
                    }
                }
            }
        }
        catch {
            // Protect against unexpected null refs; not critical.
        }

        OnSkillCastComplete.Invoke();
    }

    public IEnumerator CastSkillsPart(SO_Skillpart skillPart, Unit caster)
    {
        var skillVFX = skillPart.SkillVFX;
        var skillPartData = skillPart.PartData;
        skillPart.DamageEffects.ForEach(x => { x.Caster = UnitData.ActiveUnit; });

        if (skillPart.displacementEffect != null && skillPart.displacementEffect.UseDisplacement)
		{
            if (skillPart.displacementEffect.UseDuration)
                yield return StartCoroutine(DisplaceUnit(skillPart.displacementEffect, skillPart, caster));
            else
                StartCoroutine(DisplaceUnit(skillPart.displacementEffect, skillPart, caster));
        }

        bool hasVFX = skillVFX != null && skillVFX.Length > 0;
        if (hasVFX)
        {
			foreach (var VFX in skillVFX)
			{
                if (VFX.ShowDamage)
                    damageManager.DealDamageSetup(skillPart, VFX.ShowDamageDelay);
			}
        }
        else if (skillPart.DamageEffects.Count > 0)
        {
            // No VFX configured (or empty array) — deal damage immediately.
            damageManager.DealDamageSetup(skillPart, 0f);
        }

        // Temporary 2D stand-in for VFX: dash only when this skill part opts in.
        if (skillPart.DashOnDamageOrEffect)
            yield return StartCoroutine(caster.DashTowards(GetSkillPartTargetPosition(skillPartData, caster)));

        if (skillPart.SummonObject != null)
            unitManager.SummonUnit(skillPart.SummonObject, skillPart.PartData.TilesHit[0], UnitData.ActiveUnit.Friendly);

        if (skillPart.tileEffects.Count > 0)
            AddTileEffects(skillPart);

        if (skillPart.StatusEffects.Count > 0)
            skillPart.StatusEffects.ForEach(x => statusEffectManager.ApplyStatusEffect(x, skillPartData.TargetsHit));
    }

    public void AddTileEffects(SO_Skillpart skillPart)
	{
        foreach (var tile in skillPart.PartData.TilesHit)
		{
            foreach (var te in skillPart.tileEffects)
            {
                var TE = new TileEffect();
                TE.Init(te, tile);
            }
		}
	}

    Vector3 GetSkillPartTargetPosition(SkillPartData skillPartData, Unit caster)
    {
        if (skillPartData.TargetsHit.Count > 0)
            return skillPartData.TargetsHit[0].transform.position;

        if (skillPartData.TilesHit.Count > 0)
            return skillPartData.TilesHit[0].transform.position;

        // Fallback: use the caster's current facing direction
        var sprite = caster.modelSprite;
        Vector3 facingDir = sprite != null && sprite.flipX ? Vector3.left : Vector3.right;
        return caster.transform.position + facingDir;
    }

    public IEnumerator DisplaceUnit(DisplacementEffect displacement, SO_Skillpart skillPart, Unit caster)
    {
        // Validate displacement configuration
        if (displacement.Unit == null)
        {
            Debug.LogError($"DisplacementEffect on {skillPart.name}: Unit field is not assigned!");
            yield break;
        }

        if (displacement.Unit.PartData == null || displacement.Unit.PartData.TargetsHit == null || displacement.Unit.PartData.TargetsHit.Count == 0)
        {
            // No targets were hit (e.g. skill missed) — skip displacement silently.
            yield break;
        }

        if ((displacement.DisplacementType == DisplacementEnum.Move || displacement.DisplacementType == DisplacementEnum.Teleport) && displacement.TargetPosition == null)
        {
            Debug.LogError($"DisplacementEffect on {skillPart.name}: TargetPosition field is not assigned for {displacement.DisplacementType} displacement!");
            yield break;
        }

        if (displacement.TargetPosition != null && (displacement.TargetPosition.PartData == null || displacement.TargetPosition.PartData.TilesHit == null || displacement.TargetPosition.PartData.TilesHit.Count == 0))
        {
            Debug.LogError($"DisplacementEffect on {skillPart.name}: TargetPosition field '{displacement.TargetPosition.name}' has no valid tiles! " +
                          $"Make sure this skill part executes BEFORE the displacement and actually hits tiles.");
            yield break;
        }

        if (displacement.StartVFX)
		{
            displacement.StartVFX.SetValues(skillPart.PartData, caster);
            StartCoroutine(skillVFXManager.Cast(displacement.StartVFX, caster));
        }

        switch (displacement.DisplacementType)
        {
            case DisplacementEnum.Teleport:
                yield return StartCoroutine(TeleportUnit(displacement, skillPart, caster));
                break;

            case DisplacementEnum.Move:
                yield return StartCoroutine(MoveUnit(displacement, skillPart, caster));
                break;

            case DisplacementEnum.Lift:
                yield return StartCoroutine(LiftUnit(displacement, skillPart, caster));
                break;
        }
    }

    public IEnumerator TeleportUnit(DisplacementEffect displacement, SO_Skillpart skillPart, Unit caster)
    {
        var unit = displacement.Unit.PartData.TargetsHit.First();
        var originalPosition = unit.Tile;
        var targetPosition = displacement.TargetPosition.PartData.TilesHit.First();

        yield return new WaitForSeconds(displacement.Delay);

        unit.Tile = targetPosition;
        
        // Only update tile references if moving to a different tile.
        // Guard against clearing a currentUnit that was already replaced by a prior displacement.
        if (originalPosition != targetPosition)
        {
            if (originalPosition.currentUnit == unit)
                originalPosition.currentUnit = null;
            targetPosition.currentUnit = unit;
        }
        
        unit.transform.position = targetPosition.position;


        if (displacement.EndVFX)
        {
            displacement.EndVFX.SetValues(skillPart.PartData, caster);
            StartCoroutine(skillVFXManager.Cast(displacement.EndVFX, caster));
        }

        ToggleDisplacementEndEffects(displacement, skillPart, caster);
    }

    public IEnumerator MoveUnit(DisplacementEffect displacement, SO_Skillpart skillPart, Unit caster)
    {
        var pointList = new List<Vector3>();
        var units = new List<Unit>(displacement.Unit.PartData.TargetsHit);
        var originalTiles = units.Select(x => x.Tile).ToList();
        var targetTiles = new List<BoardTile>(displacement.TargetPosition.PartData.TilesHit);
        var originalPositions = originalTiles.Select(x => x.position).ToList();
        var targetPositions = targetTiles.Select(x => x.position).ToList();
        
        // Clear original tiles, but skip tiles that are also target tiles,
        // and skip tiles whose currentUnit was already replaced by a prior displacement.
        for (int i = 0; i < originalTiles.Count; i++)
        {
            if (!targetTiles.Contains(originalTiles[i]) && originalTiles[i].currentUnit == units[i])
            {
                originalTiles[i].currentUnit = null;
            }
        }

        for (int i = 0; i < units.Count; i++)
        {
            units[i].Tile = targetTiles[i];
            targetTiles[i].currentUnit = units[i];
        }

        yield return new WaitForSeconds(displacement.Delay);

        float time = 0f;

        while (time < 1f)
        {
            var displacementSpeed = displacement.Speed * displacement.SpeedCurve.Evaluate(time);
            time += Time.deltaTime * displacementSpeed;
            time = Mathf.Clamp01(time);

            float height = displacement.HeightCurve.Evaluate(time) * displacement.Height;
            
            for (int i = 0; i < units.Count; i++)
			{
                Vector3 newPosition = Vector3.Lerp(originalPositions[i], targetPositions[i], time) + Vector3.up * height;
                units[i].transform.position = newPosition;
			}

            yield return null;
        }

        for (int i = 0; i < units.Count; i++)
		{
            units[i].transform.position = targetPositions[i];
		}

        if (displacement.EndVFX)
        {
            displacement.EndVFX.SetValues(skillPart.PartData, caster);
            StartCoroutine(skillVFXManager.Cast(displacement.EndVFX, caster));
        }
     
        ToggleDisplacementEndEffects(displacement, skillPart, caster);
    }

    public IEnumerator LiftUnit(DisplacementEffect displacement, SO_Skillpart skillPart, Unit caster)
    {
        var unit = displacement.Unit.PartData.TargetsHit.First();
        var defaultHeight = unit.position.y;

        yield return new WaitForSeconds(displacement.Delay);

        float time = 0f;
        float height = 0f;
        float speed = 1 / displacement.Duration;

        while (time < 1)
        {
            height = defaultHeight + displacement.HeightCurve.Evaluate(time) * displacement.Height;
            time += Time.deltaTime * speed;

            unit.transform.position = new Vector3(unit.transform.position.x, height, unit.transform.position.z);

            yield return null;
        }
        unit.transform.position = new Vector3(unit.transform.position.x, defaultHeight, unit.transform.position.z);

        ToggleDisplacementEndEffects(displacement, skillPart, caster);
    }

    void ToggleDisplacementEndEffects(DisplacementEffect displacement, SO_Skillpart skillPart, Unit caster)
    {
        if (displacement.StartVFX)
        {
            skillVFXManager.EndActiveVFX(displacement.StartVFX);
        }
        if (displacement.EndVFX)
        {
            displacement.EndVFX.SetValues(skillPart.PartData, caster);
            StartCoroutine(skillVFXManager.Cast(displacement.EndVFX, caster));
        }
    }

    public float GetSkillAttackRange()
    {
        var skill = SkillData.CurrentActiveSkill;
        return skill.GetAttackRange();
    }

    public bool CanCastSkill(Skill skill, Unit caster)
    {
        // one skill per turn
        var character = caster as Character;
        if (character != null && character.HasUsedSkillThisTurn)
            return false;

        return SkillData.GetCharges(skill) != 0;
        //return SkillData.GetCharges(skill) != 0 &&
        //    (UnitData.ActiveUnit as Character).Energy >= skill.EnergyCost;
    }

    public bool NoTargetsInRange(Skill skill)
	{
        return skill.SkillPartGroups[SkillData.SkillPartGroupIndex].skillParts.Any(x =>
                x.NoTargetsInRange());
    }

    public void TargetTileWithSkill(BoardTile tile, SO_Skillpart skillpart)
	{
        tile.SetColor(skillpart.tileColor);

        SkillData.AddTileToCurrentList(skillpart.SkillPartIndex, tile);

        var target = boardManager.FindTarget(tile, skillpart);
        SkillData.AddTargetToCurrentList(skillpart.SkillPartIndex, target);
    }
}
