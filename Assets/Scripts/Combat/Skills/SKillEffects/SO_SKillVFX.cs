using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillFX", menuName = "ScriptableObjects/SkillEffects/SkillVFX")]
public class SO_SKillVFX : ScriptableObject
{ 
	public SkillFxType SkillFxKind;
	public bool MainTargetOnly;

	[Space]
	public SkillFxTargetEnum SkillOriginKind;
	public Vector3 SkillOriginOffset;
	public Vector3 SkillOriginRotation;

	[Space]
	public SkillFxTargetEnum SkillDestinationKind;
	public Vector3 SkillDestinationOffset;
	public Vector3 SkillDestinationRotation;

	[Space]
	public bool UseLineRenderer;
	public SkillFxTargetEnum LineRendererOriginKind;
	public SkillFxTargetEnum LineRendererDestinationKind;

	[Space]
	public bool StickToUnit;

	[Space]
	public AnimationCurve ProjectileSpeedCurve = AnimationCurve.Constant(0, 1, 1);
	public float ProjectileSpeed = 1;
	public float ProjectileOffset = 0;

	[Space]
	public float StartDelay = 0;
	public float ExtendDelay = 0;
	public float DisableObjectDelay = 0;
	public float EndDelay = 0;

	[Space]
	public bool ShowDamage = true;
	public float ShowDamageDelay = 0;

	[Space]
	public GameObject SkillObject;

	[HideInInspector] public List<Vector3> Origins = new List<Vector3>();
	[HideInInspector] public List<Vector3> Destinations = new List<Vector3>();

	[HideInInspector] public SkillPartData SPData;
	//private List<DamageData> damageDatas;
	private Unit caster;

	public void SetValues(SkillPartData spd, Unit caster)
    {
		CloneData(spd);
		this.caster = caster;

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

	void CloneData(SkillPartData spd)
    {
        SPData = new SkillPartData
        { 
			TilesHit = new List<BoardTile>(spd.TilesHit),
			TargetsHit = new List<Unit>(spd.TargetsHit),
			PartIndex = spd.PartIndex,
			GroupIndex = spd.GroupIndex,
			CanCast = spd.CanCast
		};
    }

    public List<Vector3> GetOrigins()
    {
		var origins = new List<Vector3>();

		if (SkillOriginKind == SkillFxTargetEnum.Caster)
		{
			origins.Add(caster.transform.position + Vector3.up + SkillOriginOffset);
		}

		if (SkillOriginKind == SkillFxTargetEnum.Target)
		{
			origins.AddRange(SPData.TargetsHit.Select(x => x.transform.position + Vector3.up + SkillOriginOffset).ToList());
		}

		if (SkillOriginKind == SkillFxTargetEnum.Tiles)
		{
			origins.AddRange(SPData.TilesHit.Select(x => x.transform.position + Vector3.up + SkillOriginOffset).ToList());
		}

		return origins;
	}

	public List<Vector3> GetDestinations()
    {
		var destinations = new List<Vector3>();

		if (SkillDestinationKind == SkillFxTargetEnum.Caster)
		{
			destinations.Add(caster.transform.position + Vector3.up + SkillOriginOffset);
		}

		if (SkillDestinationKind == SkillFxTargetEnum.Target)
		{
			destinations.AddRange(SPData.TargetsHit.Select(x => x.transform.position + Vector3.up + SkillOriginOffset).ToList());
		}

		if (SkillDestinationKind == SkillFxTargetEnum.Tiles)
		{
			destinations.AddRange(SPData.TilesHit.Select(x => x.transform.position + Vector3.up + SkillOriginOffset).ToList());
		}

		return destinations;
    }

	public Vector3 GetLineRendererTarget(SkillFxTargetEnum target, GameObject skillObject)
    {
		if (target == SkillFxTargetEnum.Caster)
			return caster.transform.position + Vector3.up;

		if (target == SkillFxTargetEnum.Target)
			return SPData.TargetsHit.Select(x => x.transform.position + Vector3.up + SkillOriginOffset).First();

		if (target == SkillFxTargetEnum.Tiles)
			return SPData.TilesHit.Select(x => x.transform.position + Vector3.up + SkillOriginOffset).First();

		// if target == skillObject
		return skillObject.transform.position;
	}
}
