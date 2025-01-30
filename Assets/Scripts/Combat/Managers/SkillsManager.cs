using System.Linq;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class SkillsManager : MonoBehaviour
{
    public static SkillsManager Instance;
    BoardManager boardManager;
    SkillVFXManager skillVFXManager;
    DamageManager damageManager;
    UIManager uiManager;
    UnitManager unitManager;
    ConsumableManager consumableManager;
    StatusEffectManager statusEffectManager;

    float displacementVertexCount = 12;

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

        if (UnitData.CurrentAction == CurrentActionKind.CastingSkillshot 
            && SkillData.CastOnTile == false && SkillData.CastOnTarget == false)
        {
            UnitData.ActiveUnit.PreviewSkills(BoardData.CurrentMouseTile);
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

        skill.Charges--;
        var character = UnitData.ActiveUnit as Character;
        character.Energy -= skill.EnergyCost;

        var portrait = character.ThisHealthbar as CharacterPortrait;
        portrait.UpdateHealthbar(); // includes energybar

        StartCoroutine(CastSkill(skill, character));

        if (skill.IsConsumable)
            consumableManager.DeleteConsumable(skill);

        uiManager.SetSkillIcons(character);
        uiManager.SetConsumableIcons(character);

        if (skill.IsBasic == false)
            uiManager.TriggerActivityText(skill.SkillName);
    }

    public IEnumerator CastSkill(SO_MainSkill skill, Unit caster)
    {
        foreach (var spg in skill.SkillPartGroups)
        {
            foreach (var sp in spg.skillParts)
            {
                yield return StartCoroutine(CastSkillsPart(sp, caster));
            }
        }

        Character character = caster as Character;
        character.StopCasting();
    }

    public IEnumerator CastSkillsPart(SO_Skillpart skillPart, Unit caster)
    {
        var skillVFX = skillPart.SkillVFX;
        var skillPartData = skillPart.PartData;
        skillPart.DamageEffect.Caster = UnitData.ActiveUnit;

        if (skillPart.displacementEffect != null)
		{
            if (skillPart.displacementEffect.UseDuration)
                yield return StartCoroutine(DisplaceUnit(skillPart.displacementEffect, skillPart, caster));
            else
                StartCoroutine(DisplaceUnit(skillPart.displacementEffect, skillPart, caster));
        }

        if (skillVFX != null)
        {
			foreach (var VFX in skillVFX)
			{
                if (VFX.ShowDamage)
                    damageManager.DealDamageSetup(skillPart, VFX.ShowDamageDelay);

                VFX.SetValues(skillPartData, skillPart.DamageEffect);
                yield return StartCoroutine(skillVFXManager.Cast(VFX, caster));
			}
        }

        if (skillPart.SummonObject != null)
            unitManager.SummonUnit(skillPart.SummonObject, skillPart.PartData.TilesHit[0], UnitData.ActiveUnit.Friendly);

        if (skillPart.tileEffects.Count > 0)
            AddTileEffects(skillPart);

        if (skillPart.StatusEfects.Count > 0)
            skillPart.StatusEfects.ForEach(x => statusEffectManager.ApplyStatusEffect(x, skillPartData.TargetsHit));
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

    public IEnumerator DisplaceUnit(SO_DisplacementEffect displacement, SO_Skillpart skillPart, Unit caster)
    {
        if (displacement.StartVFX)
		{
            displacement.StartVFX.SetValues(skillPart.PartData, skillPart.DamageEffect);
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

    public IEnumerator TeleportUnit(SO_DisplacementEffect displacement, SO_Skillpart skillPart, Unit caster)
    {
        var unit = displacement.Unit.PartData.TargetsHit.First();
        var originalPosition = unit.Tile;
        var targetPosition = displacement.TargetPosition.PartData.TilesHit.First();

        yield return new WaitForSeconds(displacement.Delay);

        unit.Tile = targetPosition;
        originalPosition.currentUnit = null;
        targetPosition.currentUnit = unit;
        unit.transform.position = targetPosition.position;


        if (displacement.EndVFX)
        {
            displacement.EndVFX.SetValues(skillPart.PartData, skillPart.DamageEffect);
            StartCoroutine(skillVFXManager.Cast(displacement.EndVFX, caster));
        }

        ToggleDisplacementEndEffects(displacement, skillPart, caster);
    }

    public IEnumerator MoveUnit(SO_DisplacementEffect displacement, SO_Skillpart skillPart, Unit caster)
    {
        var pointList = new List<Vector3>();
        var units = new List<Unit>(displacement.Unit.PartData.TargetsHit);
        var originalTiles = units.Select(x => x.Tile).ToList();
        var targetTiles = new List<BoardTile>(displacement.TargetPosition.PartData.TilesHit);
        var originalPositions = originalTiles.Select(x => x.position).ToList();
        var targetPositions = targetTiles.Select(x => x.position).ToList();
        originalTiles.ForEach(x => x.currentUnit = null);

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
            displacement.EndVFX.SetValues(skillPart.PartData, skillPart.DamageEffect);
            StartCoroutine(skillVFXManager.Cast(displacement.EndVFX, caster));
        }
     
        ToggleDisplacementEndEffects(displacement, skillPart, caster);
    }

    public IEnumerator LiftUnit(SO_DisplacementEffect displacement, SO_Skillpart skillPart, Unit caster)
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

    void ToggleDisplacementEndEffects(SO_DisplacementEffect displacement, SO_Skillpart skillPart, Unit caster)
    {
        if (displacement.StartVFX)
        {
            skillVFXManager.EndActiveVFX(displacement.StartVFX);
        }
        if (displacement.EndVFX)
        {
            displacement.EndVFX.SetValues(skillPart.PartData, skillPart.DamageEffect);
            StartCoroutine(skillVFXManager.Cast(displacement.EndVFX, caster));
        }
    }

    public float GetSkillAttackRange()
    {
        var skill = SkillData.CurrentActiveSkill;
        return skill.GetAttackRange();
    }

    public bool CanCastSkill(SO_MainSkill skill, Unit caster)
    {
        // silenced
        if (skill.IsMagical && statusEffectManager.UnitHasStatusEffect(caster, StatusEfectEnum.Silenced))
            return false;

        // blinded
        if (skill.IsMagical == false && statusEffectManager.UnitHasStatusEffect(caster, StatusEfectEnum.Blinded))
            return false;

        return skill.Charges != 0 &&
            (UnitData.ActiveUnit as Character).Energy >= skill.EnergyCost;
    }

    public bool NoTargetsInRange(SO_MainSkill skill)
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
