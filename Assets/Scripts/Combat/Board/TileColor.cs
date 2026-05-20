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

    public Color Color => BoardManager.Instance.TileColorConfig.GetColor(Kind);
}
