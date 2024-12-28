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
    InputManager inputManager;

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
        inputManager = InputManager.Instance;
    }

    public void SetSkills(Character character)
    {
        foreach (var skill in character.skills)
        {
            skill.Init();
        }
    }

    void Update()
    {
        if (UnitData.CurrentAction != CurrentActionKind.CastingSkillshot)
            return;

        //if (Input.GetKeyDown(KeyCode.Mouse0) && !EventSystem.current.IsPointerOverGameObject())
        //{
        //    StartCasting();
        //}

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
                skill.Preview(boardManager.GetCurrentMouseTile());

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

        uiManager.SetSkillIcons(character);

        StartCoroutine(CastSkill(skill));
    }

    public IEnumerator CastSkill(SO_MainSkill skill)
    {
        var index = 0;

        foreach (var spg in skill.SkillPartGroups)
        {
            if (index >= SkillData.SkillPartGroupIndex)
                continue;

            foreach (var sp in spg.skillParts)
            {
                yield return StartCoroutine(CastSkillsPart(sp));
            }
            index++;
        }

        Character character = SkillData.Caster as Character;
        character.StopCasting();
    }

    IEnumerator CastSkillsPart(SO_Skillpart skillPart)
    {
        var skillVFX = skillPart.SkillVFX;
        var skillPartData = skillPart.PartData;

        if (skillVFX != null)
        {
            foreach (var SFX in skillVFX)
			{
                if (SFX.ShowDamage)
                    damageManager.DealDamageSetup(skillPart, SFX.ShowDamageDelay);

                if (SFX.TriggerDisplacement)
                    DisplaceUnit(skillPart.displacementEffect);
                
                SFX.SetValues(skillPartData);
                yield return StartCoroutine(skillVFXManager.Cast(SFX));
			}
        }
    }

    public void DisplaceUnit(SO_DisplacementEffect displacement)
    {
        switch (displacement.DisplacementType)
        {
            case DisplacementEnum.Teleport:
                StartCoroutine(TeleportUnit(displacement));
                return;
            case DisplacementEnum.Move:
                StartCoroutine(MoveUnit(displacement));
                return;
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
            float projectileSpeed = displacement.SpeedCurve.Evaluate(time) * displacement.Speed;

            unit.transform.position = Vector3.MoveTowards(unit.transform.position, pointList[currentIndex + 1], projectileSpeed * Time.deltaTime);

            if (Vector3.Distance(unit.transform.position, pointList[currentIndex + 1]) < 0.1f)
            {
                currentIndex++;
            }

            yield return null;
        }
    }

    public float GetSkillAttackRange()
    {
        var skill = SkillData.CurrentActiveSkill;
        return skill.GetAttackRange();
    }

    public bool CanCastSkill(SO_MainSkill skill)
    {
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
