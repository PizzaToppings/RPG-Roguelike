using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TargetUnitSkillshot", menuName = "ScriptableObjects/SkillshotParts/TargetUnitSkillshot")]
public class SO_TargetUnitSkillshot : SO_Skillshot
{
    public override SO_Skillshot Preview(BoardTile mouseOverTile, List<SO_Skillshot> skillshots) 
    {
        base.Preview(mouseOverTile, skillshots);
        SkillShotManager skillShotManager = SkillShotManager.Instance;
        skillShotManager.GetAOE(this);

        var target = TargetsHit.Find(x => x.IsTargeted);

        TargetsHit.Clear();
        TargetsHit.Add(target);

        return this;
    }
}
