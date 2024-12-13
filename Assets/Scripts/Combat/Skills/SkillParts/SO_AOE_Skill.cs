using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AOE_Skill", menuName = "ScriptableObjects/SkillParts/AOE_Skill")]
public class SO_AOE_Skill : SO_Skillpart
{
    public override SO_Skillpart Preview(BoardTile mouseOverTile, List<SO_Skillpart> skillshots,
       BoardTile overwriteOriginTile = null, BoardTile overwriteTargetTile = null, Unit Target = null)
    {
        base.Preview(mouseOverTile, skillshots);

        SkillsManager skillShotManager = SkillsManager.Instance;
        skillShotManager.GetAOE(this);
        return this;
    }
}
