using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TargetSelfSkills", menuName = "ScriptableObjects/SkillParts/TargetSelfSkill")]
public class SO_TargetSelfSkill : SO_Skillpart
{
    public override SO_Skillpart Preview(BoardTile mouseOverTile, List<SO_Skillpart> skillshots) 
    {
        OriginTargetKind = OriginTargetEnum.Caster;

        base.Preview(mouseOverTile, skillshots);
        SkillsManager skillShotManager = SkillsManager.Instance;
        PartData.TargetsHit.Add(SkillData.Caster);
        return this;
    }
}
