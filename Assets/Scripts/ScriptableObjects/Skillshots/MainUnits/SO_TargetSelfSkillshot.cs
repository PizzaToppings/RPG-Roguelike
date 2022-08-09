using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TagretSelfSkillshot", menuName = "ScriptableObjects/SkillshotParts/TagretSelfSkillshot")]
public class SO_TargetSelfSkillshot : SO_Skillshot
{
    public override SO_Skillshot Preview(BoardTile mouseOverTile, List<SO_Skillshot> skillshots) 
    {
        base.Preview(mouseOverTile, skillshots);
        SkillShotManager skillShotManager = SkillShotManager.skillShotManager;
        TargetsHit.Add(this.Caster);
        return this;
    }
}
