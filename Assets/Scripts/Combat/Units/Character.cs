using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : Unit
{
    public SO_MainSkill basicSkill;

    [HideInInspector] public int MaxSkillShotAmount = 4;
    [HideInInspector] public List<bool> SkillshotsEquipped;
    public List<SO_MainSkill> skills = new List<SO_MainSkill>();

    public override void Init()
    {
        Friendly = true;
        base.Init();
    }

    public override void Update()
    {
        if (UnitData.CurrentActiveUnit == this)
        {
            base.Update();

            if (Input.GetKeyDown(KeyCode.Space))
                EndTurn();

            UseSkills();
        }
    }

	public override void SetStats()
	{
		base.SetStats();

        if (skills?.Count == 0)
            return;

        SetSkillShots();
    }

    void SetSkillShots()
    {
        SkillData.Caster = this;

        for (int i = 0; i < MaxSkillShotAmount; i++)
        {
            if (i >= skills.Count)
            {
                SkillshotsEquipped.Add(false);
                continue;
            }

            if (skills[i] != null)
                SkillshotsEquipped.Add(true);
            else
                SkillshotsEquipped.Add(true);
        }
    }

    void UseSkills()
    {
        if (UnitData.CurrentActiveUnit != this || 
            (UnitData.CurrentAction != CurrentActionKind.Basic && UnitData.CurrentAction != CurrentActionKind.CastingSkillshot))
            return;

        if (Input.GetKeyDown(KeyCode.Alpha1))
            ToggleSpecialSkill(0);

        if (Input.GetKeyDown(KeyCode.Alpha2))
            ToggleSpecialSkill(1);

        if (Input.GetKeyDown(KeyCode.Alpha3))
            ToggleSpecialSkill(2);

        if (Input.GetKeyDown(KeyCode.Alpha4))
            ToggleSpecialSkill(3);

    }

    public void ToggleSpecialSkill(int skillIndex)
    {
        var skill = skills[skillIndex];
        ToggleSkill(skill);
    }

    public void ToggleSkill(SO_MainSkill skill)
    {
        boardManager.Clear();
        // turn off
        if (SkillData.CurrentActiveSkill == skill)
        {
            StopCasting();
        }
        // turn on
        else
        {
            if (skill.MagicalDamage && statusEffects.Find(x => x.statusEfectType == StatusEfectEnum.Silenced) != null)
                return;

            SkillData.Reset();

            UnitData.CurrentAction = CurrentActionKind.CastingSkillshot;
            SkillData.CurrentActiveSkill = skill;

            for (var i = 0; i < skill.SkillPartGroups.Count; i++)
            {
                var spg = skill.SkillPartGroups[i];
                var skillPartGroupData = new SkillPartGroupData();
                SkillData.SkillPartGroupDatas.Add(skillPartGroupData);

                for (var s = 0; s < spg.skillParts.Count; s++)
                {
                    var skillPartData = new SkillPartData
                    {
                        Index = s
                    };

                    spg.skillParts[s].SkillPartIndex = s;
                    spg.skillParts[s].PartData = skillPartData;
                    spg.skillParts[s].MatchedSkillPartGroupData = skillPartGroupData;

                    skillPartGroupData.SkillPartDatas.Add(skillPartData);
                }
            }

            //preview skill  --> This might be removeable
            skill.Reset();
            foreach (var skillPartGroup in skill.SkillPartGroups)
            {
                foreach (var skillPart in skillPartGroup.skillParts)
                {
                    if (skillPart.OriginTileKind == OriginTileEnum.Caster)
                    {
                        foreach (var tile in currentTile.connectedTiles)
                        {
                            if (tile == null)
                                continue;

                            skillPart.TargetTile = tile;
                            break;
                        }
                        skillPart.Preview(currentTile, skillPartGroup.skillParts);
                    }
                }
            }
        }
    }

    public void StopCasting()
    {
        boardManager.Clear();
        UnitData.CurrentAction = CurrentActionKind.Basic;
        SkillData.Reset();
        boardManager.SetAOE(MoveSpeedLeft, currentTile, null);
    }

    public override void PreviewSkills(BoardTile mouseOverTile)
    {
        base.PreviewSkills(mouseOverTile);

        SkillData.CurrentActiveSkill.Preview(mouseOverTile);
    }

    public override IEnumerator StartTurn()
    {
        yield return null;
        StartCoroutine(base.StartTurn());

		UnitData.CurrentAction = CurrentActionKind.Basic;
	}
}
