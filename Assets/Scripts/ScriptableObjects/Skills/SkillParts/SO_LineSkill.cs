using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LineSkillshot", menuName = "ScriptableObjects/SkillshotParts/LineSkillshot")]
public class SO_LineSkill : SO_Skillpart
{
    public int[] Angles;
    public int PierceAmount;

    public override SO_Skillpart Preview(BoardTile mouseOverTile, List<SO_Skillpart> skillshots) 
    {
        base.Preview(mouseOverTile, skillshots);
        SkillsManager skillsManager = SkillsManager.Instance;
		skillsManager.PreviewLine(this, TargetTile);

		return this;
    }
}
