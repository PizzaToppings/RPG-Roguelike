using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SO_MainSkillshot : ScriptableObject
{
    public List<SO_Skillshot> SkillshotParts;

    public virtual void Preview(BoardTile mouseOverTile) 
    {
        BoardManager boardManager = BoardManager.boardManager;
        UnitManager unitManager = UnitManager.unitManager;

        // unitManager.ClearTargets();

        // for (int i = 0; i < Skillshots.Count; i++)
        // {
        //     boardManager.ClearMovementLeftPerTile();
        //     Skillshots[i] = Skillshots[i].Preview(mouseOverTile, Skillshots);
        // }
    }
}
