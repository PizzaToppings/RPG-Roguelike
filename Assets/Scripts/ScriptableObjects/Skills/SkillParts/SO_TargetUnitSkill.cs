using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TargetUnitSkillshot", menuName = "ScriptableObjects/SkillshotParts/TargetUnitSkillshot")]
public class SO_TargetUnitSkill : SO_Skillpart
{
    public override SO_Skillpart Preview(BoardTile mouseOverTile, List<SO_Skillpart> skillshots) 
    {
        base.Preview(mouseOverTile, skillshots);
        SkillsManager skillShotManager = SkillsManager.Instance;
        skillShotManager.GetAOE(this);

        var target = TargetsHit.Find(x => x.IsTargeted);

        TargetsHit.Clear();
        TargetsHit.Add(target);

        return this;
    }
}
