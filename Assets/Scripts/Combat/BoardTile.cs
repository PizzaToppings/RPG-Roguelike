using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardTile : MonoBehaviour
{
    BoardManager boardManager;
    SkillShotManager skillShotManager;
    public BoardTile[] connectedTiles = new BoardTile[6];

    public int xPosition = 0;
    public int yPosition = 0;

    public int movementLeft = -1;

    // temp
    int index = 0;


    public void Init(int xPosition, int yPosition)
    {
        this.xPosition = xPosition;
        this.yPosition = yPosition;
        boardManager = BoardManager.boardManager;
        skillShotManager = SkillShotManager.skillShotManager;
        gameObject.name = xPosition + ", " + yPosition;

        StartCoroutine(SlideIn());
    }

    void OnMouseDown()
    {
        if (CombatData.CurrentActiveUnit.Friendly == false)
            return;

        if (movementLeft > -1 && UnitData.CurrentAction == UnitData.CurrentActionKind.Moving)
        {
            // start moving
            boardManager.Path.Reverse();
            CombatData.CurrentActiveUnit.StartMoving(boardManager.Path);
        }
    }

    void OnMouseEnter()
    {
        if (CombatData.CurrentActiveUnit.Friendly == false)
            return;
        
        // Start moving
        if (movementLeft > -1 
            && UnitData.CurrentAction == UnitData.CurrentActionKind.Moving)
        {
            boardManager.Path = new List<BoardTile>();
            boardManager.PreviewMovementLine(this, movementLeft);
        }

        if (UnitData.CurrentAction == UnitData.CurrentActionKind.CastingSkillshot)
            CombatData.CurrentActiveUnit.PreviewSkills(this);
    }

    void OnMouseExit()
    {
        if (CombatData.CurrentActiveUnit.Friendly && movementLeft > -1)
            boardManager.StopShowingMovement();
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
        int unevenColumnOffset = 1;
        if (yPosition % 2 != 0)
        {
            unevenColumnOffset = 0;
        }

        connectedTiles[0] = boardManager.GetBoardTile(xPosition + 1, yPosition);
        connectedTiles[1] = boardManager.GetBoardTile(xPosition + unevenColumnOffset, yPosition - 1);
        connectedTiles[2] = boardManager.GetBoardTile(xPosition - 1 + unevenColumnOffset, yPosition - 1);
        connectedTiles[3] = boardManager.GetBoardTile(xPosition - 1, yPosition);
        connectedTiles[4] = boardManager.GetBoardTile(xPosition - 1 + unevenColumnOffset, yPosition + 1);
        connectedTiles[5] = boardManager.GetBoardTile(xPosition + unevenColumnOffset, yPosition + 1);
    }

    int IncreaseIndex(int x, int y, int yOffset)
    {
        if (!(x == xPosition + yOffset && y != yPosition) &&
                    !(x == xPosition && y == yPosition))
                    return 1;

        return 0;
    }
}
