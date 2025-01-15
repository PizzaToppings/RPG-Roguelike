using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBaseAI : Enemy
{
    public float OptimalRange;
    [HideInInspector] public BoardTile OptimalTile;

    public override IEnumerator StartTurn()
    {
        yield return StartCoroutine(base.StartTurn());
    }

    public void FindOptimalTile()
    {
        boardManager.SetAOE(MoveSpeedLeft, Tile, null);

        foreach (var tile in PossibleMovementTiles)
        {
            foreach (var character in UnitData.Characters)
            {
                var rangeToCharacter = boardManager.GetRangeBetweenTiles(tile, character.Tile);

                if (rangeToCharacter <= OptimalRange)
                    tile.EnemyPreferenceRating += 200;

                float distanceModifier = 100;
                distanceModifier -= 5 * rangeToCharacter;
                if (distanceModifier < 0)
                    distanceModifier = 0;

                tile.EnemyPreferenceRating -= distanceModifier;
            }

            foreach (var enemy in UnitData.Enemies)
            {
                if (enemy == this)
                    continue;

                var rangeToEnemy = boardManager.GetRangeBetweenTiles(tile, enemy.Tile);

                if (rangeToEnemy <= 5) // TODO Edit this value based on something
                    tile.EnemyPreferenceRating += 50;
            }
        }

        OptimalTile = PossibleMovementTiles.OrderByDescending(x => x.EnemyPreferenceRating).First();
    }

    public IEnumerator MoveToTile()
    {
        boardManager.PreviewMovementLine(OptimalTile);
        boardManager.StopShowingMovement();
        yield return StartCoroutine(boardManager.MoveToTile());
    }
}
