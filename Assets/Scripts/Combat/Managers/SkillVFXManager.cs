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
        // Big SkillVFX idea: Can toggle vfx on or off during skills, so it can last longer

        yield return new WaitForSeconds(skillVFX.StartDelay);

        var skillObjects = GetOrCreateSpellObjects(skillVFX);
        
        for (int i = 0; i < skillObjects.Count; i++)
		{
            if (skillVFX.UseLineRenderer)
                SetLineRenderer(skillObjects[0], skillVFX);
            
            var originIndex = i < skillVFX.Origins.Count - 1 ? i : skillVFX.Origins.Count - 1;

            skillObjects[i].transform.position = skillVFX.Origins[originIndex];
            skillObjects[i].transform.rotation = Quaternion.Euler(GetAimDirection(skillVFX, caster) + skillVFX.SkillOriginRotation);
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

            yield return new WaitForSeconds(particleSystem.main.duration);
        }

        yield return new WaitForSeconds(skillVFX.ExtendDelay);

        StartCoroutine(DisableSkillObject(skillVFX, skillObjects));

        yield return new WaitForSeconds(skillVFX.EndDelay);
    }

    Vector3 GetAimDirection(SO_SKillVFX skillVFX, Unit caster)
    {
        var mainTarget = skillVFX.Destinations[0];
        return (mainTarget - caster.position).normalized;
    }

    IEnumerator DisableSkillObject(SO_SKillVFX skillVFX, List<GameObject> skillObjects)
    {
        if (skillVFX.StickToUnit)
        {
            float time = 0;
            while (true)
            {
                time += Time.deltaTime;
                if (skillVFX.GetDestinations().Count == 0)
                    break;

                skillObjects[0].transform.position = skillVFX.GetDestinations()[0];

                if (skillVFX.UseLineRenderer)
                    SetLineRenderer(skillObjects[0], skillVFX);

                if (time >= skillVFX.DisableObjectDelay)
                    break;

                yield return null;
            }
        }
        else
        {
            yield return new WaitForSeconds(skillVFX.DisableObjectDelay);
        }

        skillObjects.ForEach(skillObject => skillObject.SetActive(false));
    }

    void SetLineRenderer(GameObject skillObject, SO_SKillVFX skillVFX)
    {
        var lineRenderer = skillObject.GetComponent<LineRenderer>();

        lineRenderer.SetPosition(0, skillVFX.GetLineRendererTarget(skillVFX.LineRendererOriginKind, skillObject));
        lineRenderer.SetPosition(1, skillVFX.GetLineRendererTarget(skillVFX.LineRendererDestinationKind, skillObject));
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


    private IEnumerator MoveProjectilesAlongCurve(List<GameObject> skillObjects, SO_SKillVFX skillVfx)
    {
        var pointList = new List<Vector3>();
        Vector3 originPosition = skillVfx.Origins[0] + Vector3.up;
        Vector3 targetPosition = skillVfx.Destinations[0] + Vector3.up;

        var heightOffset = Vector3.Distance(originPosition, targetPosition);
        var middleOffset = Vector3.up * heightOffset * skillVfx.ProjectileOffset * 0.5f;

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
            float projectileSpeed = skillVfx.ProjectileSpeedCurve.Evaluate(time) * skillVfx.ProjectileSpeed;

            skillObjects[0].transform.position = Vector3.MoveTowards(skillObjects[0].transform.position, pointList[currentIndex + 1], projectileSpeed * Time.deltaTime);

            if (Vector3.Distance(skillObjects[0].transform.position, pointList[currentIndex + 1]) < 0.1f)
            {
                currentIndex++;
            }

            if (skillVfx.UseLineRenderer)
                SetLineRenderer(skillObjects[0], skillVfx);

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
