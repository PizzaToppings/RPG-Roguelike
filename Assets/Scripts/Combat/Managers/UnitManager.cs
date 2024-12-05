using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance;

    //---------------------------

    [SerializeField] HealthCanvas healthCanvas;

    [Space]
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

        healthCanvas.Init();
    }

    void PlaceUnit(Transform child)
    {
        var unit = child.GetComponent<Unit>();
        UnitData.Units.Add(unit);
        
        var startingTile = BoardData.BoardTiles[unit.startXPosition, unit.startYPosition];
        unit.currentTile = startingTile;
        startingTile.currentUnit = unit;
        unit.transform.position = startingTile.position;

        unit.Init();

        if (unit.Friendly)
            UnitData.Characters.Add(unit as Character);
        else
            UnitData.Enemies.Add(unit as Enemy);
    }
}
