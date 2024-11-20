
using System.Collections.Generic;

public class SkillData
{
    public static SO_MainSkill CurrentMainSkillshot = null;
    public static SO_Skillpart CurrentSkillshot = null;
    public static int? CurrentSkillshotIndex = null;

    public static Unit Caster;
    
    public static int SkillPartIndex = 0;
    public static List<SkillPartData> SkillPartDatas = new List<SkillPartData>();

    static SkillPartData CurrentSkillPartData => SkillPartDatas[SkillPartIndex];
    public static List<Unit> CurrentTargetsHit => CurrentSkillPartData.TargetsHit;
    public static List<BoardTile> CurrentTilesHit => CurrentSkillPartData.TilesHit;

    public static void AddTileToCurrentList(BoardTile tile)
    {
        if (tile == null)
            return;

        if (CurrentTilesHit.Contains(tile) == false)
        {
            CurrentTilesHit.Add(tile);
        }
    }

    public static void AddTileRangeToCurrentList(List<BoardTile> tiles)
    {
        foreach (var tile in tiles)
        {
            AddTileToCurrentList(tile);
        }
    }

    public static void AddTargetToCurrentList(Unit target)
    {
        if (target == null)
        {
            return;
        }

        if (CurrentTargetsHit.Contains(target) == false)
        {
            CurrentTargetsHit.Add(target);
        }
    }

    public static void Reset()
    {
        CurrentSkillshotIndex = null;
        SkillPartDatas.Clear();
        SkillPartIndex = 0;
    }
}
