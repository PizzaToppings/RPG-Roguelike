using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoardManager))]
public class SkillShotManager : MonoBehaviour
{
    public static SkillShotManager skillShotManager;
    BoardManager boardManager;

    public void Init()
    {
        skillShotManager = this;
        boardManager = GetComponent<BoardManager>();
    }

    public void PreviewLine(SO_LineSkillshot data, BoardTile mouseOverTile)
    {
        BoardTile originTile = GetOriginalTile(data);

        List<int> directions = new List<int>();

        foreach (var direction in data.Directions)
        {
            var dir = direction;
            dir += GetNextInLine(originTile, mouseOverTile);
            directions.Add(dir);
        }

        boardManager.PreviewLineCast(originTile, directions.ToArray(), data.Range);
    } 

    int GetNextInLine(BoardTile originTile, BoardTile mouseOverTile)
    {
        int[] diffs = new int[6];

        for (int i = 0; i < originTile.connectedTiles.Length; i++)
        {
            if (originTile.connectedTiles[i] == null)
            {
                diffs[i] = int.MaxValue;
                continue;
            }

            var connectedTile = originTile.connectedTiles[i];
            int xDif = Mathf.Abs(connectedTile.xPosition - mouseOverTile.xPosition);
            int yDif = Mathf.Abs(connectedTile.yPosition - mouseOverTile.yPosition);

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

        return lowestDiffIndex;
    }

    public void GetAOE(SO_AOE_Skillshot data)
    {
        BoardTile originTile = GetOriginalTile(data);
        boardManager.SetMovementLeft(data.Range, originTile);
    }

    BoardTile GetOriginalTile(SO_Skillshot lineSkillshot)
    {
        if (lineSkillshot.OriginTile == SO_Skillshot.OriginTileEnum.Caster)
            return lineSkillshot.Caster.currentTile;

        return null;
    }
}
