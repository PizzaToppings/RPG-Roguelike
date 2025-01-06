using UnityEngine;

[CreateAssetMenu(fileName = "TileEffect", menuName = "ScriptableObjects/TileEffect")]
public class SO_TileEffect : ScriptableObject
{
	public string TileEffectName;
	public TileTriggerMomentEnum TriggerMoment;
	public TileTriggerEffectEnum TriggerEffect;

	[Space]
	public TileColor tileColor = new TileColor();

	[Space]
	public DamageData damageEffect;
	public int MaxDuration = 1;
}
