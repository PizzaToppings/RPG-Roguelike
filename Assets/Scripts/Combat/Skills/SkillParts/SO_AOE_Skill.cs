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

        var duplicateTiles = new List<BoardTile>();

        PartData.TilesHit.RemoveAll(x => HasDuplicateTile(x));

        return this;
    }

    public override bool UnableToCast()
    {
        SkillsManager skillsManager = SkillsManager.Instance;
        skillsManager.GetAOE(this);

        if (PartData.TilesHit.Count == 0)
            return true;

        return false;
    }
}
