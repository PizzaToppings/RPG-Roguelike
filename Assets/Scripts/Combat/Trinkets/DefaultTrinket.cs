using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Trinket", menuName = "ScriptableObjects/Trinkets/DefaultTrinket")]
public class DefaultTrinket : SO_Trinket
{
    public TriggerMomentEnum TriggerMoment;
    public TriggerEffectEnum TriggerEffect;
    
    [Min(1)]
    public int ChargesToTrigger = 1;
    public bool TriggerOnce;

    [Space]
    public int Value = 1;
    
    [Space]
    public DamageTypeEnum DamageType;
    public bool IsMagical;

    [Space]
    public TargetEnum Target;
    public float Range;

    [Space]
    public List<SO_StatusEffect> StatusEffects;
    public List<StatsEnum> Stat;

    [Space]
    public CombatStyle RequiredSkillStyle = CombatStyle.None;

    public override void Init(Character character, Trinket trinket)
    {
        switch (TriggerMoment)
        {
            case TriggerMomentEnum.Instant:
                var partyMember = character.partyMemberIndex < RunData.Party.Count
                    ? RunData.Party[character.partyMemberIndex]
                    : null;

                // Only apply once per run; guard against re-init on subsequent combats
                if (partyMember == null || !partyMember.AppliedInstantTrinkets.Contains(TrinketName))
                {
                    if (partyMember != null)
                    {
                        partyMember.AppliedInstantTrinkets.Add(TrinketName);

                        // Persist MaxHitpoints / MaxEnergy bonuses in RunData so they carry over each combat
                        if (TriggerEffect == TriggerEffectEnum.ModifyStat)
                        {
                            foreach (var stat in Stat)
                            {
                                if (stat == StatsEnum.MaxHitpoints) partyMember.BonusMaxHitpoints += Value;
                                else if (stat == StatsEnum.MaxEnergy) partyMember.BonusMaxEnergy += Value;
                            }
                        }
                    }

                    // Apply the effect immediately on the character (SetStats already ran with old bonuses)
                    Trigger(character);
                    trinket.hasTriggered = true;

                    // When MaxHP increases, also heal the character for that amount
                    if (TriggerEffect == TriggerEffectEnum.ModifyStat)
                    {
                        foreach (var stat in Stat)
                        {
                            if (stat == StatsEnum.MaxHitpoints)
                            {
                                character.Hitpoints = Mathf.Min(character.Hitpoints + Value, character.MaxHitpoints);
                                if (partyMember != null)
                                    partyMember.CurrentHitpoints = character.Hitpoints;
                            }
                        }
                    }
                }

                // Refresh the healthbar — InitTrinkets runs after healthCanvas.Init() so the bar
                // was drawn with pre-trinket stats and needs an explicit update
                if (character.ThisHealthbar != null)
                    character.ThisHealthbar.UpdateHealthbar();
                break;
            case TriggerMomentEnum.StartOfCombat:
                Trigger(character);
                break;
            case TriggerMomentEnum.StartOfTurn:
                character.OnUnitTurnStartEvent.AddListener(() => OnTrigger(character, trinket));
                break;
            case TriggerMomentEnum.EndOfTurn:
                character.OnUnitTurnEndEvent.AddListener(() => OnTrigger(character, trinket));
                break;
            case TriggerMomentEnum.StartOfRound:
                CombatData.onRoundStart.AddListener(() => OnTrigger(character, trinket));
                break;
            case TriggerMomentEnum.EndOfRound:
                CombatData.onRoundEnd.AddListener(() => OnTrigger(character, trinket));
                break;
            case TriggerMomentEnum.EndOfCombat:
                CombatData.onCombatEnd.AddListener(() => OnTrigger(character, trinket));
                break;
            case TriggerMomentEnum.OnDealDamage:
                if (character.OnDealDamage == null)
                    character.OnDealDamage = new UnityEngine.Events.UnityEvent<DamageDataCalculated>();
                character.OnDealDamage.AddListener(data => OnDealDamageTrigger(character, trinket, data));
                break;
            case TriggerMomentEnum.OnTakeDamage:
                if (character.OnTakeDamage == null)
                    character.OnTakeDamage = new UnityEngine.Events.UnityEvent<DamageDataCalculated>();
                character.OnTakeDamage.AddListener(_ => OnTrigger(character, trinket));
                break;
            case TriggerMomentEnum.OnKillEnemy:
                character.OnKillEnemyEvent.AddListener(_ => OnTrigger(character, trinket));
                break;
            case TriggerMomentEnum.OnUseAbility:
                character.OnSkillCastEvent.AddListener(skill => OnSkillUseTrigger(character, trinket, skill));
                break;
        }
    }

    private void OnTrigger(Character character, Trinket trinket)
    {
        if (TriggerOnce && trinket.hasTriggered)
        {
            return;
        }

        if (ChargesToTrigger > 1)
        {
            trinket.chargeCount++;
            if (trinket.chargeCount < ChargesToTrigger) return;
            trinket.chargeCount = 0;
        }

        Trigger(character);
        trinket.hasTriggered = true;
    }

    private void OnSkillUseTrigger(Character character, Trinket trinket, Skill skill)
    {
        // Check if the skill's combat style matches the required style (None = any style)
        if (RequiredSkillStyle != CombatStyle.None && skill.mainSkillSO.SkillCombatStyle != RequiredSkillStyle)
        {
            return;
        }

        OnTrigger(character, trinket);
    }

    private void OnDealDamageTrigger(Character character, Trinket trinket, DamageDataCalculated data)
    {
        // Check if the skill's combat style matches the required style (None = any style)
        if (RequiredSkillStyle != CombatStyle.None && character.CurrentCombatStyle != RequiredSkillStyle)
        {
            return;
        }

        // For OnDealDamage triggers with status effects, apply to the damaged target
        if (TriggerEffect == TriggerEffectEnum.AddStatusEffect && data.Target != null)
        {
            // Check charges/trigger once before applying
            if (TriggerOnce && trinket.hasTriggered)
                return;

            if (ChargesToTrigger > 1)
            {
                trinket.chargeCount++;
                if (trinket.chargeCount < ChargesToTrigger) return;
                trinket.chargeCount = 0;
            }

            // Apply status effects to the damaged target
            foreach (var statusEffect in StatusEffects)
            {
                StatusEffectManager.Instance.ApplyStatusEffect(statusEffect, new List<Unit> { data.Target }, Value);
            }

            trinket.hasTriggered = true;
        }
        else
        {
            // For other effects, use standard trigger
            OnTrigger(character, trinket);
        }
    }

    private void Trigger(Character character)
    {
        switch (TriggerEffect)
        {
            case TriggerEffectEnum.DealDamage:
                var damageData = new DamageData { Caster = character, DamageType = DamageType, Power = Value, IsMagical = IsMagical };
                var damageManager = DamageManager.Instance;
                foreach (var target in GetTargets(character))
                {
                    var calculated = damageManager.CalculateDamageData(damageData, target);
                    if (DamageType == DamageTypeEnum.Healing)
                        damageManager.HealUnit(calculated);
                    else if (DamageType == DamageTypeEnum.Shield)
                        damageManager.ShieldUnit(calculated);
                    else
                        damageManager.DealDamage(calculated);
                }
                break;

            case TriggerEffectEnum.AddStatusEffect:
                foreach (var statusEffect in StatusEffects)
                    StatusEffectManager.Instance.ApplyStatusEffect(statusEffect, GetTargets(character), Value);
                break;

            case TriggerEffectEnum.AddEnergy:
                character.SetEnergy(character.Energy + Value);
                break;

            case TriggerEffectEnum.ModifyStat:
                foreach (var target in GetTargets(character))
                    foreach (var stat in Stat)
                        ApplyStatChange(target, stat, Value);
                break;
        }
    }

    private List<Unit> GetTargets(Character character)
    {
        switch (Target)
        {
            case TargetEnum.Self:
                return new List<Unit> { character };

            case TargetEnum.closestTarget:
                var closestPool = FilterByRange(UnitData.Enemies.Cast<Unit>().ToList(), character);
                if (closestPool.Count == 0) return closestPool;
                return new List<Unit> { closestPool.OrderBy(u => BoardManager.Instance.GetRangeBetweenTiles(character.Tile, u.Tile)).First() };

            case TargetEnum.LowestHealthTarget:
                var lowestPool = FilterByRange(UnitData.Enemies.Cast<Unit>().ToList(), character);
                if (lowestPool.Count == 0) return lowestPool;
                return new List<Unit> { lowestPool.OrderBy(u => u.Hitpoints).First() };

            case TargetEnum.AllAllies:
                return FilterByRange(UnitData.Characters.Cast<Unit>().ToList(), character);

            case TargetEnum.AllEnemies:
                return FilterByRange(UnitData.Enemies.Cast<Unit>().ToList(), character);

            case TargetEnum.AllUnits:
                return FilterByRange(UnitData.Units, character);

            default:
                return new List<Unit>();
        }
    }

    private List<Unit> FilterByRange(List<Unit> units, Character character)
    {
        if (Range <= 0) return new List<Unit>(units);
        return units.Where(u => BoardManager.Instance.GetRangeBetweenTiles(character.Tile, u.Tile) <= Range).ToList();
    }

    private void ApplyStatChange(Unit target, StatsEnum stat, int value)
    {
        switch (stat)
        {
            case StatsEnum.PhysicalPower:   target.PhysicalPower   += value; break;
            case StatsEnum.MagicalPower:    target.MagicalPower    += value; break;
            case StatsEnum.PhysicalDefense: target.PhysicalDefense += value; break;
            case StatsEnum.MagicalDefense:  target.MagicalDefense  += value; break;
            case StatsEnum.MoveSpeed:       target.MoveSpeed       += value; break;
            case StatsEnum.MaxHitpoints:    target.MaxHitpoints    += value; break;
            case StatsEnum.MaxEnergy:
                var character = target as Character;
                if (character != null) character.MaxEnergy += value;
                break;
        }
    }
}
