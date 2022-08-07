using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TargetTest", menuName = "ScriptableObjects/Skillshots/TargetTest")]
public class TargetTest : SO_MainSkillshot
{
    public override void Preview(BoardTile mouseOverTile)
    {
        base.Preview(mouseOverTile);

        SkillshotParts[0].Preview(mouseOverTile, SkillshotParts);

        if (SkillshotParts[0].TargetsHit.Count != 0)
            SkillshotParts[1].Preview(mouseOverTile, SkillshotParts);
    }
}
