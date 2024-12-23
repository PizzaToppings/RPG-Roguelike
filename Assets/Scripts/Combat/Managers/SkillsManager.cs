using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SkillsManager : MonoBehaviour
{
    public static SkillsManager Instance;
    BoardManager boardManager;
    SkillFXManager skillFXManager;
    DamageManager damageManager;
    UIManager uiManager;
    Camera camera;

    public void CreateInstance()
    {
        Instance = this;
    }


    public void Init()
    {
        boardManager = BoardManager.Instance;
        skillFXManager = GetComponent<SkillFXManager>();
        damageManager = DamageManager.Instance;
        uiManager = UIManager.Instance;
        camera = Camera.main;
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

        if (Input.GetKeyDown(KeyCode.Mouse0) && !EventSystem.current.IsPointerOverGameObject())
        {
            StartCasting();
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (SkillData.SkillPartGroupIndex > 0)
            {
                SkillData.SkillPartGroupIndex--;
            }
            else
            {
                var character = UnitData.CurrentActiveUnit as Character;
                character.StopCasting();
            }
        }

        if (UnitData.CurrentAction == CurrentActionKind.CastingSkillshot 
            && SkillData.CastOnTile == false && SkillData.CastOnTarget == false)
        {
            UnitData.CurrentActiveUnit.PreviewSkills(boardManager.currentMouseTile);
        }
    }

    public void PreviewLine(SO_LineSkill skillData)
    {
        List<int> directions = new List<int>();

        foreach (var originTile in skillData.OriginTiles)
        {
            foreach (var direction in skillData.Angles)
            {
                var dir = direction + GetDirection(skillData);
                directions.Add(dir);
            }

            boardManager.PreviewLineCast(originTile, directions.ToArray(), skillData);
        }
    }

    public void PreviewCone(SO_ConeSkill skillData)
    {
        foreach (var originTile in skillData.OriginTiles)
        {
            var direction = GetDirection(skillData);
            boardManager.PreviewConeCast(direction, skillData);
        }
    }

    public void PreviewHalfCircle(SO_HalfCircleSkill skillData)
    {
        foreach (var originTile in skillData.OriginTiles)
        {
            var direction = GetDirection(skillData);
            boardManager.PreviewHalfCircleCast(direction, skillData);
        }
    }

    int GetDirection(SO_Skillpart skillData)
    {
        if (skillData.TargetTileKind == TargetTileEnum.PreviousDirection)
        {
            skillData.FinalDirection = skillData.GetPreviousSkillPart().FinalDirection;
            return skillData.FinalDirection;
        }

        var directionCenterTile = GetDirectionCenterTile(skillData);

        var tileDirectionIndex = 0;
        var mousePosition = GetMousePosition();
        Vector3 dir = (mousePosition - directionCenterTile.position).normalized;
        Vector2 mouseDirection = new Vector2(Mathf.Round(dir.x), Mathf.Round(dir.z));

        var directions = boardManager.Directions;
        for (int i = 0; i < directions.Length; i++)
        {
            if (mouseDirection == directions[i])
            {
                tileDirectionIndex = i;
                break;
            }
        }

        skillData.FinalDirection = tileDirectionIndex;
        return tileDirectionIndex;
    }

    BoardTile GetDirectionCenterTile(SO_Skillpart skillpart)
	{
        var tiles = new List<BoardTile>();
        var previousTargetsHit = new List<Unit>();
        var previousTilesHit = new List<BoardTile>();

        if (skillpart.SkillPartIndex > 0)
        {
            previousTargetsHit = SkillData.GetPreviousTargetsHit(skillpart.SkillPartIndex);
            previousTilesHit = SkillData.GetPreviousTilesHit(skillpart.SkillPartIndex);
        }

        switch (skillpart.DirectionAnchor)
        {
            case OriginTileEnum.Caster:
                SkillData.Caster = UnitData.CurrentActiveUnit;
                tiles.Add(SkillData.Caster.currentTile);
                break;

            case OriginTileEnum.LastTargetTile:
                if (previousTargetsHit.Count == 0)
                    return null;

                previousTargetsHit.ForEach(x => tiles.Add(x.currentTile));
                break;

            case OriginTileEnum.LastTile:
                tiles.AddRange(previousTilesHit);
                break;

            case OriginTileEnum.GetFromSkillPart:
                var tileList = skillpart.OriginTileSkillParts.SelectMany(x => x.PartData.TilesHit).ToList();
                tiles.AddRange(tileList);
                break;
        }

        return tiles[0];
    }

    Vector3 GetMousePosition()
    {
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, Vector3.zero);

        float distance;
        if (plane.Raycast(ray, out distance))
        {
            return ray.GetPoint(distance);
        }

        return Vector3.zero;
    }

    public void GetAOE(SO_Skillpart data)
    {
        boardManager.SetAOE(data.Range, data.OriginTiles, data);
    }

    void StartCasting()
    {
        if (SkillData.SkillPartGroupIndex < SkillData.SkillPartGroupDatas.Count - 1)
        {
            SkillData.SkillPartGroupIndex++;
            return;
        }

        var skill = SkillData.CurrentActiveSkill;

        UnitData.CurrentAction = CurrentActionKind.Animating;
        boardManager.Clear();
        skillFXManager.EndProjectileLine();

        skill.Charges--;
        var character = UnitData.CurrentActiveUnit as Character;
        character.Energy -= skill.EnergyCost;

        var portrait = character.ThisHealthbar as CharacterPortrait;
        portrait.UpdateHealthbar(); // includes energybar

        uiManager.SetSkillIcons(character);

        StartCoroutine(CastSkill(skill));
    }

    public IEnumerator CastSkill(SO_MainSkill skill)
    {
        foreach (var spg in skill.SkillPartGroups)
        {
            foreach (var sp in spg.skillParts)
            {
                yield return StartCoroutine(CastSkillsPart(sp));
            }
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
                yield return StartCoroutine(skillFXManager.Cast(SFX));
			}
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
            (UnitData.CurrentActiveUnit as Character).Energy >= skill.EnergyCost;
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
        var originalPosition = unit.currentTile;
        var targetPosition = displacement.TargetPosition.PartData.TilesHit.First();

        yield return new WaitForSeconds(displacement.Delay);

        unit.currentTile = targetPosition;
        originalPosition.currentUnit = null;
        targetPosition.currentUnit = unit;
        unit.transform.position = targetPosition.position;
    }

    public IEnumerator MoveUnit(SO_DisplacementEffect displacement)
    {
        var unit = displacement.Unit.PartData.TargetsHit.First();
        var originalPosition = unit.currentTile;
        var targetPosition = displacement.TargetPosition.PartData.TilesHit.First();

        yield return new WaitForSeconds(displacement.Delay);

        //unit.currentTile = targetPosition;
        //originalPosition.currentUnit = null;
        //targetPosition.currentUnit = unit;
        //unit.transform.position = targetPosition.position;
    }
}
