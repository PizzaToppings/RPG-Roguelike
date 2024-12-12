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
    Material[] edgeMaterials;

    // movement
    public float movementLeft = -0.5f;
    public float DistanceTraveled;
    public float DistanceToTarget;

    // sillshot Info
    public List<float> skillshotsRangeLeft = new List<float>();

    public Unit currentUnit = null;

    public TileColor currentTileColor = new TileColor();
    public TileColor skillCastColor = new TileColor();


    [HideInInspector] public Vector3 position => transform.position;

    public void Init()
    {
        centerMaterial = gameObject.GetComponent<MeshRenderer>().materials[1];
        edgeMaterials[0] = gameObject.GetComponent<MeshRenderer>().materials[0];

        boardManager = BoardManager.Instance;
        boardManager = BoardManager.Instance;

        DistanceTraveled = Mathf.Infinity;
        DistanceToTarget = Mathf.Infinity;

        skillShotManager = SkillsManager.Instance;
    }

    void OnMouseDown()
    {
        OnClick();
    }

    void OnMouseEnter()
    {
        boardManager.currentMouseTile = this;

        if (!EventSystem.current.IsPointerOverGameObject() && movementLeft > -1)
            Target();
    }

    public void Target()
    {
        if (UnitData.CurrentActiveUnit.Friendly == false)
            return;

        if (currentUnit != null)
		{
            currentUnit.IsTargeted = true;

            if (currentUnit is Enemy)
			{
				(currentUnit as Enemy).TargetEnemy();
			}
			return;
		}

        // Show movement line
        boardManager.Path = new List<BoardTile>();
		boardManager.PreviewMovementLine(this);
        SetColor(boardManager.MouseOverColor);

        if (UnitData.CurrentAction == CurrentActionKind.CastingSkillshot && SkillData.CastOnTile)
		{
            SkillData.CurrentActiveSkill.Preview(this);
		}
    }

    void OnMouseExit()
    {
        UnTarget();
    }

    public void OnClick()
	{
        if (currentUnit == null)
		{
            if (UnitData.CurrentActiveUnit.Friendly == false || EventSystem.current.IsPointerOverGameObject())
                return;

            if (movementLeft > -1 && UnitData.CurrentAction == CurrentActionKind.Basic)
            {
                // start moving
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
        boardManager.currentMouseTile = null;

		if (currentUnit != null)
		{
            currentUnit.IsTargeted = false;

            if (currentUnit is Enemy)
			{
				(currentUnit as Enemy).UnTargetEnemy();

                if (UnitData.CurrentAction == CurrentActionKind.Basic && movementLeft >= 0)
                    OverrideColor(boardManager.MovementColor);
                else
                    OverrideColor(boardManager.originalColor);
            }
            return;
		}

		if (UnitData.CurrentActiveUnit.Friendly && movementLeft > -1
            && (UnitData.CurrentAction == CurrentActionKind.Basic || UnitData.CurrentAction == CurrentActionKind.CastingSkillshot))
        {
            boardManager.StopShowingMovement();

            if (UnitData.CurrentAction == CurrentActionKind.Basic)
            {
                if (movementLeft >= -0.5f)
                    OverrideColor(boardManager.MovementColor);
                else
                    OverrideColor(boardManager.originalColor);
            }

            var skill = SkillData.CurrentActiveSkill;

            if (UnitData.CurrentAction == CurrentActionKind.CastingSkillshot)
			{
                if (boardManager.GetRangeBetweenTiles(UnitData.CurrentActiveUnit.currentTile, this) <= skill.GetAttackRange())
                {
                    OverrideColor(skillCastColor);
                }
                else
                {
					OverrideColor(boardManager.originalColor);
				}
            }
        }
    }

    public void SetColor(TileColor color)
    {
        if (currentTileColor == null)
            currentTileColor = new TileColor();

        if (currentTileColor.Priority < color.Priority)
            return;

        Color transparentColor = color.Color;
        transparentColor.a = 0.5f;

        if (color.FillCenter)
        {
            centerMaterial.color = transparentColor;
        }

        SetEdgeColors(color, transparentColor);
        currentTileColor = color;

        if (UnitData.CurrentAction == CurrentActionKind.CastingSkillshot)
            skillCastColor = color;
    }

    public void OverrideColor(TileColor color)
    {
        if (currentTileColor == null)
            currentTileColor = new TileColor();

        Color transparentColor = color.Color;
        transparentColor.a = 0.5f;

        if (color.FillCenter)
        {
            centerMaterial.color = transparentColor;
        }

        SetEdgeColors(color, transparentColor);
        currentTileColor = color;

        if (UnitData.CurrentAction == CurrentActionKind.CastingSkillshot)
            skillCastColor = color;
    }

    public void SetEdgeColors(TileColor color, Color transparentColor)
    {
        var directions = new Vector2Int[]
        { new Vector2Int(0, 1), 
            new Vector2Int(1, 0),
            new Vector2Int(0, -1),
            new Vector2Int(-1, 0)
        };

        var edgeIndex = 0;
        for (int i = 0; i < connectedTiles.Length; i += 2)
        {
            edgeIndex++;
            if (connectedTiles[i].currentTileColor == currentTileColor)
            {
                edgeMaterials[edgeIndex].color = transparentColor;
            }
            else
            {
                edgeMaterials[edgeIndex].color = color.Color;
            }
        }
    }

    //   public void SetColor(TileColor color)
    //{
    //       if (currentCenterColor == null)
    //           currentCenterColor = new TileColor();

    //       if (currentEdgeColor == null)
    //           currentEdgeColor = new TileColor();

    //       if (color.UseCenterColor &&
    //               color.CenterPriority < currentCenterColor.CenterPriority)
    //	{
    //           centerMaterial.color = color.CenterColor;
    //           currentCenterColor = color;
    //	}

    //       if (color.UseEdgeColor &&
    //               color.EdgePriority < currentEdgeColor.EdgePriority)
    //       {
    //           edgeMaterial.color = color.EdgeColor;
    //           currentEdgeColor = color;
    //       }

    //       if (UnitData.CurrentAction == CurrentActionKind.CastingSkillshot)
    //	{
    //           skillCastColor = color;
    //       }
    //   }

    //   public void OverrideColor(TileColor color)
    //{
    //       if (color.UseCenterColor)
    //       {
    //           centerMaterial.color = color.CenterColor;
    //           currentCenterColor = color;
    //       }

    //       if (color.UseEdgeColor)
    //       {
    //           edgeMaterial.color = color.EdgeColor;
    //           currentEdgeColor = color;
    //       }

    //       if (UnitData.CurrentAction == CurrentActionKind.CastingSkillshot)
    //       {
    //           skillCastColor = color;
    //       }
    //   }
}
