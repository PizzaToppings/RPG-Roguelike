using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : Unit
{
    [HideInInspector] public int MaxEnergy = 100;
    [HideInInspector] public int Energy;

    public SO_MainSkill basicAttack;
    public SO_MainSkill basicSkill;

    [HideInInspector] public int MaxSkillShotAmount = 4;
    [HideInInspector] public List<bool> SkillshotsEquipped;
    public List<SO_MainSkill> skills = new List<SO_MainSkill>();

    [Space]
    public List<SO_MainSkill> consumables;

    public override void Init()
    {
        Friendly = true;
        base.Init();

        Energy = MaxEnergy;

        SetSkillData(basicAttack);
    }

    public override void Update()
    {
        if (UnitData.ActiveUnit == this)
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

        SetSkills();
    }

    void SetSkills()
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
        if (UnitData.ActiveUnit != this || 
            (UnitData.CurrentAction != CurrentActionKind.Basic && UnitData.CurrentAction != CurrentActionKind.CastingSkillshot))
            return;

        if (Input.GetKeyDown(KeyCode.Q))
            ToggleSkill(basicAttack);

        if (Input.GetKeyDown(KeyCode.E))
            ToggleSkill(basicSkill);

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
        if (skillsManager.CanCastSkill(skill) == false)
            return;

		boardManager.VisualClear();
		// turn off
		if (UnitData.CurrentAction == CurrentActionKind.CastingSkillshot && SkillData.CurrentActiveSkill == skill)
        {
            StopCasting();
        }
        // turn on
        else
        {
            SkillData.Reset();
            UnitData.CurrentAction = CurrentActionKind.CastingSkillshot;

            SetSkillData(skill);

            var currentMouseTile = boardManager.GetCurrentMouseTile();
            skill.Preview(currentMouseTile);
		}
    }

    void SetSkillData(SO_MainSkill skill)
	{
        SkillData.CurrentActiveSkill = skill;
        SkillData.SkillPartGroupDatas.Clear();

        for (var i = 0; i < skill.SkillPartGroups.Count; i++)
        {
            var spg = skill.SkillPartGroups[i];
            var skillPartGroupData = new SkillPartGroupData();
            skillPartGroupData.CastOnTile = spg.CastOnTile;
            skillPartGroupData.CastOnTarget = spg.CastOnTarget;
            skillPartGroupData.GroupIndex = i;
            SkillData.SkillPartGroupDatas.Add(skillPartGroupData);

            for (var s = 0; s < spg.skillParts.Count; s++)
            {
                var skillPartData = new SkillPartData
                {
                    PartIndex = s,
                    GroupIndex = i
                };

                spg.skillParts[s].SkillPartIndex = s;
                spg.skillParts[s].PartData = skillPartData;
                spg.skillParts[s].MatchedSkillPartGroupData = skillPartGroupData;

                skillPartGroupData.SkillPartDatas.Add(skillPartData);
            }
        }
    }

    public void StopCasting()
    {
        boardManager.Clear();
        ui_Singletons.SetCursor(CursorType.Normal);
        UnitData.CurrentAction = CurrentActionKind.Basic;
		SkillData.Reset();
		boardManager.SetAOE(MoveSpeedLeft, Tile, null);
        skillVFXManager.EndProjectileLine();
        SetSkillData(basicAttack);
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
        Energy = MaxEnergy;
    }
}