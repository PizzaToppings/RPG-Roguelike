using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AOE_Skillshot", menuName = "ScriptableObjects/SkillshotParts/AOE_Skillshot")]
public class SO_AOE_Skill : SO_Skillpart
{
    public override SO_Skillpart Preview(BoardTile mouseOverTile, List<SO_Skillpart> skillshots) 
    {
        base.Preview(mouseOverTile, skillshots);
        
        SkillsManager skillShotManager = SkillsManager.Instance;
        skillShotManager.GetAOE(this);
        return this;
    }
}
