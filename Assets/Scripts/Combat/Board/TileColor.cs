using UnityEngine;

[System.Serializable]
public class TileColor
{
    public bool UseCenterColor = false;
    public Color CenterColor;
    public int CenterPriority = 10;

    [Space]
    public bool UseEdgeColor = true;
    public Color EdgeColor = Color.white;
    public int EdgePriority = 10;
}
