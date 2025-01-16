using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkillVFXManager : MonoBehaviour
{
    public static SkillVFXManager Instance;

    [SerializeField] Transform skillObjectParent;

    LineRenderer ProjectileLine;
    float projectileLineVertexCount = 12;

    public void CreateInstance()
    {
        Instance = this;
    }

    public void Init()
    {
		ProjectileLine = GetComponent<LineRenderer>();
	}

    public IEnumerator Cast(SO_SKillVFX skillVFX)
    {
        yield return new WaitForSeconds(skillVFX.StartDelay);

        var skillObject = GetOrCreateSpellObject(skillVFX);
        
        skillObject.transform.position = skillVFX.Origin;
        skillObject.transform.rotation = Quaternion.Euler(skillVFX.SkillOriginRotation);

        if (skillVFX.SkillFxKind == SkillFxType.Projectile)
        {
            yield return StartCoroutine(MoveProjectileAlongCurve(skillObject, skillVFX));
        }

        if (skillVFX.SkillFxKind == SkillFxType.Animation)
        {
            var particleSystem = skillObject.GetComponent<ParticleSystem>();

            if (skillVFX.StickToUnit)
            {
                while (true)
                {
                    skillObject.transform.position = skillVFX.GetDestination();

                    if (particleSystem.isStopped)
                        break;

                    yield return null;
                }
            }

            yield return new WaitUntil(() => particleSystem.isStopped);
        }

        yield return new WaitForSeconds(skillVFX.ExtendDelay);

        skillObject.SetActive(false);
        yield return new WaitForSeconds(skillVFX.EndDelay);
    }

    GameObject GetOrCreateSpellObject(SO_SKillVFX skillVFX)
	{
        if (skillObjectParent.childCount > 0)
        {
            foreach (Transform childTransform in skillObjectParent.transform)
            {
                var child = childTransform.gameObject;

                if (child == null)
                    continue;

                if (child.activeSelf)
                    continue;

                if (child.name == skillVFX.SkillObject.name + "(Clone)")
                {
                    child.transform.position = skillVFX.Origin;
                    child.SetActive(true);
                    return child;
                }
            }
        }
        return Instantiate(skillVFX.SkillObject, skillVFX.Origin, Quaternion.identity, skillObjectParent);
    }

    private IEnumerator MoveProjectileAlongCurve(GameObject skillObject, SO_SKillVFX skillFx)
    {
        var pointList = new List<Vector3>();
        Vector3 casterPosition = skillFx.Origin + Vector3.up;
        Vector3 targetPosition = skillFx.Destination + Vector3.up;

        var heightOffset = Vector3.Distance(casterPosition, targetPosition);
        var middleOffset = Vector3.up * heightOffset * skillFx.ProjectileOffset * 0.5f;

        Vector3 middlePosition = Vector3.Lerp(casterPosition, targetPosition, 0.5f) + middleOffset;

        for (float ratio = 0; ratio <= 1; ratio += 1f / projectileLineVertexCount)
        {
            var tangent1 = Vector3.Lerp(targetPosition, middlePosition, ratio);
            var tangent2 = Vector3.Lerp(middlePosition, casterPosition, ratio);
            var curve = Vector3.Lerp(tangent1, tangent2, ratio);
            pointList.Add(curve);
        }
        pointList.Reverse();

        float time = 0f;
        int currentIndex = 0;

        while (currentIndex < pointList.Count - 1)
        {
            time += Time.deltaTime;
            float projectileSpeed = skillFx.ProjectileSpeedCurve.Evaluate(time) * skillFx.ProjectileSpeed;

            skillObject.transform.position = Vector3.MoveTowards(skillObject.transform.position, pointList[currentIndex + 1], projectileSpeed * Time.deltaTime);

            if (Vector3.Distance(skillObject.transform.position, pointList[currentIndex + 1]) < 0.1f)
            {
                currentIndex++;
            }

            yield return null;
        }
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
        ProjectileLine.enabled = true;
    }

    public void EndProjectileLine()
	{
		ProjectileLine.positionCount = 0;
        ProjectileLine.enabled = false;
    }
}
