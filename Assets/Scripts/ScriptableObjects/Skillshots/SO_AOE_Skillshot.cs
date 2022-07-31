using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AOE_Skillshot", menuName = "ScriptableObjects/Skillshots/AOE_Skillshot")]
public class SO_AOE_Skillshot : SO_Skillshot
{
    public override void Preview(BoardTile mouseOverTile) 
    {
        base.Preview(mouseOverTile);
        SkillShotManager skillShotManager = SkillShotManager.skillShotManager;
        skillShotManager.GetAOE(this);
    }
}
