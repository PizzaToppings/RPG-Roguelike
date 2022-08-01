using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ConeSkillshot", menuName = "ScriptableObjects/Skillshots/ConeSkillshot")]
public class SO_ConeSkillshot : SO_Skillshot
{
    public bool isWide;

    public override SO_Skillshot Preview(BoardTile mouseOverTile, List<SO_Skillshot> skillshots) 
    {
        base.Preview(mouseOverTile, skillshots);
        SkillShotManager skillShotManager = SkillShotManager.skillShotManager;
        skillShotManager.PreviewCone(this, mouseOverTile);
        return this;
    }
}
