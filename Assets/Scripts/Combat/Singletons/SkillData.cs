
using System.Collections.Generic;

public class SkillData
{
    public static SO_MainSkill CurrentActiveSkill = null;

    public static Unit Caster;
    
    public static int SkillPartGroupIndex = 0;
    public static List<SkillPartGroupData> SkillPartGroupDatas = new List<SkillPartGroupData>();

    // quick references
    public static SkillPartGroupData CurrentSkillPartGroupData => SkillPartGroupDatas[SkillPartGroupIndex];


    public static SkillPartData GetCurrentSkillPartData(int skillPartIndex)
	{
        return CurrentSkillPartGroupData.SkillPartDatas[skillPartIndex];
    }

    public static List<Unit> GetCurrentTargetsHit(int skillPartIndex)
    {
        return GetCurrentSkillPartData(skillPartIndex).TargetsHit;
    }

    public static List<BoardTile> GetCurrentTilesHit(int skillPartIndex)
	{
        return GetCurrentSkillPartData(skillPartIndex).TilesHit;
    }

    public static SkillPartData GetPreviousSkillPartData(int skillPartIndex)
	{
        return CurrentSkillPartGroupData.SkillPartDatas[skillPartIndex - 1];
    }

    public static List<Unit> GetPreviousTargetsHit(int skillPartIndex)
	{
        return GetPreviousSkillPartData(skillPartIndex).TargetsHit;
    }

    public static List<BoardTile> GetPreviousTilesHit(int skillPartIndex)
    {
        return GetPreviousSkillPartData(skillPartIndex).TilesHit;
    }

    public static void AddTileToCurrentList(int skillPartIndex, BoardTile tile)
    {
        if (tile == null)
            return;

        var currentTilesHit = GetCurrentTilesHit(skillPartIndex);

        if (currentTilesHit.Contains(tile) == false)
        {
            currentTilesHit.Add(tile);
        }
    }

    public static void AddTileRangeToCurrentList(int skillPartIndex, List<BoardTile> tiles)
    {
        foreach (var tile in tiles)
        {
            AddTileToCurrentList(skillPartIndex, tile);
        }
    }

    public static void AddTargetToCurrentList(int skillPartIndex, Unit target)
    {
        if (target == null)
            return;

        var currentTargetsHit = GetCurrentTargetsHit(skillPartIndex);

        if (currentTargetsHit.Contains(target) == false)
        {
            currentTargetsHit.Add(target);
        }
    }

    public static void Reset()
    {
        CurrentActiveSkill = null;
        SkillPartGroupDatas.ForEach(x => x.Reset());
        SkillPartGroupIndex = 0;
    }
}
