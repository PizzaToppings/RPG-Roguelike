using UnityEngine;
using System.Linq;
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

    public IEnumerator Cast(SO_SKillVFX skillVFX, Unit caster)
    {
        yield return new WaitForSeconds(skillVFX.StartDelay);

        var skillObjects = GetOrCreateSpellObjects(skillVFX);

        for (int i = 0; i < skillObjects.Count; i++)
		{
            var originIndex = i < skillVFX.Origins.Count - 1 ? i : skillVFX.Origins.Count - 1;

            skillObjects[i].transform.position = skillVFX.Origins[originIndex];
            skillObjects[i].transform.rotation = Quaternion.Euler(caster.transform.rotation.eulerAngles + skillVFX.SkillOriginRotation);
        }

        if (skillVFX.SkillFxKind == SkillFxType.Projectile)
        {
            yield return StartCoroutine(MoveProjectilesAlongCurve(skillObjects, skillVFX));
        }

        if (skillVFX.SkillFxKind == SkillFxType.Animation)
        {
            var particleSystem = skillObjects[0].GetComponent<ParticleSystem>();

            if (skillVFX.StickToUnit)
            {
                while (true)
                {
                    if (skillVFX.GetDestinations().Count == 0)
                        break;

                    skillObjects[0].transform.position = skillVFX.GetDestinations()[0];

                    if (particleSystem.isStopped)
                        break;

                    yield return null;
                }
            }

            yield return new WaitUntil(() => particleSystem.isStopped);
        }

        yield return new WaitForSeconds(skillVFX.ExtendDelay);

        skillObjects.ForEach(skillObject => skillObject.SetActive(false));
        yield return new WaitForSeconds(skillVFX.EndDelay);
    }

    public void EndActiveVFX(SO_SKillVFX skillVFX)
    {
        var skillObjects = GetActiveSpellObjects(skillVFX);
        var particleSystems = skillObjects.Select(x => x.GetComponent<ParticleSystem>()).ToList();
        particleSystems.ForEach(x => x.Stop());
        skillObjects.ForEach(skillObject => skillObject.SetActive(false));
    }

    List<GameObject> GetActiveSpellObjects(SO_SKillVFX skillVFX)
    {
        var objectList = new List<GameObject>();

        foreach (Transform childTransform in skillObjectParent.transform)
        {
            var child = childTransform.gameObject;

            if (child == null)
                continue;

            if (child.activeSelf == false)
                continue;

            if (child.name == skillVFX.SkillObject.name + "(Clone)")
            {
                objectList.Add(child);
            }
        }

        return objectList;
    }

    List<GameObject> GetOrCreateSpellObjects(SO_SKillVFX skillVFX)
    {
        var objectList = new List<GameObject>();
        var objectAmount = skillVFX.Origins.Count > skillVFX.Destinations.Count ? skillVFX.Origins.Count : skillVFX.Destinations.Count;

        if (skillVFX.MainTargetOnly)
            objectAmount = 1;

        for (int i = 0; i < objectAmount; i++)
		{
            var originIndex = i < skillVFX.Origins.Count-1 ? i : skillVFX.Origins.Count-1;

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
                        child.transform.position = skillVFX.Origins[originIndex];
                        child.SetActive(true);
                        objectList.Add(child);
                    }
                }
            }
            objectList.Add(Instantiate(skillVFX.SkillObject, skillVFX.Origins[originIndex], Quaternion.identity, skillObjectParent));
        }
        
        return objectList;
    }


    private IEnumerator MoveProjectilesAlongCurve(List<GameObject> skillObjects, SO_SKillVFX skillFx)
    {
        var pointList = new List<Vector3>();
        Vector3 originPosition = skillFx.Origins[0] + Vector3.up;
        Vector3 targetPosition = skillFx.Destinations[0] + Vector3.up;

        var heightOffset = Vector3.Distance(originPosition, targetPosition);
        var middleOffset = Vector3.up * heightOffset * skillFx.ProjectileOffset * 0.5f;

        Vector3 middlePosition = Vector3.Lerp(originPosition, targetPosition, 0.5f) + middleOffset;

        for (float ratio = 0; ratio <= 1; ratio += 1f / projectileLineVertexCount)
        {
            var tangent1 = Vector3.Lerp(targetPosition, middlePosition, ratio);
            var tangent2 = Vector3.Lerp(middlePosition, originPosition, ratio);
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

            skillObjects[0].transform.position = Vector3.MoveTowards(skillObjects[0].transform.position, pointList[currentIndex + 1], projectileSpeed * Time.deltaTime);

            if (Vector3.Distance(skillObjects[0].transform.position, pointList[currentIndex + 1]) < 0.1f)
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
