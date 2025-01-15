using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBaseAI : Enemy
{
    [HideInInspector] public BoardTile OptimalTile;
    [HideInInspector] public Unit Target;

    public float OptimalRange;
    public TargetPreferenceEnum TargetPreference;

    public override IEnumerator StartTurn()
    {
        yield return StartCoroutine(base.StartTurn());
    }

    public void FindOptimalTile()
    {
        boardManager.SetAOE(MoveSpeedLeft, Tile, null);

        var preferredTarget = GetTargetPreference(TargetPreference, UnitData.Characters);

        PossibleMovementTiles.Add(Tile);

        PossibleMovementTiles.ForEach(x => x.EnemyPreferenceRating = 0);

        foreach (var tile in PossibleMovementTiles)
        {
            foreach (var character in UnitData.Characters)
            {
                int preferedBonus = character == preferredTarget ? 50 : 0;

                var rangeToCharacter = boardManager.GetRangeBetweenTiles(tile, character.Tile);

                if (rangeToCharacter <= OptimalRange)
                    tile.EnemyPreferenceRating += 200 + preferedBonus;

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

    public Unit SetTarget()
    {
        List<Character> targetsInRange = new List<Character>();

        foreach (var character in UnitData.Characters)
        {
            var rangeToCharacter = boardManager.GetRangeBetweenTiles(Tile, character.Tile);

            if (rangeToCharacter <= OptimalRange)
                targetsInRange.Add(character);
        }

        var preferedTarget = GetTargetPreference(TargetPreference, targetsInRange);
        if (preferedTarget != null)
            return preferedTarget;
        else
            return targetsInRange.First();
    }

    public Unit GetTargetPreference(TargetPreferenceEnum targetPreference, List<Character> targetList)
    {
        switch (targetPreference)
        {
            case TargetPreferenceEnum.closestTarget:
                Character closestCharacter = null;
                float shortestDistance = float.MaxValue;

                foreach (var character in targetList)
                {
                    var rangeToCharacter = boardManager.GetRangeBetweenTiles(Tile, character.Tile);

                    if (rangeToCharacter < shortestDistance)
                    {
                        shortestDistance = rangeToCharacter;
                        closestCharacter = character;
                    }
                }
                return closestCharacter;

            case TargetPreferenceEnum.LowestHealthTarget:
                Character lowestHealthCharacter = UnitData.Characters.OrderByDescending(x => x.Hitpoints).First();
                return lowestHealthCharacter;
        }

        return null;
    }

    public IEnumerator MoveToTile()
    {
        boardManager.PreviewMovementLine(OptimalTile);
        boardManager.StopShowingMovement();
        yield return StartCoroutine(boardManager.MoveToTile());
    }
}
