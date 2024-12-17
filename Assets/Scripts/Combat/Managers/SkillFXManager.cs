using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkillFXManager : MonoBehaviour
{
    public static SkillFXManager Instance;
    
	LineRenderer ProjectileLine;
    float projectileLineVertexCount = 12;
    // TODO still needs to animate the casting character and affected characters

    public void Init()
    {
        Instance = this;
		ProjectileLine = GetComponent<LineRenderer>();
	}

    public IEnumerator Cast(SO_SKillFX skillFx)
    {
        yield return new WaitForSeconds(skillFx.StartDelay);
        
        var skillObject = Instantiate(skillFx.SkillObject, skillFx.Origin, Quaternion.identity);

        if (skillFx.SkillFxKind == SkillFxType.Projectile)
        {
            float distance = Vector3.Distance(skillFx.Origin, skillFx.Destination);
            var time = 0f;
           
            while (distance > 0.1f)
            {
                time += Time.deltaTime;
                var projectileSpeed = skillFx.ProjectileSpeedCurve.Evaluate(time) * skillFx.ProjectileSpeed;
                skillObject.transform.position = Vector3.MoveTowards(skillObject.transform.position, skillFx.Destination, projectileSpeed * Time.deltaTime);

                distance = Vector3.Distance(skillObject.transform.position, skillFx.Destination);

                yield return null;
            }
        }

        if (skillFx.SkillFxKind == SkillFxType.Animation)
        {
            var particleSystem = skillObject.GetComponent<ParticleSystem>();
            yield return new WaitUntil(() => particleSystem.isStopped);
        }

        yield return new WaitForSeconds(skillFx.ExtendDelay);

        Destroy(skillObject);
        yield return new WaitForSeconds(skillFx.EndDelay);
    }

	public void PreviewProjectileLine(Vector3 casterPosition, Vector3 targetPosition, float offset)
	{
        var pointlist = new List<Vector3>();

        casterPosition += Vector3.up;
        targetPosition += Vector3.up;

        var heightOffset = Vector3.Distance(casterPosition, targetPosition);
        var middleOffset = Vector3.up * heightOffset * offset * 0.5f;

        Vector3 middlePosition = Vector3.Lerp(casterPosition, targetPosition, 0.5f) + (middleOffset);

        for (float ratio = 0; ratio <= 1; ratio += 1/projectileLineVertexCount)
		{
            var tangent1 = Vector3.Lerp(targetPosition, middlePosition, ratio);
            var tangent2 = Vector3.Lerp(middlePosition, casterPosition, ratio);
            var curve = Vector3.Lerp(tangent1, tangent2, ratio);

            pointlist.Add(curve);
        }

		ProjectileLine.positionCount = pointlist.Count;
        ProjectileLine.SetPositions(pointlist.ToArray());
	}

	public void EndProjectileLine()
	{
		ProjectileLine.positionCount = 0;
	}
}
