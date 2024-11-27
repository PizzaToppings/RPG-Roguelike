using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TargetUnitSkill", menuName = "ScriptableObjects/SkillParts/TargetUnitSkill")]
public class SO_TargetUnitSkill : SO_Skillpart
{
    public override SO_Skillpart Preview(BoardTile mouseOverTile, List<SO_Skillpart> skillshots) 
    {
        base.Preview(mouseOverTile, skillshots);
        SkillsManager skillsManager = SkillsManager.Instance;
        skillsManager.GetAOE(this);

        var target = SkillData.GetCurrentTargetsHit(SkillPartIndex).Find(x => x.IsTargeted);

        SkillData.GetCurrentTargetsHit(SkillPartIndex).Clear();
        SkillData.GetCurrentTargetsHit(SkillPartIndex).Add(target);

        return this;
    }
}
