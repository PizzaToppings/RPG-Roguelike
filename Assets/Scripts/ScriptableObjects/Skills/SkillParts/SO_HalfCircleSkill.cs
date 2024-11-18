using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "halfCircleSkillshot", menuName = "ScriptableObjects/SkillshotParts/halfCircleSkillshot")]
public class SO_HalfCircleSkill : SO_Skillpart
{
    public override SO_Skillpart Preview(BoardTile mouseOverTile, List<SO_Skillpart> skillshots)
    {
        base.Preview(mouseOverTile, skillshots);
        SkillsManager skillsManager = SkillsManager.Instance;
        skillsManager.PreviewHalfCircle(this, TargetTile);
        return this; 
    }
}
