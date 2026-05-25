using System.Collections.Generic;

public class SkillPartGroupData
{
    public List<SkillPartData> SkillPartDatas = new List<SkillPartData>();
    public int GroupIndex;
    public bool CastOnTile;
    public bool CastOnTarget;

    public void Reset()
	{
        SkillPartDatas.ForEach(x => x.Reset());
    }
}
