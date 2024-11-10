using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FireBall", menuName = "ScriptableObjects/Skillshots/FireBall")]
public class FireBall : SO_MainSkill
{
    public override void Preview(BoardTile mouseOverTile)
    {
        base.Preview(mouseOverTile);

        SkillParts[0].Preview(mouseOverTile, SkillParts);

        SkillParts[1].Preview(mouseOverTile, SkillParts);
    }
}
