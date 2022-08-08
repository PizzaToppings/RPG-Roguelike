using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DefaultSkillshot", menuName = "ScriptableObjects/Skillshots/DefaultSkillshot")]
public class DefaultSkillshot : SO_MainSkillshot
{
    public override void Preview(BoardTile mouseOverTile)
    {
        base.Preview(mouseOverTile);

        foreach (var so in SkillshotParts)
            so.Preview(mouseOverTile, SkillshotParts);
    }
}
