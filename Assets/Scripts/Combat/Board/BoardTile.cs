using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BoardTile : MonoBehaviour
{
    BoardManager boardManager;
    SkillsManager skillShotManager;
    public BoardTile[] connectedTiles = new BoardTile[8];
    public BoardTile PreviousTile;

    // position
    public int xPosition = 0;
    public int yPosition = 0;
    public Vector2Int Coordinates;

    // materials
    Material centerMaterial;
    Material edgeMaterial;

    // movement
    public float movementLeft = -1;
    public float DistanceTraveled;
    public float DistanceToTarget;

    // sillshot Info
    public List<float> skillshotsRangeLeft = new List<float>();

    public Unit currentUnit = null;

    public TileColor currentCenterColor = new TileColor();
    public TileColor currentEdgeColor = new TileColor();
    

    [HideInInspector] public Vector3 position => transform.position;

    public void Init()
    {
        centerMaterial = gameObject.GetComponent<MeshRenderer>().materials[1];
        edgeMaterial = gameObject.GetComponent<MeshRenderer>().materials[0];

        boardManager = BoardManager.Instance;
        boardManager = BoardManager.Instance;

        DistanceTraveled = Mathf.Infinity;
        DistanceToTarget = Mathf.Infinity;

        skillShotManager = SkillsManager.Instance;
    }

    void OnMouseDown()
    {
        if (UnitData.CurrentActiveUnit.Friendly == false || EventSystem.current.IsPointerOverGameObject())
            return;

        if (movementLeft > -1 && UnitData.CurrentAction == CurrentActionKind.Basic)
        {
            // start moving
            StartCoroutine(boardManager.MoveToTile());
        }
    }

    void OnMouseEnter()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
            Target();
    }

    public void Target()
    {
        if (UnitData.CurrentActiveUnit.Friendly == false)
            return;

		if (currentUnit != null)
		{
            currentUnit.IsTargeted = true;

            if (currentUnit is Enemy && UnitData.CurrentAction == CurrentActionKind.Basic)
			{
				(currentUnit as Enemy).TargetEnemy();
			}
			return;
		}

		boardManager.currentMouseTile = this;
        
        // Show movement line
        if (movementLeft > -1 
            && UnitData.CurrentAction == CurrentActionKind.Basic)
        {
            boardManager.Path = new List<BoardTile>();
			boardManager.PreviewMovementLine(this);
            SetColor(boardManager.MouseOverColor);
		}
    }

    void OnMouseExit()
    {
        UnTarget();
    }

    public void UnTarget()
    {
        boardManager.currentMouseTile = null;

		if (currentUnit != null)
		{
            currentUnit.IsTargeted = false;

            if (currentUnit is Enemy)
			{
				(currentUnit as Enemy).UnTargetEnemy();

                if (movementLeft >= 0)
                    OverrideColor(boardManager.MovementColor);
                else
                    OverrideColor(boardManager.originalColor);
            }
            return;
		}

		if (UnitData.CurrentActiveUnit.Friendly && movementLeft > -1
            && UnitData.CurrentAction == CurrentActionKind.Basic)
        {
            boardManager.StopShowingMovement();
            OverrideColor(boardManager.MovementColor);
        }
    }

    public void SetColor(TileColor color)
	{
        if (currentCenterColor == null)
            currentCenterColor = new TileColor();

        if (currentEdgeColor == null)
            currentEdgeColor = new TileColor();

        if (color.UseCenterColor &&
                color.CenterPriority < currentCenterColor.CenterPriority)
		{
            centerMaterial.color = color.CenterColor;
            currentCenterColor = color;
		}

        if (color.UseEdgeColor &&
                color.EdgePriority < currentEdgeColor.EdgePriority)
        {
            edgeMaterial.color = color.EdgeColor;
            currentEdgeColor = color;
        }
    }

    public void OverrideColor(TileColor color)
	{
        if (color.UseCenterColor)
        {
            centerMaterial.color = color.CenterColor;
            currentCenterColor = color;
        }

        if (color.UseEdgeColor)
        {
            edgeMaterial.color = color.EdgeColor;
            currentEdgeColor = color;
        }
    }
}
