using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    [SerializeField] BoardManager boardManager; 

    // List<Unit> units = new List<Unit>(); 
    // List<Character> characters = new List<Character>(); 
    // List<Enemy> enemies = new List<Enemy>(); 

    //---------------------------

    public bool canMove;

    [SerializeField] GameObject playerPlaceholder;
    [SerializeField] GameObject enemyPlaceholder;
    void Update()
    {
        canMove = BoardData.canMove;
    }

    public void PlaceUnits()
    {
        // TODO: Get from where?
        // characters ==> playerprefs
        // enemies ==> scriptable objects + prefabs


        // placeholder

        for (int x = 0; x < 3; x++)
        {
            var randomX = Random.Range(0, BoardData.rowAmount);
            var randomY = Random.Range(0, BoardData.columnAmount);
            var randomPlace = BoardData.BoardTiles[randomX, randomY];
            var allyUnit = Instantiate(playerPlaceholder, randomPlace.transform.position + Vector3.up, Quaternion.identity);
            var ally = allyUnit.GetComponent<Character>();
            ally.currentTile = randomPlace;

            var ErandomX = Random.Range(0, BoardData.rowAmount);
            var ErandomY = Random.Range(0, BoardData.columnAmount);
            var ErandomPlace = BoardData.BoardTiles[ErandomX, ErandomY];
            var enemyUnit = Instantiate(enemyPlaceholder, ErandomPlace.transform.position + Vector3.up, Quaternion.identity);
            var enemy = enemyUnit.GetComponent<Enemy>();
            enemy.currentTile = ErandomPlace;

            UnitData.Units.Add(ally);
            UnitData.Units.Add(enemy);
            UnitData.Characters.Add(ally);
            UnitData.Enemies.Add(enemy);
        } 
    }
}
