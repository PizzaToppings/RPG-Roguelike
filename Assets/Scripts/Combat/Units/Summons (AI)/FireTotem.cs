using System.Collections;
using UnityEngine;

public class FireTotem : Unit
{
    public SO_Skillpart skill;

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

        var damageData = skill.DamageEffect;
        damageData.Caster = this;

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
            skill.PartData = new SkillPartData();
            skill.PartData.TargetsHit.Add(target);
            StartCoroutine(skillsManager.CastSkillsPart(skill, this));
		}

        yield return new WaitForSeconds(2);

        EndTurn();
    }
}
