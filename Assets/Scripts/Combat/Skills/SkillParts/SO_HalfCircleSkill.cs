using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "halfCircleSkill", menuName = "ScriptableObjects/SkillParts/halfCircleSkill")]
public class SO_HalfCircleSkill : SO_Skillpart
{
    [Header(" - Targeting")]
    public OriginTileEnum OriginTileKind = OriginTileEnum.None;

    public override SO_Skillpart Preview(BoardTile mouseOverTile, List<SO_Skillpart> skillshots)
    {
        OriginTileMain = OriginTileKind;

        base.Preview(mouseOverTile, skillshots);
        SkillsManager skillsManager = SkillsManager.Instance;
        skillsManager.PreviewHalfCircle(this);
        return this; 
    }
}
