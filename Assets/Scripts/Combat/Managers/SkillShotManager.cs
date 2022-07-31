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

        var direction = data.Direction;
        direction += GetDirection(originTile, mouseOverTile);

        boardManager.PreviewLineCast(originTile, direction, data.Range);
    } 

    int GetDirection(BoardTile originTile, BoardTile mouseOverTile)
    {
        int[] diffs = new int[6];

        for (int i = 0; i < originTile.connectedTiles.Length; i++)
        {
            var connectedTile = originTile.connectedTiles[i];
            int xDif = Mathf.Abs(connectedTile.xPosition - mouseOverTile.xPosition);
            int yDif = Mathf.Abs(connectedTile.yPosition - mouseOverTile.yPosition);

            diffs[i] = xDif + yDif;
        }

        var lowestDiff = diffs[0];
        var lowestDiffIndex = 0;
        Debug.LogWarning(diffs[0]);


        for (int i = 1; i < diffs.Length; i++)
        {
            Debug.Log(diffs[i]);
            if (diffs[i] < lowestDiff)
            {
                lowestDiff = diffs[i];
                lowestDiffIndex = i;
            }
        }

        return lowestDiffIndex;
    }

    BoardTile GetOriginalTile(SO_LineSkillshot lineSkillshot)
    {
        if (lineSkillshot.OriginTile == SO_Skillshot.OriginTileEnum.Caster)
            return lineSkillshot.Caster.currentTile;

        return null;
    }
}
