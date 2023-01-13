using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SO_MainSkillshot : ScriptableObject
{
    public Sprite Image;

    [TextArea(15,20)]    
    public string Description;

    public enum TargetKindEnum {Enemies, Allies, All};

    public bool MagicalDamage;
    public List<SO_Skillshot> SkillshotParts;

    public TargetKindEnum TargetKind;

    public virtual void Preview(BoardTile mouseOverTile) 
    {
        SkillshotData.CurrentMainSkillshot = this;
        BoardManager boardManager = BoardManager.Instance;
        UnitManager unitManager = UnitManager.Instance;
    }

    public virtual void Cast()
    {
        foreach (var sp in SkillshotParts)
            sp.Cast();
    }
}
