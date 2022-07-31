using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    [SerializeField] BoardManager boardManager;
    [SerializeField] PlayerManager playerManager;
    [SerializeField] UnitManager unitManager;
    [SerializeField] SkillShotManager skillShotManager;

    [Space]
    [SerializeField] InitiativeTracker initiativeTracker;


    void Start()
    {
        CombatData.onTurnStart += TurnStart;
        CombatData.onTurnEnd += TurnEnd;

        InitManagers();
        CreateBattlefield();
        PlaceUnits();
        CreateTurnOrder();
        SetInitiative();
        RoundStart();
        TurnStart();
    }

    void InitManagers()
    {
        boardManager.Init();
        unitManager.Init();
        skillShotManager.Init();
    }

    void CreateBattlefield()
    {
        boardManager.CreateBoard();
    }

    void PlaceUnits()
    {
        unitManager.PlaceUnits();
    }

    void CreateTurnOrder()
    {
        UnitData.Units.Sort((x1, x2) =>
            x1.Initiative.CompareTo(x2.Initiative)
        );
    }

    void SetInitiative()
    {
        initiativeTracker.SetInitiative();
    }

    public void RoundStart()
    {
        CombatData.currentRound++;
        Debug.Log("starting new round: " + CombatData.currentRound);

        initiativeTracker.NextRound();

        if (CombatData.onRoundStart != null)
            CombatData.onRoundStart.Invoke();
    }

    public void TurnStart()
    {
        Debug.Log("starting new turn: " + CombatData.currentCharacterTurn);

        if (CombatData.currentCharacterTurn == UnitData.Units.Count)
        {
            CombatData.currentCharacterTurn = 0;
            RoundStart();
        }
        
        initiativeTracker.NextTurn();

        var CurrentActiveUnit = UnitData.Units[CombatData.currentCharacterTurn];
        CurrentActiveUnit.StartTurn();
        CombatData.CurrentActiveUnit = CurrentActiveUnit;
        CombatData.currentCharacterTurn++;
    }

    public void TurnEnd()
    {
        TurnStart();
    }
}
