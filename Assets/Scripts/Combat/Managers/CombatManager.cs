﻿using System.Collections;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance;
    [SerializeField] BoardManager boardManager;
    [SerializeField] UnitManager unitManager;
    [SerializeField] SkillsManager skillsManager;
    [SerializeField] DamageManager damageManager;
    [SerializeField] StatusEffectManager statusEffectManager;
    [SerializeField] SkillFXManager skillFXManager;
    [SerializeField] UIManager uiManager;

    [Space]
    [SerializeField] InitiativeTracker initiativeTracker;


    void Start()
    {
        Instance = this;

        CreateInstances();
        InitManagers();
        CreateBattlefield();
		PlaceUnits();
		CreateTurnOrder();
		SetInitiative();
		RoundStart();
		TurnStart();
	}

    void CreateInstances()
    {
        uiManager.CreateInstance();
        boardManager.CreateInstance();
        unitManager.CreateInstance();
        statusEffectManager.CreateInstance();
        damageManager.CreateInstance();
        skillsManager.CreateInstance();
        skillFXManager.CreateInstance();
    }

    void InitManagers()
    {
        boardManager.Init();
		unitManager.Init();
		damageManager.Init();
		skillsManager.Init();
        skillFXManager.Init();
		uiManager.Init();
	}

    void CreateBattlefield()
    {
        boardManager.AddBoardTilesToList();
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

        initiativeTracker.NextRound();

        if (CombatData.onRoundStart != null)
            CombatData.onRoundStart.Invoke();
    }

    public void TurnStart()
    {
        if (CombatData.currentCharacterTurn == UnitData.Units.Count)
        {
            CombatData.currentCharacterTurn = 0;
            RoundStart();
        }

        initiativeTracker.NextTurn();

        var CurrentActiveUnit = UnitData.Units[CombatData.currentCharacterTurn];
        UnitData.CurrentActiveUnit = CurrentActiveUnit;
		StartCoroutine(CurrentActiveUnit.StartTurn());
		CombatData.currentCharacterTurn++;

        if (CurrentActiveUnit.Friendly)
        {
            skillsManager.SetSkills(CurrentActiveUnit as Character);
        }

        uiManager.StartTurn(CurrentActiveUnit);
    }

    public IEnumerator EndTurn()
    {
        if (CombatData.onTurnEnd != null)
            CombatData.onTurnEnd.Invoke();
        yield return new WaitForSeconds(1);
        TurnStart();
    }
}
