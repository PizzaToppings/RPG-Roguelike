using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DefaultSkillshot", menuName = "ScriptableObjects/Skillshots/DefaultSkillshot")]
public class DefaultSkill : SO_MainSkill
{
    public override void Preview(BoardTile mouseOverTile)
    {
        base.Preview(mouseOverTile);

        foreach (var so in SkillParts)
            so.Preview(mouseOverTile, SkillParts);
    }
}
