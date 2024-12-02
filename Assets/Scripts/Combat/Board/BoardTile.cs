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
    

    [HideInInspector] public Vector3 position = new Vector3();

    public void Init(int xPosition, int yPosition)
    {
        centerMaterial = gameObject.GetComponent<MeshRenderer>().materials[1];
        edgeMaterial = gameObject.GetComponent<MeshRenderer>().materials[0];

        boardManager = BoardManager.Instance;
        boardManager = BoardManager.Instance;

        position = transform.position;
        this.xPosition = xPosition;
        this.yPosition = yPosition;
        Coordinates = new Vector2Int(xPosition, yPosition);

        DistanceTraveled = Mathf.Infinity;
        DistanceToTarget = Mathf.Infinity;

        skillShotManager = SkillsManager.Instance;
        gameObject.name = xPosition + ", " + yPosition;
    }

    void OnMouseDown()
    {
        if (UnitData.CurrentActiveUnit.Friendly == false || EventSystem.current.IsPointerOverGameObject())
            return;

        if (movementLeft > -1 && UnitData.CurrentAction == CurrentActionKind.Basic)
        {
            // start moving
            MoveToTile();
        }
    }

    void OnMouseEnter()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
            Target();
    }

    public void MoveToTile() 
    {
        UnitData.CurrentAction = CurrentActionKind.Animating;
        boardManager.Path.Reverse();
        UnitData.CurrentActiveUnit.StartMoving(boardManager.Path);
    }

    public void Target()
    {
        if (UnitData.CurrentActiveUnit.Friendly == false)
            return;

		if (currentUnit != null && UnitData.CurrentAction == CurrentActionKind.Basic)
		{
			if (currentUnit is Enemy)
			{
				(currentUnit as Enemy).TargetEnemy();
			}
			return;
		}

		boardManager.currentMouseTile = this;

        if (currentUnit != null)
            currentUnit.IsTargeted = true;
        
        // Show movement line
        if (movementLeft > -1 
            && UnitData.CurrentAction == CurrentActionKind.Basic)
        {
            boardManager.Path = new List<BoardTile>();
			boardManager.PreviewMovementLine(this);
            SetColor(boardManager.MouseOverColor);
		}
    }

    public void TargetSkill()
	{

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

		if (UnitData.CurrentActiveUnit.Friendly && movementLeft > -1)
        {
            if (currentUnit != null)
                currentUnit.IsTargeted = false;
                
            boardManager.StopShowingMovement();
            OverrideColor(boardManager.MovementColor);
        }
    }

    public void SetConnectedTiles()
    {
        for (int i =0; i < connectedTiles.Length; i++)
		{
            var direction = boardManager.Directions[i];
            connectedTiles[i] = boardManager.GetBoardTile(Coordinates + direction);
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
