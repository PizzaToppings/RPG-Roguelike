using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SkillsManager : MonoBehaviour
{
    public static SkillsManager Instance;
    BoardManager boardManager;
    SkillVFXManager skillVFXManager;
    DamageManager damageManager;
    UIManager uiManager;
    InputManager inputManager;

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

        if (Input.GetKeyDown(KeyCode.Mouse0) && !EventSystem.current.IsPointerOverGameObject())
        {
            StartCasting();
        }

        if (UnitData.CurrentAction == CurrentActionKind.CastingSkillshot 
            && SkillData.CastOnTile == false && SkillData.CastOnTarget == false)
        {
            var currentMouseTile = boardManager.GetCurrentMouseTile();
            UnitData.ActiveUnit.PreviewSkills(currentMouseTile);
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

        var directionAnchorTile = GetDirectionAnchorTile(skillData);

        var tileDirectionIndex = 0;
        var mousePosition = inputManager.GetMousePosition();
        Vector3 dir = (mousePosition - directionAnchorTile.position).normalized;
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

    public BoardTile GetDirectionAnchorTile(SO_Skillpart skillpart)
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
                SkillData.Caster = UnitData.ActiveUnit;
                tiles.Add(SkillData.Caster.Tile);
                break;

            case OriginTileEnum.LastTargetTile:
                if (previousTargetsHit.Count == 0)
                    return null;

                previousTargetsHit.ForEach(x => tiles.Add(x.Tile));
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

    public void GetAOE(SO_Skillpart data)
    {
        boardManager.SetAOE(data.MaxRange, data.OriginTiles, data);
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
        var unit = displacement.Unit.PartData.TargetsHit.First();
        var originalPosition = unit.Tile;
        var targetPosition = displacement.TargetPosition.PartData.TilesHit.First();

        yield return new WaitForSeconds(displacement.Delay);

        //unit.currentTile = targetPosition;
        //originalPosition.currentUnit = null;
        //targetPosition.currentUnit = unit;
        //unit.transform.position = targetPosition.position;
    }
}
