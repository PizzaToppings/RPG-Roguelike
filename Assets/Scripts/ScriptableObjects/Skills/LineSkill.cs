using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TEST", menuName = "ScriptableObjects/Skillshots/TEST")]
public class LineSkillshots : SO_MainSkill
{
    public override void Preview(BoardTile mouseOverTile)
    {
        base.Preview(mouseOverTile);
    
        SkillParts[0].Preview(mouseOverTile, SkillParts);
    }
}
