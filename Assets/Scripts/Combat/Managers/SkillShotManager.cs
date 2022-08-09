using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoardManager))]
public class SkillShotManager : MonoBehaviour
{
    public static SkillShotManager Instance;
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

    public void PreviewLine(SO_LineSkillshot data, BoardTile targetTile)
    {
        List<int> directions = new List<int>();

        foreach (var originTile in data.OriginTiles)
        {
            foreach (var direction in data.Angles)
            {
                var dir = direction;
                dir += GetDirection(originTile, targetTile, data);
                directions.Add(dir);
            }

            boardManager.PreviewLineCast(directions.ToArray(), data);
        }
    } 

    public void PreviewCone(SO_ConeSkillshot data, BoardTile targetTile)
    {
        foreach (var originTile in data.OriginTiles)
        {
            var direction = GetDirection(originTile, targetTile, data);
            boardManager.PreviewConeCast(direction, data);
        }
    }

    int GetDirection(BoardTile originTile, BoardTile targetTile, SO_Skillshot data)
    {
        if (data.TargetTileKind == TargetTileEnum.PreviousDirection)
        {
            data.FinalDirection = data.GetPreviousSkillshot().FinalDirection;
            return data.FinalDirection;
        }

        int[] diffs = new int[6];

        for (int i = 0; i < originTile.connectedTiles.Length; i++)
        {
            if (originTile.connectedTiles[i] == null)
            {
                diffs[i] = int.MaxValue;
                continue;
            }

            var connectedTile = originTile.connectedTiles[i];
            int xDif = Mathf.Abs(connectedTile.xPosition - targetTile.xPosition);
            int yDif = Mathf.Abs(connectedTile.yPosition - targetTile.yPosition);

            diffs[i] = xDif + yDif;
        }

        var lowestDiff = diffs[0];
        var lowestDiffIndex = 0;

        for (int i = 1; i < diffs.Length; i++)
        {
            if (diffs[i] < lowestDiff)
            {
                lowestDiff = diffs[i];
                lowestDiffIndex = i;
            }
        }

        data.FinalDirection = lowestDiffIndex;
        return lowestDiffIndex;
    }

    public void GetAOE(SO_Skillshot data)
    {
         boardManager.SetAOE(data.Range, data.OriginTiles, data.tileColor, data);
    }
}
