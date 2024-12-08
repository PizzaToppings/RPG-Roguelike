using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LineSkill", menuName = "ScriptableObjects/SkillParts/LineSkill")]
public class SO_LineSkill : SO_Skillpart
{
    [Header(" - Targeting")]
    public OriginTileEnum OriginTileKind = OriginTileEnum.None;

    [Header(" - Line Specific")]
    public int[] Angles;
    public int PierceAmount = -1;

    public override SO_Skillpart Preview(BoardTile mouseOverTile, List<SO_Skillpart> skillshots) 
    {
        OriginTileMain = OriginTileKind;

        base.Preview(mouseOverTile, skillshots);
        SkillsManager skillsManager = SkillsManager.Instance;
		skillsManager.PreviewLine(this);

		return this;
    }
}
