
using System.Collections.Generic;
using UnityEngine;

public class SkillData
{
    public static Skill CurrentActiveSkill = null;

    public static Unit Caster;
    public static bool CastOnTile => CurrentSkillPartGroupData.CastOnTile;
    public static bool CastOnTarget => CurrentSkillPartGroupData.CastOnTarget;

    public static int SkillPartGroupIndex = 0;
    public static List<SkillPartGroupData> SkillPartGroupDatas = new List<SkillPartGroupData>();

    public static SkillPartGroupData CurrentSkillPartGroupData => SkillPartGroupDatas[SkillPartGroupIndex];

    static Dictionary<Skill, int> SkillCharges = new Dictionary<Skill, int>();
    static Dictionary<Skill, int> SkillCooldowns = new Dictionary<Skill, int>();

    public static int GetCharges(Skill skill)
    {
        return SkillCharges.TryGetValue(skill, out int charges) ? charges : skill.DefaultCharges;
    }

    public static void SetCharges(Skill skill, int value)
    {
        SkillCharges[skill] = value;
    }

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
        SkillPartGroupDatas.ForEach(x => x.Reset());
        SkillPartGroupIndex = 0;
    }

    public static int GetCooldown(Skill skill)
    {
        return SkillCooldowns.TryGetValue(skill, out int cd) ? cd : 0;
    }

    public static void SetCooldown(Skill skill, int value)
    {
        SkillCooldowns[skill] = Mathf.Max(0, value);
    }

    public static void TickCooldownsForUnit(Unit unit)
    {
        // Reduce cooldowns on all skills that belong to the given unit (basic + basicSkill + specials + consumables)
        if (unit == null) return;

        var skills = new System.Collections.Generic.List<Skill>();
        if (unit is Character c)
        {
            if (c.basicAttack != null) skills.Add(c.basicAttack);
            if (c.basicSkill != null) skills.Add(c.basicSkill);
            if (c.skills != null) skills.AddRange(c.skills);
            if (c.consumables != null) skills.AddRange(c.consumables);
        }
        else if (unit is Enemy)
        {
            // enemies may have abilities represented differently; skip
        }

        foreach (var s in skills)
        {
            if (s == null) continue;
            if (SkillCooldowns.TryGetValue(s, out int cd) && cd > 0)
            {
                cd--;
                SkillCooldowns[s] = Mathf.Max(0, cd);
            }
        }
    }
}
