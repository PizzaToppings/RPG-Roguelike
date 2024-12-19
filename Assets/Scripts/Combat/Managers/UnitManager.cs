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

    public void CreateInstance()
    {
        Instance = this;
    }

    public void Init()
    {
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
        unit.currentTile = startingTile;
        startingTile.currentUnit = unit;
        unit.transform.position = startingTile.position;

        unit.Init();
    }
}
