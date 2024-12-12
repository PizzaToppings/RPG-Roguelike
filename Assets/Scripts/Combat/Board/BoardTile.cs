using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BoardTile : MonoBehaviour
{
    BoardManager boardManager;
    SkillsManager skillsManager;
    SkillFXManager skillFXManager;
    UIManager uiManager;
    public BoardTile[] connectedTiles = new BoardTile[8];
    public BoardTile PreviousTile;

    // position
    public int xPosition = 0;
    public int yPosition = 0;
    public Vector2Int Coordinates;

    // materials
    Material centerMaterial;
    public Material[] edgeMaterials;

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
        edgeMaterials = new Material[4];

        centerMaterial = gameObject.GetComponent<MeshRenderer>().materials[4];
        edgeMaterials[0] = gameObject.GetComponent<MeshRenderer>().materials[1];//2, up
        edgeMaterials[1] = gameObject.GetComponent<MeshRenderer>().materials[2];//1, right
        edgeMaterials[2] = gameObject.GetComponent<MeshRenderer>().materials[3];//4, bottom
        edgeMaterials[3] = gameObject.GetComponent<MeshRenderer>().materials[0];//3, left

        boardManager = BoardManager.Instance;
        uiManager = UIManager.Instance;

        DistanceTraveled = Mathf.Infinity;
        DistanceToTarget = Mathf.Infinity;

        skillsManager = SkillsManager.Instance;
        skillFXManager = SkillFXManager.Instance;
    }

    void OnMouseDown()
    {
        OnClick();
    }

    void OnMouseEnter()
    {
        boardManager.currentMouseTile = this;

        if (!EventSystem.current.IsPointerOverGameObject())
            Target();
    }

    public void Target()
    {
        if (UnitData.CurrentActiveUnit.Friendly == false || UnitData.CurrentAction == CurrentActionKind.Animating)
            return;

        if (currentUnit != null && SkillData.CastOnTarget)
		{
            currentUnit.IsTargeted = true;

            if (currentUnit is Enemy)
			{
				(currentUnit as Enemy).TargetEnemy();
			}
			return;
		}

        if (UnitData.CurrentAction == CurrentActionKind.Basic && movementLeft > -1)
        {
            boardManager.Path = new List<BoardTile>();
            SetColor(boardManager.MouseOverColor);
            boardManager.PreviewMovementLine(this);
        }

        if (UnitData.CurrentAction == CurrentActionKind.CastingSkillshot && SkillData.CastOnTile)
		{
            UnitData.CurrentActiveUnit.PreviewSkills(this);

            var attackRange = skillsManager.GetSkillAttackRange();
            if (attackRange == 0)
                return;

            var tilesInAttackRange = boardManager.GetTilesInAttackRange(this, attackRange, true);
            if (tilesInAttackRange != null)
                TargetSkill(tilesInAttackRange, attackRange);
        }
    }

    void TargetSkill(List<BoardTile> tilesInAttackRange, float attackRange)
    {
        var closestTile = this;
        var skill = SkillData.CurrentActiveSkill;

        if (boardManager.GetTilesInAttackRange(this, attackRange, true).Any(x => x.currentUnit == UnitData.CurrentActiveUnit) == false)
        {
            closestTile = tilesInAttackRange.FirstOrDefault();
            closestTile.PreviewAttackWithinRange();
        }

        skill.SetTargetAndTile(currentUnit, this);

        uiManager.SetCursor(SkillData.CurrentActiveSkill.Cursor);

        if (attackRange > 1.5f) // so more than melee
            skillFXManager.PreviewProjectileLine(closestTile.transform.position, transform.position);
    }

    public void PreviewAttackWithinRange()
    {
        boardManager.Path = new List<BoardTile>();
        OverrideColor(boardManager.MouseOverColor);
        boardManager.PreviewMovementLine(this);

        // TODO set hologram from attacker
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
            }

            
            //return;
		}
        boardManager.StopShowingMovement();

        if (UnitData.CurrentAction == CurrentActionKind.Basic && movementLeft > -1)
            OverrideColor(boardManager.MovementColor);
        else if (UnitData.CurrentAction == CurrentActionKind.Basic && movementLeft < 0)
            OverrideColor(boardManager.originalColor);
        else if (UnitData.CurrentAction == CurrentActionKind.CastingSkillshot)
        {
            SkillData.Reset();
            boardManager.VisualClear();
            skillFXManager.EndProjectileLine();
            SkillData.CurrentActiveSkill.Preview(null);
        }
    }

    public void SetColor(TileColor color)
    {
        if (currentTileColor == null)
            currentTileColor = new TileColor();

        Color transparentColor = color.Color;
        transparentColor.a = 0.5f;

        if (color.FillCenter)
            centerMaterial.color = transparentColor;
        else
            centerMaterial.color = Color.clear;

        if (currentTileColor.Priority < color.Priority)
            return;
        
        currentTileColor = color;

        SetEdgeColors(color);

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
            centerMaterial.color = transparentColor;
        else
            centerMaterial.color = Color.clear;

        currentTileColor = color;

        SetEdgeColors(color);

        if (UnitData.CurrentAction == CurrentActionKind.CastingSkillshot)
            skillCastColor = color;
    }

    void SetEdgeColors(TileColor color)
    {
        Color transparentColor = color.Color;
        transparentColor.a = 0.2f;

        var edgeIndex = 0;
        for (int i = 0; i < connectedTiles.Length; i += 2)
        {
            if (connectedTiles[i] == null)
            {
                SetEdgeColor(color.Color, edgeIndex);
                edgeIndex++;
                continue;
            }

            var reverseEdgeIndex = (edgeIndex + 2) % 4;

            if (connectedTiles[i].currentTileColor == currentTileColor)
            {
                SetEdgeColor(transparentColor, edgeIndex);
                connectedTiles[i].SetEdgeColor(transparentColor, reverseEdgeIndex);
            }
            else if (currentTileColor.Priority < connectedTiles[i].currentTileColor.Priority)
            {
                SetEdgeColor(color.Color, edgeIndex);
                connectedTiles[i].SetEdgeColor(color.Color, reverseEdgeIndex);
            }
            else if (currentTileColor.Priority > connectedTiles[i].currentTileColor.Priority)
            {
                SetEdgeColor(connectedTiles[i].currentTileColor.Color, edgeIndex);
                connectedTiles[i].SetEdgeColor(connectedTiles[i].currentTileColor.Color, reverseEdgeIndex);
            }
         
            edgeIndex++;
        }
    }

    public void SetEdgeColor(Color color, int index)
    {
        edgeMaterials[index].color = color;
    }

    public void SetCenterColor(Color color)
    {
        centerMaterial.color = color;
    }
}
