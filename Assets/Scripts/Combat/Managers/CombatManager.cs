using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance;
    [SerializeField] BoardManager boardManager;
    [SerializeField] UnitManager unitManager;
    [SerializeField] SkillsManager skillsManager;
    [SerializeField] TargetSkillsManager targetSkillsManager;
    [SerializeField] DamageManager damageManager;
    [SerializeField] StatusEffectManager statusEffectManager;
    [SerializeField] SkillVFXManager skillVFXManager;
    [SerializeField] UIManager uiManager;
    [SerializeField] UI_Singletons ui_Singletons;
    [SerializeField] InputManager inputManager;
    [SerializeField] ConsumableManager consumableManager;

    [Space]
    [SerializeField] InitiativeTracker initiativeTracker;
    [SerializeField] CameraController cameraController;

    [Space]
    [SerializeField] float combatStartDelay = 1f;
    [SerializeField] float combatEndDelay = 2f;

    void Start()
    {
        Instance = this;

        UnitData.Reset();
        CombatData.Reset();
        BoardData.Reset();
        SkillData.Reset();

        CreateInstances();
        InitManagers();
        CreateBattlefield();
        boardManager.Clear();
		SetUnits();
		PreloadVFX();
		SetInitiative();
		RoundStart();
		StartCoroutine(DelayedTurnStart());
	}

    IEnumerator DelayedTurnStart()
    {
        yield return new WaitForSeconds(combatStartDelay);
        TurnStart();
    }

    void CreateInstances()
    {
        uiManager.CreateInstance();
        ui_Singletons.CreateInstance();
        boardManager.CreateInstance();
        unitManager.CreateInstance();
        statusEffectManager.CreateInstance();
        damageManager.CreateInstance();
        skillsManager.CreateInstance();
        targetSkillsManager.CreateInstance();
        skillVFXManager.CreateInstance();
        inputManager.CreateInstance();
        consumableManager.CreateInstance();
    }

    void InitManagers()
    {
        boardManager.Init();
		unitManager.Init();
		damageManager.Init();
		skillsManager.Init();
        targetSkillsManager.Init();
        skillVFXManager.Init();
		uiManager.Init();
        inputManager.Init();
        consumableManager.Init();
    }

    void CreateBattlefield()
    {
        boardManager.AddBoardTilesToList();
    }

    void SetUnits()
    {
        unitManager.PlaceUnits();
    }

    void PreloadVFX()
    {
        var allVFX = new List<SO_SKillVFX>();
        foreach (var unit in UnitData.Units)
            allVFX.AddRange(unit.GetSkillVFXList());
        skillVFXManager.PreloadVFX(allVFX);
    }

    public void SetInitiative()
    {
        UnitData.Units.Sort((x1, x2) =>
            x1.Initiative.CompareTo(x2.Initiative));

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
        CombatData.IsReady = true;

        if (CombatData.CurrentUnitTurn == UnitData.Units.Count)
        {
            CombatData.CurrentUnitTurn = 0;
            RoundStart();
        }

        var CurrentActiveUnit = UnitData.Units[CombatData.CurrentUnitTurn];

        initiativeTracker.NextTurn();

        UnitData.ActiveUnit = CurrentActiveUnit;

        StartCoroutine(cameraController.MoveToUnit(CurrentActiveUnit));
		StartCoroutine(CurrentActiveUnit.StartTurn());

        uiManager.StartTurn(CurrentActiveUnit);
    }

    public IEnumerator EndTurn()
    {
        if (CombatData.onTurnEnd != null)
            CombatData.onTurnEnd.Invoke();

        yield return new WaitForSeconds(1);
		CombatData.CurrentUnitTurn++;

        TurnStart();
    }

    public void Win()
	{
        StartCoroutine(WinCoroutine());
	}

    IEnumerator WinCoroutine()
    {
        yield return new WaitForSeconds(combatEndDelay);

        foreach (var character in UnitData.Characters)
        {
            if (character.partyMemberIndex < RunData.Party.Count)
                RunData.Party[character.partyMemberIndex].CurrentHitpoints = character.Hitpoints;
        }

        if (RunManager.Instance != null)
            RunManager.Instance.OnCombatWon();
        else
            Debug.Log("Combat won! (RunManager not present - testing mode)");
    }

    public void Lose()
	{
        StartCoroutine(LoseCoroutine());
	}

    IEnumerator LoseCoroutine()
    {
        yield return new WaitForSeconds(combatEndDelay);
        if (RunManager.Instance != null)
            RunManager.Instance.OnCombatLost();
        else
            Debug.Log("Combat lost! (RunManager not present - testing mode)");
    }
}
