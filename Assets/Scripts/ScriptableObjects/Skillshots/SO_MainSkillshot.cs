using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MainSkillshot", menuName = "ScriptableObjects/MainSkillshot", order = 1)]
public class SO_MainSkillshot : ScriptableObject
{
    public List<SO_Skillshot> Skillshots;


    public void Preview(BoardTile mouseOverTile) 
    {
        BoardManager boardManager = BoardManager.boardManager;
        for (int i = 0; i < Skillshots.Count; i++)
        {
            boardManager.ClearMovementLeftPerTile();
            Skillshots[i] = Skillshots[i].Preview(mouseOverTile, Skillshots);
        }
    }
}
