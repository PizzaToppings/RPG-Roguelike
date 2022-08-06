using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TargetSkillshot", menuName = "ScriptableObjects/SkillshotParts/TargetSkillshot")]
public class SO_TargetSkillshot : SO_Skillshot
{
    public Unit mouseOverTarget;

    public override SO_Skillshot Preview(BoardTile mouseOverTile, List<SO_Skillshot> skillshots) 
    {
        base.Preview(mouseOverTile, skillshots);
        SkillShotManager skillShotManager = SkillShotManager.skillShotManager;
        skillShotManager.GetAOE(this);
        
        TargetsHit.ForEach(x => x.OnClick += ClickTarget);
        return this;
    }

    void ClickTarget(Unit target)
    {
        Debug.Log("Dealt " + Damage + " to " + target.UnitName);
    }
}
