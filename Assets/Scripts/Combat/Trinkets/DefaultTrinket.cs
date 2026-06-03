using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Trait", menuName = "ScriptableObjects/Traits/DefaultTrait")]
public class DefaultTrait : SO_Trait
{
    public TriggerMomentEnum TriggerMoment;
    public TriggerEffectEnum TriggerEffect;
    
    [Min(1)]
    public int ChargesToTrigger = 1;
    public bool TriggerOnce;

    [Space]
    public int Value = 1;
    
    [Space]
    public HitTypeEnum HitType;

    [Space]
    [Tooltip("DEPRECATED: Use TargetFaction and TargetSelection instead")]
    public TraitTargetFactionEnum TargetFaction;
    public TraitTargetSelectionEnum TargetSelection;
    public float Range;

    [Space]
    public List<SO_StatusEffect> StatusEffects;
    public List<StatsEnum> Stat;

    [Space]
    public CombatStyle RequiredSkillStyle = CombatStyle.None;

    public override void Init(Character character, Trait trait)
    {
        switch (TriggerMoment)
        {
            case TriggerMomentEnum.Instant:
                var partyMember = character.partyMemberIndex < RunData.Party.Count
                    ? RunData.Party[character.partyMemberIndex]
                    : null;

                // Only apply once per run; guard against re-init on subsequent combats
                if (partyMember == null || !partyMember.AppliedInstantTraits.Contains(TraitName))
                {
                    if (partyMember != null)
                    {
                        partyMember.AppliedInstantTraits.Add(TraitName);

                        // Persist MaxHitpoints / MaxEnergy bonuses in RunData so they carry over each combat
                        if (TriggerEffect == TriggerEffectEnum.ModifyStat)
                        {
                            foreach (var stat in Stat)
                            {
                                if (stat == StatsEnum.MaxHitpoints) partyMember.BonusMaxHitpoints += Value;
                                // else if (stat == StatsEnum.MaxEnergy) partyMember.BonusMaxEnergy += Value;
                            }
                        }
                    }

                    // Apply the effect immediately on the character (SetStats already ran with old bonuses)
                    Trigger(character);
                    trait.hasTriggered = true;

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

                // Refresh the healthbar — InitTraits runs after healthCanvas.Init() so the bar
                // was drawn with pre-trinket stats and needs an explicit update
                if (character.ThisHealthbar != null)
                    character.ThisHealthbar.UpdateHealthbar();
                break;
            case TriggerMomentEnum.StartOfCombat:
                Trigger(character);
                break;
            case TriggerMomentEnum.StartOfTurn:
                character.OnUnitTurnStartEvent.AddListener(() => OnTrigger(character, trait));
                break;
            case TriggerMomentEnum.EndOfTurn:
                character.OnUnitTurnEndEvent.AddListener(() => OnTrigger(character, trait));
                break;
            case TriggerMomentEnum.StartOfRound:
                CombatData.onRoundStart.AddListener(() => OnTrigger(character, trait));
                break;
            case TriggerMomentEnum.EndOfRound:
                CombatData.onRoundEnd.AddListener(() => OnTrigger(character, trait));
                break;
            case TriggerMomentEnum.EndOfCombat:
                CombatData.onCombatEnd.AddListener(() => OnTrigger(character, trait));
                break;
            case TriggerMomentEnum.OnDealDamage:
                if (character.OnDealDamage == null)
                    character.OnDealDamage = new UnityEngine.Events.UnityEvent<DamageDataCalculated>();
                character.OnDealDamage.AddListener(data => OnDealDamageTrigger(character, trait, data));
                break;
            case TriggerMomentEnum.OnTakeDamage:
                if (character.OnTakeDamage == null)
                    character.OnTakeDamage = new UnityEngine.Events.UnityEvent<DamageDataCalculated>();
                character.OnTakeDamage.AddListener(_ => OnTrigger(character, trait));
                break;
            case TriggerMomentEnum.OnKillEnemy:
                character.OnKillEnemyEvent.AddListener(_ => OnTrigger(character, trait));
                break;
            case TriggerMomentEnum.OnUseAbility:
                character.OnSkillCastEvent.AddListener(skill => OnSkillUseTrigger(character, trait, skill));
                break;
        }
    }

    private void OnTrigger(Character character, Trait trait)
    {
        if (TriggerOnce && trait.hasTriggered)
        {
            return;
        }

        if (ChargesToTrigger > 1)
        {
            trait.chargeCount++;
            if (trait.chargeCount < ChargesToTrigger) return;
            trait.chargeCount = 0;
        }

        Trigger(character);
        trait.hasTriggered = true;
    }

    private void OnSkillUseTrigger(Character character, Trait trait, Skill skill)
    {
        // Check if the skill's combat style matches the required style (None = any style)
        if (RequiredSkillStyle != CombatStyle.None && skill.mainSkillSO.SkillCombatStyle != RequiredSkillStyle)
        {
            return;
        }

        OnTrigger(character, trait);
    }

    private void OnDealDamageTrigger(Character character, Trait trait, DamageDataCalculated data)
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
            if (TriggerOnce && trait.hasTriggered)
                return;

            if (ChargesToTrigger > 1)
            {
                trait.chargeCount++;
                if (trait.chargeCount < ChargesToTrigger) return;
                trait.chargeCount = 0;
            }

            // Apply status effects to the damaged target
            foreach (var statusEffect in StatusEffects)
            {
                StatusEffectManager.Instance.ApplyStatusEffect(statusEffect, new List<Unit> { data.Target });
            }

            trait.hasTriggered = true;
        }
        else
        {
            // For other effects, use standard trigger
            OnTrigger(character, trait);
        }
    }

    private void Trigger(Character character)
    {
        switch (TriggerEffect)
        {
            case TriggerEffectEnum.DealDamage:
                var damageData = new DamageData { Caster = character, HitType = HitType, Power = Value };
                var damageManager = DamageManager.Instance;
                foreach (var target in GetTargets(character))
                {
                    var calculated = damageManager.CalculateDamageData(damageData, target);
                    if (HitType == HitTypeEnum.Healing)
                        damageManager.HealUnit(calculated);
                    else if (HitType == HitTypeEnum.Shield)
                        damageManager.ShieldUnit(calculated);
                    else
                        damageManager.DealDamage(calculated);
                }
                break;

            case TriggerEffectEnum.AddStatusEffect:
                foreach (var statusEffect in StatusEffects)
                    StatusEffectManager.Instance.ApplyStatusEffect(statusEffect, GetTargets(character));
                break;

            case TriggerEffectEnum.AddEnergy:
                //character.SetEnergy(character.Energy + Value);
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
        // Get the base pool of targets based on faction
        List<Unit> targetPool = GetTargetPool(character);

        // Apply selection logic to the pool
        return SelectFromPool(targetPool, character);
    }

    private List<Unit> GetTargetPool(Character character)
    {
        switch (TargetFaction)
        {
            case TraitTargetFactionEnum.Friendly:
                return UnitData.Characters.Cast<Unit>().ToList();

            case TraitTargetFactionEnum.Enemy:
                return UnitData.Enemies.Cast<Unit>().ToList();

            case TraitTargetFactionEnum.All:
                return new List<Unit>(UnitData.Units);

            default:
                return new List<Unit>();
        }
    }

    private List<Unit> SelectFromPool(List<Unit> pool, Character character)
    {
        // Apply range filtering first
        pool = FilterByRange(pool, character);

        if (pool.Count == 0)
            return pool;

        switch (TargetSelection)
        {
            case TraitTargetSelectionEnum.Self:
                return new List<Unit> { character };

            case TraitTargetSelectionEnum.Closest:
                return new List<Unit> { pool.OrderBy(u => BoardManager.Instance.GetRangeBetweenTiles(character.Tile, u.Tile)).First() };

            case TraitTargetSelectionEnum.LowestHealth:
                return new List<Unit> { pool.OrderBy(u => u.Hitpoints).First() };

            case TraitTargetSelectionEnum.Area:
                return pool;

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
            case StatsEnum.Power:        target.Power        += value; break;
            case StatsEnum.Defense:      target.Defense      += value; break;
            case StatsEnum.Shield:       target.ShieldPoints += value; break;
            case StatsEnum.Range:        target.Range        += value; break;
            case StatsEnum.MoveSpeed:    target.MoveSpeed    += value; break;
            case StatsEnum.MaxHitpoints: target.MaxHitpoints += value; break;
            //case StatsEnum.MaxEnergy:
            //    var character = target as Character;
            //    if (character != null) character.MaxEnergy += value;
            //    break;
        }
    }
}
