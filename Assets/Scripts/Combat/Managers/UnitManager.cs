using System.Collections;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance;

    [SerializeField] InitiativeTracker initiativeTracker;

    CombatManager combatManager;

    [SerializeField] HealthCanvas healthCanvas;

    [Space]
    [SerializeField] Transform characterParent;
    [SerializeField] Transform enemyParent;

    public void CreateInstance()
    {
        Instance = this;
    }

    public void Init()
    {
        combatManager = CombatManager.Instance;

        foreach (Transform child in characterParent)
        {
            var unit = child.GetComponent<Character>();
            UnitData.Characters.Add(unit);
        }

        foreach (Transform child in enemyParent)
        {
            var unit = child.GetComponent<Enemy>();
            UnitData.Enemies.Add(unit);
        }
    }

    public void PlaceUnits()
    {
        foreach (Transform child in characterParent)
        {
            PlaceUnit(child);
        }

        foreach (Transform child in enemyParent)
        {
            PlaceUnit(child);
        }

        healthCanvas.Init();

        foreach (Transform child in characterParent)
        {
            var character = child.GetComponent<Character>();
            if (character != null)
                character.InitTraits();
        }
    }

    void PlaceUnit(Transform child)
    {
        var unit = child.GetComponent<Unit>();
        UnitData.Units.Add(unit);

        int rows = BoardData.BoardTiles.GetLength(0);
        int cols = BoardData.BoardTiles.GetLength(1);

        if (unit.startXPosition < 0 || unit.startXPosition >= rows ||
            unit.startYPosition < 0 || unit.startYPosition >= cols)
        {
            Debug.LogError($"[UnitManager] '{child.name}' has startXPosition={unit.startXPosition}, startYPosition={unit.startYPosition} " +
                           $"but the board array is {rows}x{cols}. Set values in range [0,{rows - 1}] x [0,{cols - 1}].");
            return;
        }

        var startingTile = BoardData.BoardTiles[unit.startXPosition, unit.startYPosition];
        if (startingTile == null)
        {
            Debug.LogError($"[UnitManager] '{child.name}' start tile [{unit.startXPosition},{unit.startYPosition}] exists in the array but has no BoardTile (hole in the map).");
            return;
        }

        unit.Tile = startingTile;
        startingTile.currentUnit = unit;
        unit.transform.position = startingTile.position;

        unit.Init();
    }

    public void SummonUnit(GameObject unitPrefab, BoardTile tile, bool friendly)
	{
        var parent = friendly ? characterParent : enemyParent;
        var summonedUnit = Instantiate(unitPrefab, tile.position, Quaternion.identity, parent);
        
        var unit = summonedUnit.GetComponent<Unit>();
        UnitData.Units.Add(unit);

        if (friendly)
            UnitData.Characters.Add(unit as Character);
        else
            UnitData.Enemies.Add(unit as Enemy);

        unit.Tile = tile;
        tile.currentUnit = unit;

        unit.Init();

        int insertIndex = UnitData.Units.Count - 1; 
        UnitData.Units.RemoveAt(UnitData.Units.Count - 1);
        insertIndex = 0;
        while (insertIndex < UnitData.Units.Count && UnitData.Units[insertIndex].Initiative <= unit.Initiative)
            insertIndex++;
        UnitData.Units.Insert(insertIndex, unit);

        if (insertIndex <= CombatData.CurrentUnitTurn)
            CombatData.CurrentUnitTurn++;

        initiativeTracker.AddToInitiative(unit);
    }

    public IEnumerator RemoveUnit(Unit unit)
	{
        unit.OnDeathEvent.Invoke();

        unit.gameObject.SetActive(false);
        unit.Tile.currentUnit = null;
        initiativeTracker.RemoveFromInitiative(unit);

        int deadCharacterIndex = UnitData.Units.IndexOf(unit);
       
        UnitData.Units.Remove(unit);

        // remove unit from sub-lists before any turn logic runs
        if (unit is Character)
            UnitData.Characters.Remove(unit as Character);

        if (unit is Enemy)
            UnitData.Enemies.Remove(unit as Enemy);

        if (UnitData.Characters.Count == 0)
            combatManager.Lose();

        if (UnitData.Enemies.Count == 0)
            combatManager.Win();

        if (deadCharacterIndex < CombatData.CurrentUnitTurn)
        {
            CombatData.CurrentUnitTurn--;
        }
        else if (deadCharacterIndex == CombatData.CurrentUnitTurn)
        {
            CombatData.CurrentUnitTurn--;
            yield return StartCoroutine(combatManager.EndTurn());
        }

        yield return new WaitForSeconds(1);
        Destroy(unit.gameObject);
    }
}
