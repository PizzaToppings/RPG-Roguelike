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
                var dir = GetDirection(originTile, targetTile, SkillData);
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

        var tileDirectionIndex = 0;
        Vector3 dir = targetTile.Coordinates - originTile.Coordinates;
        float targetDirection = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        float?[] directions = new float?[originTile.connectedTiles.Length];

        for (int i = 0; i < originTile.connectedTiles.Length; i++)
        {
            if (originTile.connectedTiles[i] == null)
			{
                directions[i] = null;
                continue;
			}

            dir = originTile.connectedTiles[i].Coordinates - originTile.Coordinates;
			float? direction = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            directions[i] = direction;
        }

        float dif = float.MaxValue;

        for (int i = 0; i < directions.Length; i++)
        {
            if (directions[i] == null)
			{
                continue;
			}

            float difference = Mathf.Abs(targetDirection - (float)directions[i]);

            if (difference < dif)
			{
                dif = difference;
                tileDirectionIndex = i;
            }
        }

        data.FinalDirection = tileDirectionIndex;
        return tileDirectionIndex;
    }

    public void GetAOE(SO_Skillpart data)
    {
         boardManager.SetAOE(data.Range, data.OriginTiles, data);
    }
}
