using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ConeSkill", menuName = "ScriptableObjects/SkillParts/ConeSkill")]
public class SO_ConeSkill : SO_Skillpart
{   
    [Header(" - Cone Specific")]
    public bool isWide;

    public override SO_Skillpart Preview(BoardTile mouseOverTile, List<SO_Skillpart> skillshots, Unit caster,
       BoardTile overwriteOriginTile = null, BoardTile overwriteTargetTile = null, Unit Target = null)
    {
        base.Preview(mouseOverTile, skillshots, caster);
        TargetSkillsManager targetSkillsManager = TargetSkillsManager.Instance;
        targetSkillsManager.PreviewCone(this);

        PartData.TilesHit.RemoveAll(x => HasDuplicateTile(x));

        return this;
    }
}
