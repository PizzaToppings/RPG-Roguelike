using System.Collections.Generic;

public class SkillPartGroupData
{
    public List<SkillPartData> SkillPartDatas = new List<SkillPartData>();
    public string Name;
    public int Index;

    public void Reset()
	{
        SkillPartDatas.ForEach(x => x.Reset());
    }
}
