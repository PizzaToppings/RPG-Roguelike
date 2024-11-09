using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MainSkillshot", menuName = "ScriptableObjects/Skillshots/MainSkillshot")]
public class SO_MainSkill : ScriptableObject
{
    public Sprite Image;

    [TextArea(15,20)]    
    public string Description;

    public enum TargetKindEnum {Enemies, Allies, All};

    public bool MagicalDamage; //change to enum?
    public List<SO_Skillpart> SkillshotParts;

    public TargetKindEnum TargetKind;

    public virtual void Preview(BoardTile mouseOverTile) 
    {
        SkillshotData.CurrentMainSkillshot = this;
        BoardManager boardManager = BoardManager.Instance;
        UnitManager unitManager = UnitManager.Instance;

        for (int i = 0; i < SkillshotParts.Count; i++)
		{
            SkillshotParts[i].skillPartIndex = i;
            SkillshotParts[i].Preview(mouseOverTile, SkillshotParts);
		}
    }

    public virtual void Cast()
    {
        foreach (var sp in SkillshotParts)
            sp.Cast();
    }
}
