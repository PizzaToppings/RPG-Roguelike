using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ConeSkill", menuName = "ScriptableObjects/SkillParts/ConeSkill")]
public class SO_ConeSkill : SO_Skillpart
{
    [Header(" - Targeting")]
    public OriginTileEnum OriginTileKind = OriginTileEnum.None;
    
    [Header(" - Cone Specific")]
    public bool isWide;

    public override SO_Skillpart Preview(BoardTile mouseOverTile, List<SO_Skillpart> skillshots) 
    {
        OriginTileMain = OriginTileKind;

        base.Preview(mouseOverTile, skillshots);
        SkillsManager skillsManager = SkillsManager.Instance;
        skillsManager.PreviewCone(this);
        return this;
    }
}
