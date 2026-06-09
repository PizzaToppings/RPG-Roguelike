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
    [SerializeField] TilemapInputHandler tilemapInputHandler;
    [SerializeField] CharacterPlacementManager characterPlacementManager;
    [SerializeField] DisplacementPreviewManager displacementPreviewManager;

    [Space]
    [SerializeField] InitiativeTracker initiativeTracker;
    [SerializeField] CameraController cameraController;

    [Space]
    [SerializeField] float combatStartDelay = 1f;
    [SerializeField] float combatEndDelay = 2f;

    bool placementPhaseEnabled = false;

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
        StartPlacementPhase();
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
        tilemapInputHandler.CreateInstance();
        
        if (displacementPreviewManager != null)
            displacementPreviewManager.CreateInstance();
        
        if (characterPlacementManager != null)
            characterPlacementManager.CreateInstance();
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
        tilemapInputHandler.Init();
        
        if (characterPlacementManager != null)
            characterPlacementManager.Init();
    }

    void CreateBattlefield()
    {
        boardManager.AddBoardTilesToList();
        boardManager.InitHighlightTilemap();
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
        // Build the per-round turn sequence from the encounter TurnOrder (if configured)
        CombatData.TurnSequence.Clear();

        var encounter = RunData.CurrentEncounter;
        if (encounter != null && encounter.TurnOrder != null && encounter.TurnOrder.Count > 0)
        {
            // Interpret each TurnOrder entry as a turn slot and map to a Unit instance.
            foreach (var slot in encounter.TurnOrder)
            {
                if (slot == null) continue;
                if (slot.UnitType == UnitTurnEnum.Character)
                {
                    if (slot.UnitIndex >= 0 && slot.UnitIndex < UnitData.Characters.Count)
                        CombatData.TurnSequence.Add(UnitData.Characters[slot.UnitIndex]);
                }
                else // Enemy
                {
                    if (slot.UnitIndex >= 0 && slot.UnitIndex < UnitData.Enemies.Count)
                        CombatData.TurnSequence.Add(UnitData.Enemies[slot.UnitIndex]);
                }
            }
        }
        else
        {
            // Default: all characters first (in list order), then enemies in list order
            foreach (var c in UnitData.Characters)
                CombatData.TurnSequence.Add(c);
            foreach (var e in UnitData.Enemies)
                CombatData.TurnSequence.Add(e);
        }

        // As a convenience for other UI, set each Unit.Initiative to the first slot index where it appears
        for (int i = 0; i < CombatData.TurnSequence.Count; i++)
        {
            var u = CombatData.TurnSequence[i];
            if (u != null)
            {
                // only set if not already assigned to an earlier slot
                if (u.Initiative == 0 && !UnitData.Units.Contains(u))
                    u.Initiative = i;
                else
                    u.Initiative = Mathf.Min(u.Initiative, i);
            }
        }

        // Ensure CurrentUnitTurn reset to 0 for new round setup
        CombatData.CurrentUnitTurn = 0;

        // Refresh UI
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

        if (CombatData.CurrentUnitTurn >= CombatData.TurnSequence.Count)
        {
            CombatData.CurrentUnitTurn = 0;
            if (CombatData.onRoundEnd != null)
                CombatData.onRoundEnd.Invoke();
            RoundStart();
        }

        Unit CurrentActiveUnit = null;
        if (CombatData.CurrentUnitTurn < CombatData.TurnSequence.Count)
            CurrentActiveUnit = CombatData.TurnSequence[CombatData.CurrentUnitTurn];

        initiativeTracker.NextTurn();

        UnitData.ActiveUnit = CurrentActiveUnit;

        // Set the global action state depending on the active unit kind.
        // If an enemy is active, enter EnemyTurn so systems can adjust input/visuals.
        if (CurrentActiveUnit != null && CurrentActiveUnit.Friendly == false)
            UnitData.CurrentAction = CurrentActionKind.EnemyTurn;
        else
            UnitData.CurrentAction = CurrentActionKind.Basic;

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

        CombatData.onCombatEnd.Invoke();

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

    /// <summary>
    /// Starts the character placement phase.
    /// </summary>
    void StartPlacementPhase()
    {
        if (characterPlacementManager == null)
        {
            Debug.LogError("[CombatManager] CharacterPlacementManager not assigned!");
            // Fallback to normal combat start
            SetInitiative();
            RoundStart();
            StartCoroutine(DelayedTurnStart());
            return;
        }

        placementPhaseEnabled = true;

        // Enter placement mode: disable normal actions and enable placement interactions
        UnitData.ActiveUnit = null;
        UnitData.CurrentAction = CurrentActionKind.CharacterPlacement;

        // Build initiative order and populate the tracker before placement starts
        SetInitiative();

        // If none configured, use all board tiles
        List<Vector2Int> placementTiles;
        
        if (RunData.CurrentEncounter.PlayerPlacementTiles != null && 
            RunData.CurrentEncounter.PlayerPlacementTiles.Count > 0)
        {
            placementTiles = RunData.CurrentEncounter.PlayerPlacementTiles;
        }
        else
        {
            // Use all board tiles
            placementTiles = GetAllBoardTiles();
        }

        // Subscribe to placement confirmation event
        characterPlacementManager.OnPlacementConfirmed.AddListener(OnPlacementConfirmed);

        // Start the placement phase
        characterPlacementManager.StartPlacementPhase(UnitData.Characters, placementTiles);
    }

    /// <summary>
    /// Gets all board tiles as placement options.
    /// </summary>
    List<Vector2Int> GetAllBoardTiles()
    {
        var allTiles = new List<Vector2Int>();
        
        if (BoardData.BoardTiles != null)
        {
            for (int x = 0; x < BoardData.rowAmount; x++)
            {
                for (int y = 0; y < BoardData.columnAmount; y++)
                {
                    if (BoardData.BoardTiles[x, y] != null)
                    {
                        allTiles.Add(new Vector2Int(x, y));
                    }
                }
            }
        }
        
        return allTiles;
    }

    /// <summary>
    /// Called when the player confirms their character placement.
    /// </summary>
    void OnPlacementConfirmed()
    {
        placementPhaseEnabled = false;

        // Unsubscribe from the event
        if (characterPlacementManager != null)
            characterPlacementManager.OnPlacementConfirmed.RemoveListener(OnPlacementConfirmed);

        // Reset input state and start combat normally
        UnitData.CurrentAction = CurrentActionKind.Basic;

        RoundStart();
        StartCoroutine(DelayedTurnStart());
    }

}
