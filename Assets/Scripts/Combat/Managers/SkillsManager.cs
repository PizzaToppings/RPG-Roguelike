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
            var currentMouseTile = boardManager.GetCurrentMouseTile();
            UnitData.ActiveUnit.PreviewSkills(currentMouseTile);
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
                skill.Preview(boardManager.GetCurrentMouseTile(), UnitData.ActiveUnit);

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

        Character character = SkillData.Caster as Character;
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
        if (displacement.VFX)
		{
            displacement.VFX.SetValues(skillPart.PartData, skillPart.DamageEffect);
            StartCoroutine(skillVFXManager.Cast(displacement.VFX, caster));
        }

        switch (displacement.DisplacementType)
        {
            case DisplacementEnum.Teleport:
                yield return StartCoroutine(TeleportUnit(displacement));
                break;

            case DisplacementEnum.Move:
                yield return StartCoroutine(MoveUnit(displacement));
                break;

            case DisplacementEnum.Lift:
                yield return StartCoroutine(LiftUnit(displacement));
                break;
        }
    }

    public IEnumerator TeleportUnit(SO_DisplacementEffect displacement)
    {
        var unit = displacement.Unit.PartData.TargetsHit.First();
        var originalPosition = unit.Tile;
        var targetPosition = displacement.TargetPosition.PartData.TilesHit.First();

        yield return new WaitForSeconds(displacement.Delay);

        unit.Tile = targetPosition;
        originalPosition.currentUnit = null;
        targetPosition.currentUnit = unit;
        unit.transform.position = targetPosition.position;
    }

    public IEnumerator MoveUnit(SO_DisplacementEffect displacement)
    {
        var pointList = new List<Vector3>();
        var unit = displacement.Unit.PartData.TargetsHit.First();
        var originalTile = unit.Tile;
        var targetTile = displacement.TargetPosition.PartData.TilesHit.First();
        var originalPosition = originalTile.position;
        var targetPosition = targetTile.position;

        unit.Tile = targetTile;
        originalTile.currentUnit = null;
        targetTile.currentUnit = unit;

        yield return new WaitForSeconds(displacement.Delay);

        var heightOffset = Vector3.Distance(originalPosition, targetPosition);
        var middleOffset = Vector3.up * heightOffset * displacement.Offset * 0.5f;

        Vector3 middlePosition = Vector3.Lerp(originalPosition, targetPosition, 0.5f) + middleOffset;

        for (float ratio = 0; ratio <= 1; ratio += 1f / displacementVertexCount)
        {
            var tangent1 = Vector3.Lerp(targetPosition, middlePosition, ratio);
            var tangent2 = Vector3.Lerp(middlePosition, originalPosition, ratio);
            var curve = Vector3.Lerp(tangent1, tangent2, ratio);
            pointList.Add(curve);
        }
        pointList.Reverse();

        float time = 0f;
        int currentIndex = 0;

        while (currentIndex < pointList.Count - 1)
        {
            time += Time.deltaTime;
            float speed = displacement.SpeedCurve.Evaluate(time) * displacement.Speed;

            unit.transform.position = Vector3.MoveTowards(unit.transform.position, pointList[currentIndex + 1], speed * Time.deltaTime);

            if (Vector3.Distance(unit.transform.position, pointList[currentIndex + 1]) < 0.1f)
            {
                currentIndex++;
            }

            yield return null;
        }
    }

    public IEnumerator LiftUnit(SO_DisplacementEffect displacement)
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
