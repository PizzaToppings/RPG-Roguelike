using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LineSkillshot", menuName = "ScriptableObjects/SkillshotParts/LineSkillshot")]
public class SO_LineSkillshot : SO_Skillshot
{
    public int[] Directions;
    public int PierceAmount;

    public override SO_Skillshot Preview(BoardTile mouseOverTile, List<SO_Skillshot> skillshots) 
    {
        base.Preview(mouseOverTile, skillshots);

        SkillShotManager skillShotManager = SkillShotManager.skillShotManager;
        skillShotManager.PreviewLine(this, mouseOverTile);
        return this;
    }
}
