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
    }

    void PlaceUnit(Transform child)
    {
        var unit = child.GetComponent<Unit>();
        UnitData.Units.Add(unit);
        
        var startingTile = BoardData.BoardTiles[unit.startXPosition, unit.startYPosition];
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

        initiativeTracker.AddToInitiative(unit);
    }

    public IEnumerator RemoveUnit(Unit unit)
	{
        unit.gameObject.SetActive(false);
        unit.Tile.currentUnit = null;
        initiativeTracker.RemoveFromInitiative(unit);

        int deadCharacterIndex = UnitData.Units.IndexOf(unit);
       
        UnitData.Units.Remove(unit);

        if (deadCharacterIndex < CombatData.currentUnitTurn)
        {
            CombatData.currentUnitTurn--;
        }
        else if (deadCharacterIndex == CombatData.currentUnitTurn)
        {
            CombatData.currentUnitTurn--;
            yield return StartCoroutine(combatManager.EndTurn());
        }

        // remove unit from lists
        if (unit is Character)
            UnitData.Characters.Remove(unit as Character);

        if (unit is Enemy)
            UnitData.Enemies.Remove(unit as Enemy);

        if (UnitData.Characters.Count == 0)
            combatManager.Lose();

        if (UnitData.Enemies.Count == 0)
            combatManager.Win();

        yield return new WaitForSeconds(1);
        Destroy(unit.gameObject);
    }
}
