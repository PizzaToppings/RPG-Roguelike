using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Data-driven anomaly. Configure entirely in the Inspector.
/// Anomalies affect all characters, all enemies, all units, or the board state.
/// For complex behaviour create a custom SO_Anomaly subclass instead.
/// </summary>
[CreateAssetMenu(fileName = "Anomaly", menuName = "ScriptableObjects/Anomalies/DefaultAnomaly")]
public class DefaultAnomaly : SO_Anomaly
{
    [Header("Trigger")]
    public AnomalyTriggerEnum TriggerMoment;
    public bool TriggerOnce;

    [Min(1)]
    public int ChargesToTrigger = 1;

    [Header("Target")]
    public AnomalyTargetEnum Target;

    [Header("Effect")]
    public AnomalyEffectEnum TriggerEffect;

    [Tooltip("General-purpose value (damage, stat delta, etc.)")]
    public int Value = 1;

    [Header("Damage (for DealDamage effect)")]
    public HitTypeEnum HitType;

    [Header("Status Effects (for AddStatusEffect effect)")]
    public List<SO_StatusEffect> StatusEffects;

    [Header("Stat (for ModifyStat effect)")]
    public List<StatsEnum> Stat;

    public override void Init(Anomaly anomaly)
    {
        Debug.Log($"[Anomaly] '{AnomalyName}' Init � Trigger: {TriggerMoment}, Target: {Target}, Effect: {TriggerEffect}");

        switch (TriggerMoment)
        {
            case AnomalyTriggerEnum.OnCombatStart:
                ApplyEffect(anomaly);
                break;

            case AnomalyTriggerEnum.StartOfRound:
                CombatData.onRoundStart.AddListener(() => OnTrigger(anomaly));
                break;

            case AnomalyTriggerEnum.EndOfRound:
                CombatData.onRoundEnd.AddListener(() => OnTrigger(anomaly));
                break;

            case AnomalyTriggerEnum.OnCombatEnd:
                CombatData.onCombatEnd.AddListener(() => OnTrigger(anomaly));
                break;
        }
    }

    void OnTrigger(Anomaly anomaly)
    {
        if (TriggerOnce && anomaly.hasTriggered) return;

        if (ChargesToTrigger > 1)
        {
            anomaly.chargeCount++;
            if (anomaly.chargeCount < ChargesToTrigger) return;
            anomaly.chargeCount = 0;
        }

        ApplyEffect(anomaly);
        anomaly.hasTriggered = true;
    }

    void ApplyEffect(Anomaly anomaly)
    {
        Debug.Log($"[Anomaly] '{AnomalyName}' applying effect '{TriggerEffect}' to '{Target}'");

        var targets = GetTargets();

        switch (TriggerEffect)
        {
            case AnomalyEffectEnum.AddStatusEffect:
                foreach (var se in StatusEffects)
                    StatusEffectManager.Instance.ApplyStatusEffect(se, targets);
                break;

            case AnomalyEffectEnum.DealDamage:
                var damageManager = DamageManager.Instance;
                foreach (var target in targets)
                {
                    var damageData = new DamageData { HitType = HitType, Power = Value };
                    var calculated = damageManager.CalculateDamageData(damageData, target);
                    if (HitType == HitTypeEnum.Healing)
                        damageManager.HealUnit(calculated);
                    else if (HitType == HitTypeEnum.Shield)
                        damageManager.ShieldUnit(calculated);
                    else
                        damageManager.DealDamage(calculated);
                }
                break;

            case AnomalyEffectEnum.ModifyStat:
                foreach (var target in targets)
                    foreach (var stat in Stat)
                        ApplyStatChange(target, stat, Value);
                break;
        }
    }

    List<Unit> GetTargets()
    {
        switch (Target)
        {
            case AnomalyTargetEnum.AllCharacters:
                return UnitData.Characters.Cast<Unit>().ToList();

            case AnomalyTargetEnum.AllEnemies:
                return UnitData.Enemies.Cast<Unit>().ToList();

            case AnomalyTargetEnum.AllUnits:
                return new List<Unit>(UnitData.Units);

            default:
                return new List<Unit>();
        }
    }

    void ApplyStatChange(Unit target, StatsEnum stat, int value)
    {
        switch (stat)
        {
            case StatsEnum.Power:        target.Power        += value; break;
            case StatsEnum.Armor:        target.Armor      += value; break;
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
