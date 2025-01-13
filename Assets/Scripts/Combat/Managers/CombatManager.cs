using System.Collections;
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

    void Start()
    {
        Instance = this;

        CreateInstances();
        InitManagers();
        CreateBattlefield();
		PlaceUnits();
		SetInitiative();
		RoundStart();
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

    void PlaceUnits()
    {
        unitManager.PlaceUnits();
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
        if (CombatData.currentCharacterTurn == UnitData.Units.Count)
        {
            CombatData.currentCharacterTurn = 0;
            RoundStart();
        }

        var CurrentActiveUnit = UnitData.Units[CombatData.currentCharacterTurn];

        StartCoroutine(cameraController.MoveToUnit(CurrentActiveUnit));

        // move cam to active unit

        initiativeTracker.NextTurn();

        UnitData.ActiveUnit = CurrentActiveUnit;
		StartCoroutine(CurrentActiveUnit.StartTurn());
		CombatData.currentCharacterTurn++;

        if (CurrentActiveUnit.Friendly && CurrentActiveUnit.Summon == false)
        {
            skillsManager.SetSkills(CurrentActiveUnit as Character);
            consumableManager.SetConsumables(CurrentActiveUnit as Character);
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
