using UnityEngine;

[CreateAssetMenu(fileName = "TileColorConfig", menuName = "Combat/Tile Color Config")]
public class TileColorConfig : ScriptableObject
{
    public TileColorPreset[] TileColors = new TileColorPreset[]
    {
        new TileColorPreset { Kind = TileColorKind.Original,      Color = Color.white                               },
        new TileColorPreset { Kind = TileColorKind.Move,          Color = new Color( 80/255f, 180/255f, 255/255f)   },
        new TileColorPreset { Kind = TileColorKind.MouseOver,     Color = new Color(255/255f, 255/255f, 255/255f)   },
        new TileColorPreset { Kind = TileColorKind.AllyTarget,    Color = new Color(180/255f, 220/255f, 255/255f)   },
        new TileColorPreset { Kind = TileColorKind.PlayerAttack,  Color = new Color(235/255f, 185/255f,  60/255f)   },
        new TileColorPreset { Kind = TileColorKind.EnemyThreat,   Color = new Color(210/255f, 100/255f,  80/255f)   },
        new TileColorPreset { Kind = TileColorKind.EnemyIntent,   Color = new Color(220/255f,  50/255f,  50/255f)   },
        new TileColorPreset { Kind = TileColorKind.Heal,          Color = new Color( 90/255f, 220/255f, 120/255f)   },
        new TileColorPreset { Kind = TileColorKind.Control,       Color = new Color(150/255f,  90/255f, 220/255f)   },
        new TileColorPreset { Kind = TileColorKind.FireTerrain,   Color = new Color(230/255f, 120/255f,  40/255f)   },
        new TileColorPreset { Kind = TileColorKind.IceTerrain,    Color = new Color(120/255f, 210/255f, 255/255f)   },
        new TileColorPreset { Kind = TileColorKind.PoisonTerrain, Color = new Color(140/255f, 190/255f,  80/255f)   },
    };

    public Color GetColor(TileColorKind kind)
    {
        foreach (var preset in TileColors)
        {
            if (preset.Kind == kind)
                return preset.Color;
        }
        return Color.white;
    }
}
