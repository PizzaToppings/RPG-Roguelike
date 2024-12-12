using UnityEngine;

public class TileColorTester : MonoBehaviour
{
    BoardTile tile;


    void Start()
    {
        tile = GetComponent<BoardTile>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y)) // top 
            tile.SetEdgeColor(Color.green, 0);

        if (Input.GetKeyDown(KeyCode.U)) // right
            tile.SetEdgeColor(Color.blue, 1);

        if (Input.GetKeyDown(KeyCode.I)) // bottom
            tile.SetEdgeColor(Color.yellow, 2);

        if (Input.GetKeyDown(KeyCode.O)) // center
            tile.SetEdgeColor(Color.red, 3);

        if (Input.GetKeyDown(KeyCode.P)) // left
            tile.SetCenterColor(Color.black);
    }
}
