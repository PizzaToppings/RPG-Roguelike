using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AOE_Skill", menuName = "ScriptableObjects/SkillParts/AOE_Skill")]
public class SO_TeleportSkill : SO_Skillpart
{
    public override SO_Skillpart Preview(BoardTile mouseOverTile, List<SO_Skillpart> skillshots)
    {
        base.Preview(mouseOverTile, skillshots);

        //var lowestCount = ori

        return this;
    }
}
