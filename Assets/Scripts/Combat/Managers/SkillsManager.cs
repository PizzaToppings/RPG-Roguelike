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

    public void PreviewLine(SO_LineSkill data, BoardTile targetTile)
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

        var direction = 0;
        var minDistance = Vector2.Distance(originTile.connectedTiles[0].position, targetTile.position);

        Debug.Log("-------------");
        Debug.Log("-------------");
        Debug.Log("-------------");
        Debug.Log($"target: {targetTile.xPosition}, {targetTile.yPosition}");
        Debug.Log("-------------");
        Debug.Log($"Tile 0: {originTile.connectedTiles[0].xPosition}, {originTile.connectedTiles[0].yPosition}");
        Debug.Log($"Distance: {minDistance}");

        for (int i = 1; i < originTile.connectedTiles.Length; i++)
        {
            if (originTile.connectedTiles[i] == null)
            {
                continue;
            }
            Debug.Log("-------------");

            var connectedTile = originTile.connectedTiles[i];

            float distance = Vector2.Distance(connectedTile.position, targetTile.position);
            Debug.Log($"Tile {i}: {originTile.connectedTiles[0].xPosition}, {originTile.connectedTiles[0].yPosition}");
            Debug.Log($"Distance: {distance}");
            if (distance < minDistance)
            {
                minDistance = distance;
                direction = i;
            }
        }
        Debug.Log($"Mininmal Distance: {minDistance} for direction: {direction}");


        data.FinalDirection = direction;
        return direction;
    }

    public void GetAOE(SO_Skillpart data)
    {
         boardManager.SetAOE(data.Range, data.OriginTiles, data.tileColor, data);
    }
}
