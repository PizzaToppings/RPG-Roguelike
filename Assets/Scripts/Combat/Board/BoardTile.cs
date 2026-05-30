using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class BoardTile : MonoBehaviour
{
    BoardManager boardManager;
    SkillsManager skillsManager;
    SkillVFXManager skillVFXManager;
    UI_Singletons ui_Singletons;

    public bool IsOpen = false;
    public bool IsClosed = false;

    public UnityEvent OnEnterTileEvent = new UnityEvent();
    public UnityEvent OnExitTileEvent = new UnityEvent();

    [HideInInspector] public bool IsBehindClosedTile = false;
    [HideInInspector] public bool IsBlocked => IsOpen || IsClosed;

    [HideInInspector] public BoardTile[] connectedTiles = new BoardTile[8];
    [HideInInspector] public BoardTile PreviousTile;

    // position
    [HideInInspector] public int xPosition = 0;
    [HideInInspector] public int yPosition = 0;
    [HideInInspector] public Vector2Int Coordinates;

    [HideInInspector] public Vector3Int CellPosition;

    // movement
    [HideInInspector] public float movementLeft = -1f;
    [HideInInspector] public float DistanceTraveled;
    [HideInInspector] public float DistanceToTarget;

    // sillshot Info
    [HideInInspector] public List<float> skillshotsRangeLeft = new List<float>();

    [HideInInspector] public Unit currentUnit = null;

    [HideInInspector] public TileColor currentTileColor = new TileColor();
    public bool hasTileEffect = false;
    public TileColor tileEffectColor = null;

    // Enemy AI
    [HideInInspector] public float EnemyPreferenceRating;

    [HideInInspector] public Vector3 position => transform.position;

    public void Init()
    {
        boardManager = BoardManager.Instance;
        ui_Singletons = UI_Singletons.Instance;

        DistanceTraveled = Mathf.Infinity;
        DistanceToTarget = Mathf.Infinity;

        skillsManager = SkillsManager.Instance;
        skillVFXManager = SkillVFXManager.Instance;
    }

    public void Target()
    {
        BoardData.CurrentMouseTile = this;

        if (currentUnit != null)
            currentUnit.Target();

        if (!CombatData.IsReady || UnitData.ActiveUnit.Friendly == false || UnitData.CurrentAction == CurrentActionKind.Animating)
            return;

        if (UnitData.CurrentAction == CurrentActionKind.Basic && movementLeft > -1  && currentUnit == null)
        {
            boardManager.Path = new List<BoardTile>();
            SetColor(boardManager.MouseOverColor);
            boardManager.PreviewMovementLine(this);
        }

        if (UnitData.CurrentAction == CurrentActionKind.CastingSkillshot && SkillData.CastOnTarget)
        {
            if (currentUnit == null)
                return;

            ((Character)UnitData.ActiveUnit).PreviewSkills(this);
        }

        if (UnitData.CurrentAction == CurrentActionKind.CastingSkillshot && SkillData.CastOnTile)
		{
            ((Character)UnitData.ActiveUnit).PreviewSkills(this);

            var attackRange = skillsManager.GetSkillAttackRange();
            if (attackRange == 0)
                return;
        }
    }

    public void OnClick()
	{
        if (!CombatData.IsReady)
            return;

        if (currentUnit == null)
		{
            if (UnitData.ActiveUnit.Friendly == false || EventSystem.current.IsPointerOverGameObject())
                return;

            if (movementLeft > -1 && UnitData.CurrentAction == CurrentActionKind.Basic)
            {
                StartCoroutine(boardManager.MoveToTile());
            }
        }
        else
		{
            currentUnit.OnClick();
        }
	}

    public void UnTarget()
    {
        boardManager.StopShowingMovementLine();

        if (currentUnit != null)
            currentUnit.Untarget();

        if (UnitData.CurrentAction == CurrentActionKind.Basic && movementLeft > -1)
		{
            if (currentUnit == null)
                OverrideColor(boardManager.MovementColor);
            
            if (hasTileEffect)
                SetColor(tileEffectColor);
        }
        else if (UnitData.CurrentAction == CurrentActionKind.Basic)
		{
            OverrideColor(boardManager.originalColor);
            
            if (hasTileEffect)
                SetColor(tileEffectColor);
        }
        else if (UnitData.CurrentAction == CurrentActionKind.CastingSkillshot)
        {
            boardManager.VisualClear();
            skillVFXManager.EndProjectileLine();
            SkillData.CurrentActiveSkill.Preview(null, UnitData.ActiveUnit);
            DisplacementPreviewManager.Instance?.ShowDisplacementPreviews(SkillData.CurrentActiveSkill);
        }
    }

    public void OnEnterTile(Unit unit)
	{
        currentUnit = unit;
        unit.Tile = this;

        if (OnEnterTileEvent != null)
            OnEnterTileEvent.Invoke();
	}

    public void OnExitTile()
    {
        currentUnit = null;

        if (OnExitTileEvent != null)
            OnExitTileEvent.Invoke();
    }

    public void SetColor(TileColor color)
    {
        if (currentTileColor == null)
            currentTileColor = new TileColor();

        if (currentTileColor.Priority < color.Priority)
            return;

        currentTileColor = color;
        ApplyHighlightColor(color);
    }

    public void OverrideColor(TileColor color)
    {
        currentTileColor = color;
        ApplyHighlightColor(color);
    }

    void ApplyHighlightColor(TileColor color)
    {
        // Original color means no highlight (transparent)
        if (color.Kind == TileColorKind.Original)
        {
            boardManager.SetHighlightColor(CellPosition, Color.clear);
            return;
        }
        
        float alpha = color.FillCenter ? 0.75f : 0.3f;
        Color c = new Color(color.Color.r, color.Color.g, color.Color.b, alpha);
        boardManager.SetHighlightColor(CellPosition, c);
    }
}
