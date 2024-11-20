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

        var target = SkillData.CurrentTargetsHit.Find(x => x.IsTargeted);

        SkillData.CurrentTargetsHit.Clear();
        SkillData.CurrentTargetsHit.Add(target);

        return this;
    }
}
