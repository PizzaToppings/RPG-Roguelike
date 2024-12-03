using System.Collections.Generic;

public class SkillPartData
{
    public List<BoardTile> TilesHit = new List<BoardTile>();
    public List<Unit> TargetsHit = new List<Unit>();
    public string Name;
    public int Index;

    public void Reset()
	{
        TilesHit.Clear();
        TargetsHit.Clear();
    }
}