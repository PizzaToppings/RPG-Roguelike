using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class InitiativeTracker : MonoBehaviour
{
    public static InitiativeTracker Instance;

    public GameObject InitiativeImagePrefab;

    List<InitiativeInformation> initiativeList = new List<InitiativeInformation>();

    InitiativeInformation currentHoveredInit;

    void Awake()
    {
        Instance = this;
    }

    public void SetInitiative()
    {
        initiativeList.Clear();

        for (int i = 0; i < UnitData.Characters.Count; i++)
        {
            var character = UnitData.Characters[i];
            AddToInitiative(character);
        }
        for (int i = 0; i < UnitData.Enemies.Count; i++)
        {
            var enemy = UnitData.Enemies[i];
            AddToInitiative(enemy);
        }

        SortAndRefresh();

        NextTurn();
    }

    public void AddToInitiative(Unit unit)
	{
        var image = Instantiate(InitiativeImagePrefab, transform);
        var init = image.GetComponent<InitiativeInformation>();
        init.Init(unit);

        initiativeList.Add(init);
        SortAndRefresh();
    }

    public void RemoveFromInitiative(Unit unit)
	{
        var init = initiativeList.First(x => x.thisUnit == unit);

        initiativeList.Remove(init);
        Destroy(init.gameObject);
        SortAndRefresh();
    }

    void SortAndRefresh()
    {
        initiativeList.Sort((x, y) => x.Initiative - y.Initiative);

        for (int i = 0; i < initiativeList.Count; i++)
        {
            initiativeList[i].transform.SetSiblingIndex(i);
            initiativeList[i].RefreshColor();
        }
    }

    public void NextTurn()
    {
        foreach (var initiative in initiativeList)
            initiative.SetActiveTurn(false);

        initiativeList[CombatData.CurrentUnitTurn].SetActiveTurn(true);
    }

    public void OnInitiativeHoverEnter(Unit unit)
    {
        if (unit == null) return;

        var highlighter = unit.GetComponent<UnitHighlighter>();
        if (highlighter != null)
            highlighter.SetHighlight(true);

        if (unit is Enemy enemy)
        {
            EnemyInfoPanelManager.Instance?.ShowPanel(enemy);
            BoardManager.Instance?.ShowEnemyThreatRange(enemy);
        }
        else if (unit is Character character)
        {
            CharacterInfoPanelManager.Instance?.ShowPanel(character);
        }
    }

    public void OnInitiativeHoverExit(Unit unit)
    {
        if (unit == null) return;

        var highlighter = unit.GetComponent<UnitHighlighter>();
        if (highlighter != null)
            highlighter.SetHighlight(false);

        if (unit is Enemy)
        {
            EnemyInfoPanelManager.Instance?.HidePanel();
            BoardManager.Instance?.ClearEnemyThreatRange();
        }
        else if (unit is Character)
        {
            CharacterInfoPanelManager.Instance?.HidePanel();
        }
    }

    public void NextRound()
    {
        //roundNumber.text = CombatData.currentRound.ToString();
    }
}
