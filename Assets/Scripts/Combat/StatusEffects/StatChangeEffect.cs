using UnityEngine;

public class StatChangeEffect : StatusEffect
{
    public StatsEnum Stat;
    public int Power;
    public CooldownVariant CooldownTarget = CooldownVariant.Active;
    // Origin style for grouping in info panels
    public CombatStyle SourceCombatStyle = CombatStyle.None;
    // When true and Stat == Power, apply to the typed power bonus instead of the generic Power value.
    public bool ApplyToPowerType = false;
    // When ApplyToPowerType == true, this selects magical (true) or physical (false) bonus.
    public bool IsMagicalPower = false;

    public override void Apply()
    {
        // Let the base handle registration and duration subscription
        base.Apply();

        // Apply the stat change immediately
        ChangeStat();
    }

    public void ChangeStat()
    {
        switch (Stat)
        {
            case StatsEnum.Power:
                if (ApplyToPowerType)
                {
                    if (IsMagicalPower)
                        Target.MagicalPowerBonus += Power;
                    else
                        Target.PhysicalPowerBonus += Power;
                }
                else
                {
                    Target.Power += Power;
                }
                break;
            case StatsEnum.Armor:
                Target.Armor += Power;
                break;
            case StatsEnum.Shield:
                Target.ShieldPoints += Power;
                break;
            case StatsEnum.Range:
                Target.Range += Power;
                break;
            case StatsEnum.MoveSpeed:
                Target.MoveSpeed += Power;
                break;
            case StatsEnum.MaxHitpoints:
                Target.MaxHitpoints += Power;
                break;
            case StatsEnum.Cooldown:
                // Apply cooldown delta to all target's skills (reduce positive cooldowns by negative Power etc.)
                if (Target is Character c)
                {
                    var all = new System.Collections.Generic.List<Skill>();
                    if (c.basicAttack != null) all.Add(c.basicAttack);
                    if (c.basicSkill != null) all.Add(c.basicSkill);
                    if (c.skills != null) all.AddRange(c.skills);
                    if (c.consumables != null) all.AddRange(c.consumables);

                    foreach (var s in all)
                    {
                        if (s == null) continue;
                        if (CooldownTarget == CooldownVariant.Active || CooldownTarget == CooldownVariant.Both)
                        {
                            int current = SkillData.GetCooldown(s);
                            SkillData.SetCooldown(s, Mathf.Max(0, current + Power));
                        }
                        if (CooldownTarget == CooldownVariant.Default || CooldownTarget == CooldownVariant.Both)
                        {
                            s.DefaultCooldown = Mathf.Max(0, s.DefaultCooldown + Power);
                        }
                    }
                }
                break;
            //case StatsEnum.MaxEnergy:
            //    var character = Target as Character;
            //    character.MaxEnergy += Power;
            //    break;
        }
    }

    public void RemoveStat()
    {
        switch (Stat)
        {
            case StatsEnum.Power:
                if (ApplyToPowerType)
                {
                    if (IsMagicalPower)
                        Target.MagicalPowerBonus -= Power;
                    else
                        Target.PhysicalPowerBonus -= Power;
                }
                else
                {
                    Target.Power -= Power;
                }
                break;
            case StatsEnum.Armor:
                Target.Armor -= Power;
                break;
            case StatsEnum.Shield:
                Target.ShieldPoints -= Power;
                if (Target.ShieldPoints < 0) Target.ShieldPoints = 0;
                break;
            case StatsEnum.Range:
                Target.Range -= Power;
                break;
            case StatsEnum.MoveSpeed:
                Target.MoveSpeed -= Power;
                break;
            case StatsEnum.MaxHitpoints:
                Target.MaxHitpoints -= Power;
                break;
            case StatsEnum.Cooldown:
                if (Target is Character c)
                {
                    var all = new System.Collections.Generic.List<Skill>();
                    if (c.basicAttack != null) all.Add(c.basicAttack);
                    if (c.basicSkill != null) all.Add(c.basicSkill);
                    if (c.skills != null) all.AddRange(c.skills);
                    if (c.consumables != null) all.AddRange(c.consumables);

                    foreach (var s in all)
                    {
                        if (s == null) continue;
                        if (CooldownTarget == CooldownVariant.Active || CooldownTarget == CooldownVariant.Both)
                        {
                            int current = SkillData.GetCooldown(s);
                            SkillData.SetCooldown(s, Mathf.Max(0, current - Power));
                        }
                        if (CooldownTarget == CooldownVariant.Default || CooldownTarget == CooldownVariant.Both)
                        {
                            s.DefaultCooldown = Mathf.Max(0, s.DefaultCooldown - Power);
                        }
                    }
                }
                break;
            //case StatsEnum.MaxEnergy:
            //    var character = Target as Character;
            //    character.MaxEnergy -= Power;
            //    break;
        }
    }

    public override void EndEffect()
    {
        base.EndEffect();

        RemoveStat();
        UnsubscribeDurationTrigger();
    }
}