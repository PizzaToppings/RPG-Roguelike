using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillFX", menuName = "ScriptableObjects/SkillEffects/SkillVFX")]
public class SO_SKillVFX : ScriptableObject
{ 
	public SkillFxType SkillFxKind;
	public bool MainTargetOnly;

	[Space]
	public SkillFxOriginEnum SkillOriginKind;
	public Vector3 SkillOriginOffset;
	public Vector3 SkillOriginRotation;

	[Space]
	public SkillFxDestinationEnum SkillDestinationKind;
	public Vector3 SkillDestinationOffset;
	public Vector3 SkillDestinationRotation;

	[Space]
	public bool StickToUnit;

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
	public GameObject SkillObject;

	[HideInInspector] public List<Vector3> Origins = new List<Vector3>();
	[HideInInspector] public List<Vector3> Destinations = new List<Vector3>();

	private SkillPartData SPData;
	private DamageData damageData;

	public void SetValues(SkillPartData spd, DamageData damageData)
    {
		SPData = spd;
		this.damageData = damageData;
		if (SkillFxKind == SkillFxType.Animation)
        {
			Origins = Destinations = GetDestinations();
        }

		if (SkillFxKind == SkillFxType.Projectile)
        {
			Origins = GetOrigins();
			Destinations = GetDestinations();
        }
    }

	public List<Vector3> GetOrigins()
    {
		var origins = new List<Vector3>();

		if (SkillOriginKind == SkillFxOriginEnum.Caster)
		{
			origins.Add(damageData.Caster.transform.position + Vector3.up + SkillOriginOffset);
		}

		if (SkillOriginKind == SkillFxOriginEnum.Target)
		{
			origins.AddRange(SPData.TargetsHit.Select(x => x.transform.position + Vector3.up + SkillOriginOffset).ToList());
		}

		if (SkillOriginKind == SkillFxOriginEnum.Tiles)
		{
			origins.AddRange(SPData.TilesHit.Select(x => x.transform.position + Vector3.up + SkillOriginOffset).ToList());
		}

		return origins;
	}

	public List<Vector3> GetDestinations()
    {
		var destinations = new List<Vector3>();

		if (SkillDestinationKind == SkillFxDestinationEnum.Caster)
		{
			destinations.Add(damageData.Caster.transform.position + Vector3.up + SkillOriginOffset);
		}

		if (SkillDestinationKind == SkillFxDestinationEnum.Target)
		{
			destinations.AddRange(SPData.TargetsHit.Select(x => x.transform.position + Vector3.up + SkillOriginOffset).ToList());
		}

		if (SkillDestinationKind == SkillFxDestinationEnum.Tiles)
		{
			destinations.AddRange(SPData.TilesHit.Select(x => x.transform.position + Vector3.up + SkillOriginOffset).ToList());
		}

		return destinations;
    }
}
