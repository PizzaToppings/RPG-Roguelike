using System.Collections.Generic;
using UnityEngine;

public class SO_MainSkill : ScriptableObject
{
    public string SkillName;
    public Sprite Image;
    public Sprite Image_Inactive;

    [Space]
    public bool IsBasic;
    public bool IsConsumable;
    public bool IsMagical;

    [Space]
    public List<ClassEnum> Classes;

    [Space]
    [TextArea(15,20)]    
    public string Description;

    [Space]
    public List<SkillPartGroup> SkillPartGroups = new List<SkillPartGroup>(1);

    [Space]
    public int EnergyCost = 10;
    public int DefaultCharges = 1;

    [Space]
    public CursorType Cursor;
    public TileColor castLockColor;
}
