using System.Collections.Generic;
using UnityEngine;

public class SO_MainSkill : ScriptableObject
{
    public Sprite Image;

    [TextArea(15,20)]    
    public string Description;

    public enum TargetKindEnum {Enemies, Allies, All};

    public bool MagicalDamage; //change to enum?
    public List<SO_Skillpart> SkillParts;

    public TargetKindEnum TargetKind;

    public virtual void Preview(BoardTile mouseOverTile) 
    {
        BoardManager boardManager = BoardManager.Instance;
        UnitManager unitManager = UnitManager.Instance;

        for (int i = 0; i < SkillParts.Count; i++)
		{
            SkillParts[i].skillPartIndex = i;
            SkillParts[i].Preview(mouseOverTile, SkillParts);
		}
    }

    public virtual void Cast()
    {
        foreach (var sp in SkillParts)
            sp.Cast();
    }
}
