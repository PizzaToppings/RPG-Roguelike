using UnityEngine;
using System.Collections;

public class SkillFXManager : MonoBehaviour
{
    public static SkillFXManager Instance;

	LineRenderer ProjectileLine;
	Vector3 projectileLineOffset = Vector3.up;
    // TODO still needs to animate the casting character and affected characters

    public void Init()
    {
        Instance = this;
		ProjectileLine = GetComponent<LineRenderer>();
	}

    public IEnumerator Cast(SO_SKillFX skillFx)
    {
        var skillObject = Instantiate(skillFx.SkillObject, skillFx.Origin, Quaternion.identity);

        if (skillFx.SkillFxKind == SkillFxType.Projectile)
        {
            float distance = Vector3.Distance(skillFx.Origin, skillFx.Destination);

            while (distance > 0.1f)
            {
                skillObject.transform.position = Vector3.MoveTowards(skillObject.transform.position, skillFx.Destination, skillFx.ProjectileSpeed * Time.deltaTime);

                distance = Vector3.Distance(skillObject.transform.position, skillFx.Destination);

                yield return null;
            }
        }

        if (skillFx.SkillFxKind == SkillFxType.Animation)
        {
            var particleSystem = skillObject.GetComponent<ParticleSystem>();
            yield return new WaitUntil(() => particleSystem.isStopped);
        }

        Destroy(skillObject);  // TODO cache this
        yield return new WaitForSeconds(0.1f);
    }

	public void PreviewProjectileLine(Vector3 CasterPosition, Vector3 TargetPosition)
	{
		ProjectileLine.positionCount = 2;

		ProjectileLine.SetPosition(0, TargetPosition + projectileLineOffset);
		ProjectileLine.SetPosition(1, CasterPosition + projectileLineOffset);
	}

	public void EndProjectileLine()
	{
		ProjectileLine.positionCount = 0;
	}
}
