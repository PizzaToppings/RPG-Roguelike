using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillFX", menuName = "ScriptableObjects/SkillFX")]
public class SO_SKillFX : ScriptableObject
{
	public SpellFxType SpellFxKind;
	public SpellFxOriginEnum SpellOriginKind;
	public SpellFxDestinationEnum SpellDestinationKind;

	[Space]
	public float ProjectileSpeed;

	[HideInInspector] public GameObject SpellObject;
	[HideInInspector] public Vector3 SpellOrgin;

	public void Trigger()
	{
		//StartCoroutine(PlayEffect);
	}

}
