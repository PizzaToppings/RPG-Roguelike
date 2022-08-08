using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SO_MainSkillshot : ScriptableObject
{
    public enum TargetKindEnum {Enemies, Allies, All};

    public bool MagicalDamage;
    public List<SO_Skillshot> SkillshotParts;

    public TargetKindEnum TargetKind;

    public virtual void Preview(BoardTile mouseOverTile) 
    {
        SkillshotData.CurrentMainSkillshot = this;
        BoardManager boardManager = BoardManager.boardManager;
        UnitManager unitManager = UnitManager.unitManager;
    }

    public virtual void Cast()
    {
        foreach (var sp in SkillshotParts)
            sp.Cast();
    }
}
