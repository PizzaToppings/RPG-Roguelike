using UnityEngine;

/// <summary>
/// Base class for all Anomalies.
/// Anomalies affect all characters, all enemies, all units, or the board state globally.
/// For complex behaviour create a custom SO_Anomaly subclass instead.
/// </summary>
public class SO_Anomaly : ScriptableObject
{
    public string AnomalyName;
    public Sprite Image;

    [Range(1, 4)]
    public int Rarity;

    [TextArea(5, 10)]
    public string Description;

    public virtual void Init(Anomaly anomaly)
    {
    }
}
