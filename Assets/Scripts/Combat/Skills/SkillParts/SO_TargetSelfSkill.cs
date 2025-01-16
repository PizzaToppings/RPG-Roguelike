using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TargetSelfSkills", menuName = "ScriptableObjects/SkillParts/TargetSelfSkill")]
public class SO_TargetSelfSkill : SO_Skillpart
{
    public override SO_Skillpart Preview(BoardTile mouseOverTile, List<SO_Skillpart> skillshots, Unit caster,
       BoardTile overwriteOriginTile = null, BoardTile overwriteTargetTile = null, Unit Target = null)
    {
        base.Preview(mouseOverTile, skillshots, caster);

        caster.Tile.SetColor(tileColor);

        PartData.TargetsHit.Add(SkillData.Caster);
        return this;
    }
}
