using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public static UnitManager unitManager;

    // List<Unit> units = new List<Unit>(); 
    // List<Character> characters = new List<Character>(); 
    // List<Enemy> enemies = new List<Enemy>(); 

    //---------------------------

    [SerializeField] Transform characterParent;
    [SerializeField] Transform enemyParent;

    public void Init()
    {
        unitManager = this;
    }

    void Update()
    {
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

        // foreach (var unit in UnitData.Units)
        // {
        //     var randomX = Random.Range(0, BoardData.rowAmount);
        //     var randomY = Random.Range(0, BoardData.columnAmount);
        //     var randomPlace = BoardData.BoardTiles[randomX, randomY];
        //     // var allyUnit = Instantiate(playerPlaceholder, randomPlace.transform.position + Vector3.up, Quaternion.identity);
        //     // var ally = allyUnit.GetComponent<Character>();
        //     unit.currentTile = randomPlace;
        //     unit.transform.position =randomPlace.transform.position + Vector3.up;
        //     unit.Init();

        //     // var ErandomX = Random.Range(0, BoardData.rowAmount);
        //     // var ErandomY = Random.Range(0, BoardData.columnAmount);
        //     // var ErandomPlace = BoardData.BoardTiles[ErandomX, ErandomY];
        //     // var enemyUnit = Instantiate(enemyPlaceholder, ErandomPlace.transform.position + Vector3.up, Quaternion.identity);
        //     // var enemy = enemyUnit.GetComponent<Enemy>();
        //     // enemy.currentTile = ErandomPlace;
        // } 
    }

    void PlaceUnit(Transform child)
    {
        var unit = child.GetComponent<Unit>();
        UnitData.Units.Add(unit);

        if (unit.Friendly)
            UnitData.Characters.Add(unit as Character);
        else
            UnitData.Enemies.Add(unit as Enemy);

        var randomX = Random.Range(0, BoardData.rowAmount);
        var randomY = Random.Range(0, BoardData.columnAmount);
        var randomPlace = BoardData.BoardTiles[randomX, randomY];
        unit.currentTile = randomPlace;
        unit.transform.position =randomPlace.transform.position + Vector3.up;
        unit.Init();
    }
}
