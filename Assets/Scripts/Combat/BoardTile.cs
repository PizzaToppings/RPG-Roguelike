using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardTile : MonoBehaviour
{
    public BoardTile[] connectedTiles = new BoardTile[6];

    public int xPosition = 0;
    public int yPosition = 0;

    public bool activated;

    // temp
    int index = 0;
    LineRenderer lineRenderer;


    void Start()
    {
        gameObject.name = xPosition + ", " + yPosition;

        StartCoroutine(SlideIn());
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
        activated = true;

        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = 12;
        var randomColor = Random.ColorHSV();
        lineRenderer.startColor = randomColor;
        lineRenderer.endColor = randomColor;

        // x1 (4, 12)
        // 
        // 0, +1
        // +1, +1
        // -1, 0
        // +1, 0
        //  0, -1
        //  1, -1

        // x2  (5, 12)
        // 
        // 0, +1
        // 1, +1
        // -1, 0
        // +1, 0
        //  0, -1
        //  1, -1

        // x3  (5, 11)
        // 
        // -1, +1
        // 0, +1
        // -1, 0
        // +1, 0
        //  -1, -1
        //  0, -1

        int unevenColumnOffset = 0;
        if (xPosition % 2 == 0)
        {
            unevenColumnOffset = -1;
        }

        

        
        BoardTile tile = BoardData.BoardTiles[xPosition + unevenColumnOffset, yPosition + 1];
        Debug.Log(tile);
        connectedTiles[index] = tile;

        lineRenderer.SetPosition(index * 2, new Vector3(tile.xPosition, 1, tile.yPosition));
        lineRenderer.SetPosition(index * 2 + 1, new Vector3(xPosition, 1, yPosition));
        index++;

        BoardTile tile2 = BoardData.BoardTiles[xPosition + unevenColumnOffset + 1, yPosition + 1];
        connectedTiles[index] = tile2;

        lineRenderer.SetPosition(index * 2, new Vector3(tile2.xPosition, 1, tile2.yPosition));
        lineRenderer.SetPosition(index * 2 + 1, new Vector3(xPosition, 1, yPosition));
        index++;

        BoardTile tile3 = BoardData.BoardTiles[xPosition + unevenColumnOffset - 1, yPosition];
        connectedTiles[index] = tile3;

        lineRenderer.SetPosition(index * 2, new Vector3(tile3.xPosition, 1, tile3.yPosition));
        lineRenderer.SetPosition(index * 2 + 1, new Vector3(xPosition, 1, yPosition));
        index++;

        BoardTile tile4 = BoardData.BoardTiles[xPosition + unevenColumnOffset + 1, yPosition];
        connectedTiles[index] = tile4;

        lineRenderer.SetPosition(index * 2, new Vector3(tile4.xPosition, 1, tile4.yPosition));
        lineRenderer.SetPosition(index * 2 + 1, new Vector3(xPosition, 1, yPosition));
        index++;

        BoardTile tile5 = BoardData.BoardTiles[xPosition + unevenColumnOffset, yPosition - 1];
        connectedTiles[index] = tile5;

        lineRenderer.SetPosition(index * 2, new Vector3(tile5.xPosition, 1, tile5.yPosition));
        lineRenderer.SetPosition(index * 2 + 1, new Vector3(xPosition, 1, yPosition));
        index++;

        BoardTile tile6 = BoardData.BoardTiles[xPosition + unevenColumnOffset + 1, yPosition - 1];
        connectedTiles[index] = tile6;

        lineRenderer.SetPosition(index * 2, new Vector3(tile6.xPosition, 1, tile6.yPosition));
        lineRenderer.SetPosition(index * 2 + 1, new Vector3(xPosition, 1, yPosition));
    }
}
