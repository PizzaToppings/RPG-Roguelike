using System.Collections;
using UnityEngine;

public class SimpleEnemyAI : EnemyBaseAI
{
    public SO_Skillpart skill;


    public override IEnumerator StartTurn()
    {
        yield return StartCoroutine(base.StartTurn());

        FindOptimalTile();
        yield return StartCoroutine(MoveToTile());

        yield return new WaitForSeconds(1);
        yield return StartCoroutine(Attack());
    }

    public IEnumerator Attack()
    {
        StartCoroutine(uiManager.ShowActivityText("Strike"));
        yield return new WaitForSeconds(1);

        var damageData = skill.DamageEffect;
        damageData.Caster = this;

        Unit target = null;
        float closestTargetRange = 0;

        foreach (var character in UnitData.Characters)
        {
            var blocked = boardManager.TileIsBehindClosedTile(Tile, character.Tile);
            if (blocked)
                continue;

            var range = boardManager.GetRangeBetweenTiles(Tile, character.Tile);
            if (range > skill.MaxRange)
                continue;

            if (target == null)
            {
                target = character;
                closestTargetRange = range;
            }
            else
            {
                if (range < closestTargetRange)
                {
                    target = character;
                    closestTargetRange = range;
                }
            }
        }

        if (target != null)
        {
            skill.PartData = new SkillPartData();
            skill.PartData.TargetsHit.Add(target);
            StartCoroutine(skillsManager.CastSkillsPart(skill));
        }

        yield return new WaitForSeconds(2);

        EndTurn();
    }
}
