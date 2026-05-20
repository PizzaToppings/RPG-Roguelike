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
        // New turn order system: Characters go first, then enemies in configured order
        int characterCount = UnitData.Characters.Count;
        
        // Assign initiative to characters (they go first)
        for (int i = 0; i < UnitData.Characters.Count; i++)
        {
            UnitData.Characters[i].Initiative = i;
        }
        
        // Assign initiative to enemies (they go after all characters)
        for (int i = 0; i < UnitData.Enemies.Count; i++)
        {
            var enemy = UnitData.Enemies[i];
            // Characters get 0 to (characterCount-1), so enemies start at characterCount
            enemy.Initiative = characterCount + enemy.encounterTurnOrder;
        }

        // Sort all units by initiative
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
            if (CombatData.onRoundEnd != null)
                CombatData.onRoundEnd.Invoke();
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

        // Now start combat normally
        SetInitiative();
        RoundStart();
        StartCoroutine(DelayedTurnStart());
    }
}
