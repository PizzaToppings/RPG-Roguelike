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
    Camera camera;

    public void Init()
    {
        Instance = this;
        boardManager = BoardManager.Instance;
        skillFXManager = GetComponent<SkillFXManager>();
        damageManager = DamageManager.Instance;
        camera = Camera.main;
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

        if (UnitData.CurrentAction == CurrentActionKind.CastingSkillshot && SkillData.CastOnTile == false)
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

        var originTile = SkillData.Caster.currentTile;

        var tileDirectionIndex = 0;
        var mousePosition = GetMousePosition();
        Vector3 dir = (mousePosition - originTile.position).normalized;
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

        UnitData.CurrentAction = CurrentActionKind.Animating;
        boardManager.Clear();
        StartCoroutine(CastSkills(SkillData.CurrentActiveSkill));
    }

    public IEnumerator CastSkills(SO_MainSkill skill)
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
        var index = skillPart.SkillPartIndex;
        var skillFX = skillPart.SkillFX;
        var skillPartData = skillPart.PartData;

        foreach (var target in SkillData.GetCurrentTargetsHit(index))
        {
            if (skillPart.Power > 0)
            {
                var data = damageManager.GetDamageData(skillPart, target);
                damageManager.TakeDamage(data);
            }
        }

        if (skillFX != null)
        {
            foreach (var SFX in skillFX)
			{
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
}
