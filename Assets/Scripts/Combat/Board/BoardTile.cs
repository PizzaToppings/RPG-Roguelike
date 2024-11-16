using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public Unit currentCharacter = null;

    public TileColor currentColor;
    

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

        //StartCoroutine(SlideIn());
    }

    void OnMouseDown()
    {
        if (UnitData.CurrentActiveUnit.Friendly == false)
            return;

        if (movementLeft > -1 && UnitData.CurrentAction == UnitData.CurrentActionKind.Moving)
        {
            // start moving
            MoveToTile();
        }
    }

    void OnMouseEnter()
    {
        Target();
    }

    public void MoveToTile() 
    {
        boardManager.Path.Reverse();
        UnitData.CurrentActiveUnit.StartMoving(boardManager.Path);
    }

    public void Target()
    {
        if (UnitData.CurrentActiveUnit.Friendly == false)
            return;

        boardManager.currentMouseTile = this;

        if (currentCharacter != null)
            currentCharacter.IsTargeted = true;
        
        // Show movement line
        if (movementLeft > -1 
            && UnitData.CurrentAction == UnitData.CurrentActionKind.Moving)
        {
            boardManager.Path = new List<BoardTile>();
			boardManager.PreviewMovementLine(this);
		}

    }

    void OnMouseExit()
    {
        UnTarget();
    }

    public void UnTarget()
    {
        boardManager.currentMouseTile = null;

        if (UnitData.CurrentActiveUnit.Friendly && movementLeft > -1)
        {
            if (currentCharacter != null)
                currentCharacter.IsTargeted = false;
                
            boardManager.StopShowingMovement();
        }
    }

    IEnumerator SlideIn()
    {
        bool up = Random.value > 0.5f;
        var upOrDown = 1;
        if (up)
            upOrDown = -1;
        
        var randomHeight = Random.Range(40 * upOrDown, 60 * upOrDown);
        var endPosition = transform.position;
        var startPosition = transform.position + Vector3.up * randomHeight;

        transform.position = startPosition;

        var randomTime = Random.Range(0.1f, 1.2f);
        yield return new WaitForSeconds(randomTime);

        var distanceLeft = 0f;
        var randomSpeed = Mathf.PerlinNoise(xPosition, yPosition);

        while (transform.position != endPosition)
        {
            distanceLeft += Time.deltaTime * randomSpeed;
            transform.position = Vector3.Lerp(startPosition, endPosition, distanceLeft);

            yield return new WaitForEndOfFrame();
        }
    }

    public void SetConnectedTiles()
    {
        // Top
        for (int i =0; i < connectedTiles.Length; i++)
		{
            var direction = boardManager.Directions[i];
            connectedTiles[i] = boardManager.GetBoardTile(Coordinates + direction);
        }
    }

    public void SetColor(TileColor color)
	{
		if (color.priority < currentColor.priority)
		{
            edgeMaterial.color = color.edgeColor;
            centerMaterial.color = color.centerColor;
            currentColor = color;
		}
	}

    public void OverrideColor(TileColor color)
	{
        edgeMaterial.color = color.edgeColor;
        centerMaterial.color = color.centerColor;
        currentColor = color;
	}
}
