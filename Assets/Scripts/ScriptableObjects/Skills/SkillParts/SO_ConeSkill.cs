using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ConeSkillshot", menuName = "ScriptableObjects/SkillshotParts/ConeSkillshot")]
public class SO_ConeSkill : SO_Skillpart
{
    public bool isWide;

    public override SO_Skillpart Preview(BoardTile mouseOverTile, List<SO_Skillpart> skillshots) 
    {
        base.Preview(mouseOverTile, skillshots);
        SkillsManager skillsManager = SkillsManager.Instance;
        skillsManager.PreviewCone(this);
        return this;
    }
}
