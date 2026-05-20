using UnityEngine;

public enum TileColorKind
{
    Original       = 0,
    Move           = 1,
    MouseOver      = 2,
    AllyTarget     = 3,
    PlayerAttack   = 4,
    EnemyThreat    = 5,
    EnemyIntent    = 6,
    Heal           = 7,
    Control        = 8,
    FireTerrain    = 9,
    IceTerrain     = 10,
    PoisonTerrain  = 11,
}

[System.Serializable]
public class TileColor
{
    public bool FillCenter = false;
    public TileColorKind Kind;
    public int Priority = 10;

    private static readonly Color[] PresetColors = new Color[]
    {
        new Color(1f,         1f,         1f,         1f), // Original  (white, will be overridden by originalColor)
        new Color( 80/255f,  180/255f,  255/255f,   1f), // Move           Cyan
        new Color(255/255f,  255/255f,  255/255f,   1f), // MouseOver      (white placeholder)
        new Color(180/255f,  220/255f,  255/255f,   1f), // AllyTarget     Pale blue
        new Color(235/255f,  185/255f,   60/255f,   1f), // PlayerAttack   Gold
        new Color(210/255f,  100/255f,   80/255f,   1f), // EnemyThreat    Muted orange-red
        new Color(220/255f,   50/255f,   50/255f,   1f), // EnemyIntent    Strong red
        new Color( 90/255f,  220/255f,  120/255f,   1f), // Heal           Green
        new Color(150/255f,   90/255f,  220/255f,   1f), // Control        Purple
        new Color(230/255f,  120/255f,   40/255f,   1f), // FireTerrain    Orange
        new Color(120/255f,  210/255f,  255/255f,   1f), // IceTerrain     Ice blue
        new Color(140/255f,  190/255f,   80/255f,   1f), // PoisonTerrain  Sickly green
    };

    public Color Color => PresetColors[(int)Kind];
}
