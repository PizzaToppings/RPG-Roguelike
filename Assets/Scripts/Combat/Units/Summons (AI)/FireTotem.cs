using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireTotem : Unit
{
    public SO_Skillpart skill;

    public override List<SO_SKillVFX> GetSkillVFXList()
    {
        var result = new List<SO_SKillVFX>();
        if (skill?.SkillVFX != null)
            result.AddRange(skill.SkillVFX);
        return result;
    }

    public override IEnumerator StartTurn()
    {
        UnitData.CurrentAction = CurrentActionKind.Animating;

        yield return StartCoroutine(base.StartTurn());

        yield return new WaitForSeconds(1);
        StartCoroutine(Firebolt());
    }

    public IEnumerator Firebolt()
    {
        StartCoroutine(uiManager.ShowActivityText("Firebolt"));
        yield return new WaitForSeconds(1);

        Unit target = null;
        float closestTargetRange = 0;

        foreach (var enemy in UnitData.Enemies)
		{
            var blocked = boardManager.TileIsBehindClosedTile(Tile, enemy.Tile);
            if (blocked)
                continue;

            var range = boardManager.GetRangeBetweenTiles(Tile, enemy.Tile);
            if (range > skill.MaxRange)
                continue;

            if (target == null)
			{
                target = enemy;
                closestTargetRange = range;
			}
            else
			{
                if (range < closestTargetRange)
				{
                    target = enemy;
                    closestTargetRange = range;
                }
			}
		}

        if (target != null)
		{
            Rotate(target.transform.position);
            skill.PartData = new SkillPartData();
            skill.PartData.TargetsHit.Add(target);
            StartCoroutine(skillsManager.CastSkillsPart(skill, this));
		}

        yield return new WaitForSeconds(2);

        EndTurn();
    }
}
