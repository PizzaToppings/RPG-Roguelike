using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FireBall", menuName = "ScriptableObjects/Skillshots/FireBall")]
public class FireBall : SO_MainSkillshot
{
    public override void Preview(BoardTile mouseOverTile)
    {
        base.Preview(mouseOverTile);

        SkillshotParts[0].Preview(mouseOverTile, SkillshotParts);

        SkillshotParts[1].Preview(mouseOverTile, SkillshotParts);
    }
}
