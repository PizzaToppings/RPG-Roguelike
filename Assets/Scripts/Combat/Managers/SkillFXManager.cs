using UnityEngine;
using System.Collections;

public class SkillFXManager : MonoBehaviour
{
    public static SkillFXManager Instance;

    // still needs to animate the casting character and affected characters

    public void Init()
    {
        Instance = this;
    }

    public void CastSkill(SO_SKillFX skillFx)
    {
        StartCoroutine("Cast", skillFx);
    }

    IEnumerator Cast(SO_SKillFX skillFx)
    {
        if (skillFx.SkillFxKind == SkillFxType.Projectile)
        {
            float distance = Vector3.Distance(skillFx.Origin, skillFx.Destination);
            var skillObject = Instantiate(skillFx.SkillObject, skillFx.Origin, Quaternion.identity);

            // While the object hasn't reached the destination
            while (distance > 0.1f)
            {
                // Move the GameObject towards the end position
                skillObject.transform.position = Vector3.MoveTowards(transform.position, skillFx.Destination, skillFx.ProjectileSpeed * Time.deltaTime);

                // Recalculate the distance
                distance = Vector3.Distance(skillObject.transform.position, skillFx.Destination);

                // Wait until the next frame
                yield return null;
            }
        }
    }
}
