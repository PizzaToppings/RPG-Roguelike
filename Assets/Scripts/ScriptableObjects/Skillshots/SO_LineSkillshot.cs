using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LineSkillshot", menuName = "ScriptableObjects/Skillshots/LineSkillshot", order = 1)]
public class SO_LineSkillshot : SO_Skillshot
{
    public int Direction;

    public override void Preview(BoardTile mouseOverTile) 
    {
        base.Preview(mouseOverTile);
        SkillShotManager skillShotManager = SkillShotManager.skillShotManager;
        skillShotManager.PreviewLine(this, mouseOverTile);
    }
}
