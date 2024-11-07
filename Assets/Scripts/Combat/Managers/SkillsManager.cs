using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoardManager))]
public class SkillsManager : MonoBehaviour
{
    public static SkillsManager Instance;
    BoardManager boardManager;

    public void Init()
    {
        Instance = this;
        boardManager = GetComponent<BoardManager>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            UnitData.CurrentActiveUnit.CastSkill();
        }
    }

    public void PreviewLine(SO_LineSkill SkillData, BoardTile targetTile)
    {
        List<int> directions = new List<int>();

        foreach (var originTile in SkillData.OriginTiles)
        {
            foreach (var direction in SkillData.Angles)
            {
                var dir = direction;
                dir += GetDirection(originTile, targetTile, SkillData);
                directions.Add(dir);
            }

            boardManager.PreviewLineCast(directions.ToArray(), SkillData);
        }
    } 

    public void PreviewCone(SO_ConeSkill data, BoardTile targetTile)
    {
        foreach (var originTile in data.OriginTiles)
        {
            var direction = GetDirection(originTile, targetTile, data);
            boardManager.PreviewConeCast(direction, data);
        }
    }

    int GetDirection(BoardTile originTile, BoardTile targetTile, SO_Skillpart data)
    {
        if (data.TargetTileKind == TargetTileEnum.PreviousDirection)
        {
            data.FinalDirection = data.GetPreviousSkillPart().FinalDirection;
            return data.FinalDirection;
        }

        Vector2 baseDirection = originTile.position - targetTile.position;
        float baseAngle = Mathf.Atan2(baseDirection.x, baseDirection.y) * Mathf.Rad2Deg;
        var angles = new float[originTile.connectedTiles.Length];
        Debug.Log("-----------");

        for (int i = 0; i < originTile.connectedTiles.Length; i++)
        {
            if (originTile.connectedTiles[i] == null)
            {
                angles[i] = 500; // or any other high number
                continue;
            }

            var currentTile = originTile.connectedTiles[i];
            Vector2 tileDirection = originTile.position - currentTile.position;
            float angle = Mathf.Atan2(tileDirection.x, tileDirection.y) * Mathf.Rad2Deg;
            angles[i] = angle;
            Debug.Log(angle);
        }

        var direction = 0;
        var closestAngleDif = Mathf.Abs(baseAngle - angles[0]);
        for (int i = 1; i < angles.Length; i++)
        {
            float angleDif = Mathf.Abs(baseAngle - angles[i]);

            if (angleDif < closestAngleDif)
            {
                closestAngleDif = angleDif;
            }
            direction = i;
        }

        data.FinalDirection = direction;
        return direction;
    }

    public void GetAOE(SO_Skillpart data)
    {
         boardManager.SetAOE(data.Range, data.OriginTiles, data.tileColor, data);
    }
}
