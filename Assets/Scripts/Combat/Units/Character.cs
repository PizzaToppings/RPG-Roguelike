using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Character : Unit
{
    [HideInInspector] public int partyMemberIndex = 0;

    [HideInInspector] public int MaxEnergy = 10;
    [HideInInspector] public int Energy;

    public SO_Character characterSO;

    public SO_MainSkill basicAttackSO;
    public SO_MainSkill basicSkillSO;

    public Skill basicAttack = new Skill();
    public Skill basicSkill = new Skill();

    public List<SO_MainSkill> skillsSO = new List<SO_MainSkill>();
    [HideInInspector] public List<Skill> skills = new List<Skill>();

    [Space]
    public List<SO_MainSkill> consumablesSO = new List<SO_MainSkill>();
    public List<Skill> consumables = new List<Skill>();

    [HideInInspector] public List<Trinket> trinkets = new List<Trinket>();

    [HideInInspector] public UnityEngine.Events.UnityEvent<Skill> OnSkillCastEvent = new UnityEngine.Events.UnityEvent<Skill>();

    public override void Init()
    {
        var partyMember = partyMemberIndex < RunData.Party.Count ? RunData.Party[partyMemberIndex] : null;
        if (partyMember != null)
        {
            basicAttack.Init(partyMember.Character.basicAttack);
            basicSkill.Init(partyMember.Character.basicSkill);
            skills = new List<Skill>(partyMember.Skills);
        }
        else
        {
            basicAttack.Init(basicAttackSO);
            basicSkill.Init(basicSkillSO);
            skills = skillsSO.Where(so => so != null).Select(so => { var s = new Skill(); s.Init(so); return s; }).ToList();
        }

        Friendly = true;
        base.Init(); // calls SetStats() then RollInitiative()

        SetEnergy(MaxEnergy);
        SetSkillData(basicAttack);

        skillsManager.OnSkillCastComplete.AddListener(StopCasting);
    }

    public void InitAugments()
    {
        var partyMember = partyMemberIndex < RunData.Party.Count ? RunData.Party[partyMemberIndex] : null;
        if (partyMember == null) return;

        var allSkills = new List<Skill> { basicAttack, basicSkill };
        allSkills.AddRange(skills);

        foreach (var skill in allSkills)
        {
            if (skill?.mainSkillSO == null) continue;
            if (partyMember.SkillAugments.TryGetValue(skill.mainSkillSO, out var augmentSOs) && augmentSOs.Count > 0)
            {
                Debug.Log($"[SkillAugment] {UnitName}: initializing {augmentSOs.Count} augment(s) for '{skill.mainSkillSO.SkillName}'.");
                skill.InitAugments(augmentSOs, this);
            }
        }
    }

    public void InitTrinkets()
    {
        var partyMember = partyMemberIndex < RunData.Party.Count ? RunData.Party[partyMemberIndex] : null;
        if (partyMember != null)
        {
            Debug.Log($"[Trinket] {UnitName}: initializing {partyMember.Trinkets.Count} trinket(s) from party data.");
            trinkets = partyMember.Trinkets.Select(so => { var t = new Trinket(); t.Init(so, this); return t; }).ToList();
        }
        else
        {
            if (characterSO != null && characterSO.BasicTrinket != null)
            {
                Debug.Log($"[Trinket] {UnitName}: initializing BasicTrinket '{characterSO.BasicTrinket.TrinketName}' from characterSO.");
                var t = new Trinket();
                t.Init(characterSO.BasicTrinket, this);
                trinkets = new List<Trinket> { t };
            }
            else
            {
                Debug.Log($"[Trinket] {UnitName}: no trinkets to initialize.");
            }
        }
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
        var partyMember = partyMemberIndex < RunData.Party.Count ? RunData.Party[partyMemberIndex] : null;
        if (partyMember != null)
        {
            var soc = partyMember.Character;
            UnitName        = soc.Name;
            MaxHitpoints    = soc.MaxHealth;
            Hitpoints       = partyMember.CurrentHitpoints > 0 ? partyMember.CurrentHitpoints : soc.MaxHealth;
            MaxEnergy       = soc.MaxEnergy;
            MoveSpeed       = soc.MoveSpeed;
            PhysicalPower   = soc.PhysicalPower;
            MagicalPower    = soc.MagicalPower;
            PhysicalDefense = soc.PhysicalDefense;
            MagicalDefense  = soc.MagicalDefense;

            // Apply permanent bonuses from Instant trinkets accumulated during this run
            MaxHitpoints += partyMember.BonusMaxHitpoints;
            MaxEnergy    += partyMember.BonusMaxEnergy;
        }
        else
        {
            if (characterSO != null)
                UnitName = characterSO.Name;
            base.SetStats(); // fallback: direct scene testing without RunManager
        }
    }

    public override void RollInitiative()
    {
        var partyMember = partyMemberIndex < RunData.Party.Count ? RunData.Party[partyMemberIndex] : null;
        if (partyMember != null)
            Initiative = partyMember.Character.Initiative;
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
            
            // Update damage predictions for the newly selected skill
            UpdateAllDamagePredictions();
            
            // Update displacement projections
            if (displacementPreviewManager != null)
                displacementPreviewManager.ShowDisplacementPreviews(SkillData.CurrentActiveSkill);
		}
    }

    void SetSkillData(Skill skill)
	{
        // Hide all enemy damage predictions when changing skills
        HideAllEnemyDamagePredictions();
        
        // Hide displacement projections when changing skills
        if (displacementPreviewManager != null)
            displacementPreviewManager.HideAllProjections();

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
        if (UnitData.ActiveUnit != this)
            return;

        boardManager.Clear();
        ui_Singletons.SetCursor(CursorType.Normal);
        uiManager.SetActiveSkillBorder(null);
        UnitData.CurrentAction = CurrentActionKind.Basic;

        SkillData.Reset();
        boardManager.SetAOE(MoveSpeedLeft, Tile, null);
        skillVFXManager.EndProjectileLine();
        SetSkillData(basicAttack);

        // Hide all enemy damage predictions
        HideAllEnemyDamagePredictions();
        
        // Hide all displacement projections
        if (displacementPreviewManager != null)
            displacementPreviewManager.HideAllProjections();
    }

    void HideAllEnemyDamagePredictions()
    {
        var enemies = UnitData.Enemies;
        if (enemies == null) return;

        foreach (var enemy in enemies)
        {
            if (enemy != null && enemy.ThisHealthbar is FloatingHealthbar floatingHealthbar)
            {
                floatingHealthbar.HideDamagePreview();
            }
        }
    }

    public override void PreviewSkills(BoardTile mouseOverTile)
    {
        base.PreviewSkills(mouseOverTile);

        SkillData.CurrentActiveSkill.Preview(mouseOverTile, this);

        // Update damage predictions for all enemies in the skill's area
        UpdateAllDamagePredictions();
        
        // Update displacement projections
        if (displacementPreviewManager != null)
            displacementPreviewManager.ShowDisplacementPreviews(SkillData.CurrentActiveSkill);
    }

    void UpdateAllDamagePredictions()
    {
        // First hide all predictions
        HideAllEnemyDamagePredictions();

        if (SkillData.CurrentActiveSkill == null || UnitData.ActiveUnit != this)
            return;

        // Get all enemies that will be hit by the current skill
        // IMPORTANT: Use the runtime Skill.SkillPartGroups, not mainSkillSO.SkillPartGroups
        // because Skill creates instantiated copies with PartData set
        var currentSpg = SkillData.CurrentActiveSkill.SkillPartGroups[SkillData.SkillPartGroupIndex];
        
        foreach (var skillPart in currentSpg.skillParts)
        {
            if (skillPart.PartData?.TargetsHit == null)
                continue;

            foreach (var target in skillPart.PartData.TargetsHit)
            {
                if (target is Enemy enemy && enemy.ThisHealthbar is FloatingHealthbar floatingHealthbar)
                {
                    floatingHealthbar.ShowDamagePreview(SkillData.CurrentActiveSkill);
                }
            }
        }
    }

    public override List<SO_SKillVFX> GetSkillVFXList()
    {
        var result = new List<SO_SKillVFX>();
        var allSkills = new List<Skill> { basicAttack, basicSkill };
        allSkills.AddRange(skills);
        allSkills.AddRange(consumables);
        foreach (var skill in allSkills)
        {
            if (skill == null) continue;
            foreach (var spg in skill.SkillPartGroups)
                foreach (var sp in spg.skillParts)
                    if (sp?.SkillVFX != null)
                        result.AddRange(sp.SkillVFX);
        }
        return result;
    }

    public override IEnumerator StartTurn()
    {
        yield return StartCoroutine(base.StartTurn());

        skillsManager.SetSkills(this);
        consumableManager.SetConsumables(this);

        // Refresh icons now that energy has been restored and charges have been reset.
        uiManager.SetSkillIcons(this);
        uiManager.SetConsumableIcons(this);

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