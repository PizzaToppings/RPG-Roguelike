using System.Collections.Generic;

public class SkillPartData
{
    public List<BoardTile> TilesHit = new List<BoardTile>();
    public List<Unit> TargetsHit = new List<Unit>();
    public int PartIndex;
    public int GroupIndex;
    public bool CanCast = false;

    public void Reset()
	{
        TilesHit.Clear();
        TargetsHit.Clear();
    }
}