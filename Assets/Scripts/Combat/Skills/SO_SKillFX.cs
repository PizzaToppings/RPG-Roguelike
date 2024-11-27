using UnityEngine;using System.Collections;

[CreateAssetMenu(fileName = "SkillFX", menuName = "ScriptableObjects/SkillFX")]
public class SO_SKillFX : ScriptableObject
{
	public SkillFxType SkillFxKind;
	public SkillFxOriginEnum SkillOriginKind;
	public SkillFxDestinationEnum SkillDestinationKind;

	[Space]
	public float ProjectileSpeed;

	[HideInInspector] public GameObject SkillObject;
	[HideInInspector] public Vector3 Origin;
	[HideInInspector] public Vector3 Destination;

	private SkillPartData data;

	public void Trigger(SkillPartData spd)
	{
		data = spd;
		GetValues();
	}

	public void GetValues()
    {
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

		return origin;
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

		return destination;
    }
}
