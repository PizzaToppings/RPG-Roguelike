using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : Unit
{
    [HideInInspector] public int MaxEnergy = 10;
    [HideInInspector] public int Energy;

    public SO_MainSkill basicAttackSO;
    public SO_MainSkill basicSkillSO;

    public Skill basicAttack = new Skill();
    public Skill basicSkill = new Skill();

    public List<SO_MainSkill> skillsSO = new List<SO_MainSkill>();
    [HideInInspector] public List<Skill> skills = new List<Skill>();

    [Space]
    public List<SO_MainSkill> consumablesSO = new List<SO_MainSkill>();
    public List<Skill> consumables = new List<Skill>();

    public override void Init()
    {
        // Apply the selected character's skills before base.Init() so they are
        // ready for SetSkillData() and the skills manager.
        if (RunData.SelectedCharacter != null)
        {
            basicAttack.Init(RunData.SelectedCharacter.basicAttack);
            basicSkill.Init(RunData.SelectedCharacter.basicSkill);
            skills = RunData.AcquiredSkills.ConvertAll(so => { var s = new Skill(); s.Init(so); return s; });
        }
        else
        {
            basicAttack.Init(basicAttackSO);
            basicSkill.Init(basicSkillSO);
            skills = skillsSO.ConvertAll(so => { var s = new Skill(); s.Init(so); return s; });
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

    public void ToggleSkill(Skill skill)
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

            var currentMouseTile = BoardData.CurrentMouseTile;
            skill.Preview(currentMouseTile, this);
		}
    }

    void SetSkillData(Skill skill)
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

        skillsManager.SetSkills(this);
        consumableManager.SetConsumables(this);

        UnitData.CurrentAction = CurrentActionKind.Basic;

        if (UnitData.ActiveUnit.Friendly && BoardData.CurrentMouseTile != null)
            BoardData.CurrentMouseTile.Target();
    }

    public override void SetStartOfTurnStats()
    {
        base.SetStartOfTurnStats();
        SetEnergy(MaxEnergy);
        ThisHealthbar.UpdateHealthbar();
    }

    public void InitSkills()
    {
        //basicAttack = new Skill();
        //basicSkill = new Skill();
        basicAttack.Init(basicAttackSO);
        basicSkill.Init(basicSkillSO);

        skills.Clear();
        for (int i = 0; i < 4; i++)
        {
            if (skillsSO[i] == null)
                continue;

            var skillSO = skillsSO[i];

            var skill = new Skill();
            skill.Init(skillSO);
            skills.Add(skill);
        }
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