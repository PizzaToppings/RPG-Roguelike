using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleEnemyAI : EnemyBaseAI
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
        yield return StartCoroutine(base.StartTurn());

        FindOptimalTile();
        yield return StartCoroutine(MoveToTile());

        yield return new WaitForSeconds(0.3f);
        yield return StartCoroutine(Attack());
    }

    public IEnumerator Attack()
    {
        StartCoroutine(uiManager.ShowActivityText("Strike"));
        yield return new WaitForSeconds(0.3f);

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
            Rotate(target.transform.position);
            skill.PartData = new SkillPartData();
            skill.PartData.TargetsHit.Add(target);
            StartCoroutine(skillsManager.CastSkillsPart(skill, this));
        }

        yield return new WaitForSeconds(2);

        EndTurn();
    }
}
