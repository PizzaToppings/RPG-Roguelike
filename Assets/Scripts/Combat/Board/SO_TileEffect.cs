using UnityEngine;

[CreateAssetMenu(fileName = "TileEffect", menuName = "ScriptableObjects/TileEffect")]
public class SO_TileEffect : ScriptableObject
{
	public string TileEffectName;
	public TileTriggerMomentEnum TriggerMoment;
	public TileTriggerEffectEnum TriggerEffect;

	[Space]
	public int MaxDuration = 1;
}
