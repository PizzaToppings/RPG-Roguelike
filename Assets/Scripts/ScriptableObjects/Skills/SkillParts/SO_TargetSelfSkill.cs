using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TagretSelfSkillshot", menuName = "ScriptableObjects/SkillshotParts/TagretSelfSkillshot")]
public class SO_TargetSelfSkill : SO_Skillpart
{
    public override SO_Skillpart Preview(BoardTile mouseOverTile, List<SO_Skillpart> skillshots) 
    {
        base.Preview(mouseOverTile, skillshots);
        SkillsManager skillShotManager = SkillsManager.Instance;
        TargetsHit.Add(this.Caster);
        return this;
    }
}
