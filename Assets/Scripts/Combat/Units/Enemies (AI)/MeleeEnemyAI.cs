using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemyAI : Enemy
{
    BoardTile optimalTile;
    int lowestThreat = 0;

    public override IEnumerator StartTurn()
    {
        StartCoroutine(base.StartTurn());
        yield return null;

		//FindOptimalTile();
		//yield return new WaitUntil(() => CurrentUnitAction == currentUnitAction.Nothing);
		yield return new WaitForSeconds(1);
		Attack();
    }

    void FindOptimalTile()
    {
        var tilesAdjecentToCharacters = new List<BoardTile>();
        var characters = UnitData.Characters;

        foreach (var character in characters)
		{
            var tile = character.Tile;

            var tiles = tile.connectedTiles;

            foreach (BoardTile bt in tiles)
			{
                if (bt == null)
                    continue;

                tilesAdjecentToCharacters.Add(bt);
			}
		}

        foreach (var tile in tilesAdjecentToCharacters)
		{
            var totaldistance = 0;
            foreach (var character in characters)
			{
                totaldistance += boardManager.GetRangeBetweenTiles(character.Tile, tile);
			}

            if (totaldistance > lowestThreat)
			{
                lowestThreat = totaldistance;
                optimalTile = tile;
			}
        }
        boardManager.SetPath(optimalTile);
        StartCoroutine(boardManager.MoveToTile());
    } // EDIT: GetTilesWithinActionRange, based on attack distance. Get a rangeToUnit List for all characters? 

    void Attack()
	{
        foreach( BoardTile tile in Tile.connectedTiles)
		{
            if (tile == null)
                continue;

            if (tile.currentUnit)
			{
                // attack 

                Debug.Log("enemy found");
                Debug.Log(tile.currentUnit);
			}
		}
	}
}
