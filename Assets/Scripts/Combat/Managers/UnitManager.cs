using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance;


    //---------------------------

    [SerializeField] Transform characterParent;
    [SerializeField] Transform enemyParent;

    //[HideInInspector] public List<Character> Characters;
    //[HideInInspector] public List<Enemy> Enemies;


    public void Init()
    {
        Instance = this;
    }

    public void PlaceUnits()
    {
        // TODO: Get from where?
        // characters ==> playerprefs
        // enemies ==> scriptable objects + prefabs


        // placeholder

        foreach (Transform child in characterParent)
        {
            PlaceUnit(child);
        }

        foreach (Transform child in enemyParent)
        {
            PlaceUnit(child);
        }
    }

    void PlaceUnit(Transform child)
    {
        var unit = child.GetComponent<Unit>();
        UnitData.Units.Add(unit);

        var randomX = Random.Range(0, BoardData.rowAmount);
        var randomY = Random.Range(0, BoardData.columnAmount);
        var randomPlace = BoardData.BoardTiles[randomX, randomY];
        unit.currentTile = randomPlace;
        randomPlace.currentUnit = unit;
        unit.transform.position = randomPlace.position;
        unit.Init();

        if (unit.Friendly)
            UnitData.Characters.Add(unit as Character);
        else
            UnitData.Enemies.Add(unit as Enemy);
    }

    public void ClearTargets()
    {
        foreach (var unit in UnitData.Units)
        {
            unit.IsTargeted = false;
        }
    }

    public void TargetUnit(Unit target)
    {
        if (target.IsTargeted)
        {
        }
    }
}
