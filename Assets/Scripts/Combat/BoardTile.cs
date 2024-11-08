using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardTile : MonoBehaviour
{
    BoardManager boardManager;
    SkillsManager skillShotManager;
    public BoardTile[] connectedTiles = new BoardTile[6];
    public BoardTile PreviousTile;

    public int xPosition = 0;
    public int yPosition = 0;
    public Vector2 Coordinates;

    public float movementLeft = -1;
    public float DistanceTraveled;
    public float DistanceToTarget;

    public Unit currentCharacter = null;

    [HideInInspector] public Vector3 position = new Vector3();

    public void Init(int xPosition, int yPosition)
    {
        boardManager = BoardManager.Instance;
        boardManager = BoardManager.Instance;

        position = transform.position;
        this.xPosition = xPosition;
        this.yPosition = yPosition;
        Coordinates = new Vector2(xPosition, yPosition);

        DistanceTraveled = Mathf.Infinity;
        DistanceToTarget = Mathf.Infinity;

        skillShotManager = SkillsManager.Instance;
        gameObject.name = xPosition + ", " + yPosition;

        StartCoroutine(SlideIn());
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

        if (currentCharacter != null)
            currentCharacter.IsTargeted = true;
        
        // Show movement line
        if (movementLeft > -1 
            && UnitData.CurrentAction == UnitData.CurrentActionKind.Moving)
        {
            boardManager.Path = new List<BoardTile>();
			boardManager.PreviewMovementLine(this);
		}

        if (UnitData.CurrentAction == UnitData.CurrentActionKind.CastingSkillshot)
            UnitData.CurrentActiveUnit.PreviewSkills(this);
    }

    void OnMouseExit()
    {
        UnTarget();
    }

    public void UnTarget()
    {
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
        // going in a circle:
        // Top
        connectedTiles[0] = boardManager.GetBoardTile(xPosition, yPosition + 1);
        // Top right
        connectedTiles[1] = boardManager.GetBoardTile(xPosition + 1, yPosition + 1);
        // Right
        connectedTiles[2] = boardManager.GetBoardTile(xPosition + 1, yPosition);
        // Bottom Right
        connectedTiles[3] = boardManager.GetBoardTile(xPosition + 1, yPosition - 1);
        // Bottom
        connectedTiles[4] = boardManager.GetBoardTile(xPosition, yPosition - 1);
        // Bottom left
        connectedTiles[5] = boardManager.GetBoardTile(xPosition - 1, yPosition - 1);
        // Left
        connectedTiles[6] = boardManager.GetBoardTile(xPosition - 1, yPosition);
        // Top Left
        connectedTiles[7] = boardManager.GetBoardTile(xPosition - 1, yPosition + 1);
    }

    int IncreaseIndex(int x, int y, int yOffset)
    {
        if (!(x == xPosition + yOffset && y != yPosition) &&
                    !(x == xPosition && y == yPosition))
                    return 1;

        return 0;
    }
}
