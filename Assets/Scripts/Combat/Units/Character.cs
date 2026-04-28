using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : Unit
{
    [HideInInspector] public int MaxEnergy = 10;
    [HideInInspector] public int Energy;

    public SO_MainSkill basicAttack;
    public SO_MainSkill basicSkill;

    public List<SO_MainSkill> skills = new List<SO_MainSkill>();

    [Space]
    public List<SO_MainSkill> consumables;

    public override void Init()
    {
        // Apply the selected character's skills before base.Init() so they are
        // ready for SetSkillData() and the skills manager.
        if (RunData.SelectedCharacter != null)
        {
            basicAttack = RunData.SelectedCharacter.basicAttack;
            basicSkill  = RunData.SelectedCharacter.basicSkill;
            skills      = new List<SO_MainSkill>(RunData.AcquiredSkills);
        }

        Friendly = true;
        base.Init(); // calls SetStats() then RollInitiative()

        SetEnergy(MaxEnergy);
        SetSkillData(basicAttack);

        skillsManager.OnSkillCastComplete.AddListener(StopCasting);
    }

    public override void Update()
    {
        if (UnitData.ActiveUnit == this)
        {
            base.Update();

            if (Input.GetKeyDown(KeyCode.Space) && UnitData.CurrentAction != CurrentActionKind.Animating)
                EndTurn();

            UseSkills();
        }
    }

	public override void SetStats()
	{
        if (RunData.SelectedCharacter != null)
        {
            // Apply stats from the ScriptableObject chosen at the start of the run.
            var soc = RunData.SelectedCharacter;
            UnitName        = soc.Name;
            MaxHitpoints    = soc.MaxHealth;
            Hitpoints       = soc.MaxHealth;
            MaxEnergy       = soc.MaxEnergy;
            MoveSpeed       = soc.MoveSpeed;
            PhysicalPower   = soc.PhysicalPower;
            MagicalPower    = soc.MagicalPower;
            PhysicalDefense = soc.PhysicalDefense;
            MagicalDefense  = soc.MagicalDefense;
        }
        else
        {
            base.SetStats(); // fallback: random MoveSpeed used during direct scene testing
        }
    }

    public override void RollInitiative()
    {
        if (RunData.SelectedCharacter != null)
            Initiative = RunData.SelectedCharacter.Initiative;
        else
            base.RollInitiative();
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
        if (skillIndex >= skills.Count)
            return;

        var skill = skills[skillIndex];
        ToggleSkill(skill);
    }

    public void ToggleSkill(SO_MainSkill skill)
    {
        if (skillsManager.CanCastSkill(skill, UnitData.ActiveUnit) == false)
            return;

		boardManager.VisualClear();
		// turn off
		if (UnitData.CurrentAction == CurrentActionKind.CastingSkillshot && SkillData.CurrentActiveSkill == skill)
        {
            StopCasting();
        }
        else
        // turn on
        {
            SkillData.Reset();
            UnitData.CurrentAction = CurrentActionKind.CastingSkillshot;

            SetSkillData(skill);
            uiManager.SetActiveSkillBorder(skill);

            var currentMouseTile = boardManager.GetCurrentMouseTile();
            skill.Preview(currentMouseTile, this);
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

                skillPartGroupData.SkillPartDatas.Add(skillPartData);
            }
        }
    }

    public void StopCasting()
    {
        boardManager.Clear();
        ui_Singletons.SetCursor(CursorType.Normal);
        uiManager.SetActiveSkillBorder(null);

        if (UnitData.ActiveUnit == this)
            UnitData.CurrentAction = CurrentActionKind.Basic;
		
        SkillData.Reset();
		boardManager.SetAOE(MoveSpeedLeft, Tile, null);
        skillVFXManager.EndProjectileLine();
        SetSkillData(basicAttack);
    }

    public override void PreviewSkills(BoardTile mouseOverTile)
    {
        base.PreviewSkills(mouseOverTile);

        SkillData.CurrentActiveSkill.Preview(mouseOverTile, this);
    }

    public override IEnumerator StartTurn()
    {
        yield return StartCoroutine(base.StartTurn());

		UnitData.CurrentAction = CurrentActionKind.Basic;
    }

    public override void SetStartOfTurnStats()
    {
        base.SetStartOfTurnStats();
        SetEnergy(MaxEnergy);
        ThisHealthbar.UpdateHealthbar();
    }

    public void SetEnergy(int amount)
    {
        Energy = amount;
        uiManager.SetEnergy(Energy, MaxEnergy);
    }

    public void ConsumeEnergy(int amount)
    {
        Energy -= amount;
        SetEnergy(Energy);
    }

    public override void Die()
    {
        skillsManager.OnSkillCastComplete.RemoveListener(StopCasting);
        base.Die();
    }
}