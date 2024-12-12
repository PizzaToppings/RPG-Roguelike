using UnityEngine;

public class TileColortest : MonoBehaviour
{
    BoardTile tile;

    void Start()
    {
        tile = GetComponent<BoardTile>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
            tile.SetEdgeColor(Color.red, 0);// top

        if (Input.GetKeyDown(KeyCode.U))
            tile.SetEdgeColor(Color.red, 1);//Right

        if (Input.GetKeyDown(KeyCode.I))
            tile.SetEdgeColor(Color.red, 2); //bottom

        if (Input.GetKeyDown(KeyCode.O))
            tile.SetEdgeColor(Color.red, 3); // left

        if (Input.GetKeyDown(KeyCode.P))
            tile.SetCenterColor(Color.red); // center
    }
}
