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
        // Build initiative UI from the current turn sequence
        // Clear existing
        foreach (var init in initiativeList)
            Destroy(init.gameObject);
        initiativeList.Clear();

        for (int i = 0; i < CombatData.TurnSequence.Count; i++)
        {
            var unit = CombatData.TurnSequence[i];
            if (unit == null) continue;
            var image = Instantiate(InitiativeImagePrefab, transform);
            var init = image.GetComponent<InitiativeInformation>();
            init.Init(unit);
            init.Initiative = i;
            initiativeList.Add(init);
        }

        SortAndRefresh();

        if (initiativeList.Count > 0)
            NextTurn();
    }

    public void AddToInitiative(Unit unit)
	{
        // Not used in the new slot-based system. Rebuild full UI instead.
        SetInitiative();
    }

    public void RemoveFromInitiative(Unit unit)
	{
        // Remove all slots referencing this unit from the turn sequence and rebuild UI
        if (CombatData.TurnSequence.Contains(unit))
        {
            CombatData.TurnSequence.RemoveAll(u => u == unit);
            // Ensure CurrentUnitTurn stays in bounds
            if (CombatData.CurrentUnitTurn >= CombatData.TurnSequence.Count)
                CombatData.CurrentUnitTurn = Mathf.Max(0, CombatData.TurnSequence.Count - 1);
            SetInitiative();
        }
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

        if (initiativeList.Count == 0) return;
        int idx = Mathf.Clamp(CombatData.CurrentUnitTurn, 0, initiativeList.Count - 1);
        initiativeList[idx].SetActiveTurn(true);
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
