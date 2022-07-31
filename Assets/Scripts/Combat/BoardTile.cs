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


    void Start()
    {
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
            boardManager.ShowMovementLine(this, movementLeft);
        }

        if (UnitData.CurrentAction == UnitData.CurrentActionKind.CastingSkillshot)
            CombatData.CurrentActiveUnit.PreviewSkills();
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
        int unevenColumnOffset = -1;
        if (yPosition % 2 != 0)
        {
            unevenColumnOffset = 1;
        }

        for (int x = xPosition - 1; x <= xPosition + 1; x++)
        {
            for (int y = yPosition - 1; y <= yPosition + 1; y++)
            {
                if (AdjacentTile(x, y, unevenColumnOffset))
                {
                    BoardTile tile = BoardData.BoardTiles[x, y];
                    
                    if (tile != this)
                    {
                        connectedTiles[index] = tile;
                        index++;
                    }
                }
            }    
        }
    }

    bool AdjacentTile(int x, int y, int yOffset)
    {
        return (x >= 0 && x < BoardData.rowAmount &&
                    y >= 0 && y < BoardData.columnAmount &&
                    !(x == xPosition + yOffset && y != yPosition) &&
                    !(x == xPosition && y == yPosition));
    }
}
