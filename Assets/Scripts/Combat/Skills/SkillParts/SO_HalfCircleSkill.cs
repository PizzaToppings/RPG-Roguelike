using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "halfCircleSkill", menuName = "ScriptableObjects/SkillParts/halfCircleSkill")]
public class SO_HalfCircleSkill : SO_Skillpart
{
    public override SO_Skillpart Preview(BoardTile mouseOverTile, List<SO_Skillpart> skillshots,
        BoardTile overwriteOriginTile = null, BoardTile overwriteTargetTile = null, Unit Target = null)
    {
        base.Preview(mouseOverTile, skillshots);
        SkillsManager skillsManager = SkillsManager.Instance;
        skillsManager.PreviewHalfCircle(this);
        return this; 
    }
}
