using UnityEngine;

[CreateAssetMenu(fileName = "SkillFX", menuName = "ScriptableObjects/SkillEffects/SkillVFX")]
public class SO_SKillVFX : ScriptableObject
{
	// TODO change
	public SkillFxType SkillFxKind;
	public SkillFxOriginEnum SkillOriginKind;
	public SkillFxDestinationEnum SkillDestinationKind;

	[Space]
	public AnimationCurve ProjectileSpeedCurve = AnimationCurve.Constant(0, 1, 1);
	public float ProjectileSpeed = 1;
	public float ProjectileOffset = 0;

	[Space]
	public float StartDelay = 0;
	public float ExtendDelay = 0;
	public float EndDelay = 0;

	[Space]
	public bool ShowDamage = true;
	public float ShowDamageDelay = 0;

	[Space]
	public bool TriggerDisplacement;

	[Space]
	public GameObject SkillObject;

	[HideInInspector] public Vector3 Origin;
	[HideInInspector] public Vector3 Destination;

	private SkillPartData data;

	public void SetValues(SkillPartData spd)
    {
		data = spd;
		if (SkillFxKind == SkillFxType.Animation)
        {
			Origin = Destination = GetDestination();
        }

		if (SkillFxKind == SkillFxType.Projectile)
        {
			Origin = GetOrigin();
			Destination = GetDestination();
        }
    }

	public Vector3 GetOrigin()
    {
		var origin = new Vector3();

		if (SkillOriginKind == SkillFxOriginEnum.Caster)
		{
			origin = SkillData.Caster.transform.position;
		}

		if (SkillOriginKind == SkillFxOriginEnum.Target)
		{
			origin = data.TargetsHit[0].transform.position;
		}

		if (SkillOriginKind == SkillFxOriginEnum.Tiles)
		{
			origin = data.TilesHit[0].transform.position;
		}

		return origin + Vector3.up;
	}

	public Vector3 GetDestination()
    {
		var destination = new Vector3();

		if (SkillDestinationKind == SkillFxDestinationEnum.Caster)
		{
			destination = SkillData.Caster.transform.position;
		}

		if (SkillDestinationKind == SkillFxDestinationEnum.Target)
		{
			destination = data.TargetsHit[0].transform.position;
		}

		if (SkillDestinationKind == SkillFxDestinationEnum.Tiles)
		{
			destination = data.TilesHit[0].transform.position;
		}

		return destination + Vector3.up;
    }
}
