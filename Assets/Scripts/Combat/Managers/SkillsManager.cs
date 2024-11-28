using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoardManager))]
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
        boardManager = GetComponent<BoardManager>();
        skillFXManager = GetComponent<SkillFXManager>();
        damageManager = DamageManager.Instance;
        camera = Camera.main;
    }

    void Update()
    {
        if (UnitData.CurrentAction != UnitData.CurrentActionKind.CastingSkillshot)
            return;

        if (Input.GetKeyDown(KeyCode.Mouse0)) // TODO requires tile
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

        if (UnitData.CurrentAction == UnitData.CurrentActionKind.CastingSkillshot)
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

        UnitData.CurrentAction = UnitData.CurrentActionKind.Animating;
        boardManager.Clear();
        StartCoroutine(CastSkills());
    }

    public IEnumerator CastSkills()
    {
        var skill = SkillData.CurrentMainSkill;

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
        var skillFX = skillPart.SKillFX;
        var skillPartData = skillPart.PartData;

        if (skillFX != null)
        {
            skillFX.SetValues(skillPartData);
            yield return StartCoroutine(skillFXManager.Cast(skillFX));
        }

        //foreach (var target in SkillData.GetCurrentTargetsHit(index))
        //{
        //    var data = damageManager.DealDamage(skillPart, target);
        //    damageManager.TakeDamage(data);
        //}
    }
}
