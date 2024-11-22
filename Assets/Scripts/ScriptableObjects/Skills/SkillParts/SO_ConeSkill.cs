using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ConeSkill", menuName = "ScriptableObjects/SkillParts/ConeSkill")]
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
