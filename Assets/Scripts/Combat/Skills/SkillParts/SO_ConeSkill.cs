using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ConeSkill", menuName = "ScriptableObjects/SkillParts/ConeSkill")]
public class SO_ConeSkill : SO_Skillpart
{   
    [Header(" - Cone Specific")]
    public bool isWide;

    public override SO_Skillpart Preview(BoardTile mouseOverTile, List<SO_Skillpart> skillshots,
       BoardTile overwriteOriginTile = null, BoardTile overwriteTargetTile = null, Unit Target = null)
    {
        base.Preview(mouseOverTile, skillshots);
        SkillsManager skillsManager = SkillsManager.Instance;
        skillsManager.PreviewCone(this);

        PartData.TilesHit.RemoveAll(x => HasDuplicateTile(x));

        return this;
    }
}
