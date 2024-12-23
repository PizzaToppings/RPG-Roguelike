using System.Collections.Generic;

[System.Serializable]
public class SkillPartGroup
{
    public bool CastOnTile = false;
    public bool CastOnTarget = false;
    public List<SO_Skillpart> skillParts = new List<SO_Skillpart>(1);
}
