using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Data-driven enemy ability. Configure entirely in the Inspector —
/// no code subclass required for common triggered effects.
///
/// For complex behaviour (e.g. multi-phase, shared health pools) create a
/// custom SO_EnemyAbility subclass instead.
/// </summary>
[CreateAssetMenu(fileName = "EnemyAbility", menuName = "ScriptableObjects/Enemies/DefaultEnemyAbility")]
public class DefaultEnemyAbility : SO_EnemyAbility
{
    [Header("Trigger")]
    public EnemyTriggerMomentEnum TriggerMoment;

    /// <summary>Number of trigger-moment events before the effect fires. 1 = every time.</summary>
    public int  ChargesToTrigger = 1;
    public bool TriggerOnce;

    [Header("Effect")]
    public TriggerEffectEnum TriggerEffect;
    public int Value = 1;

    [Header("Damage")]
    public HitTypeEnum HitType;

    [Header("Targeting")]
    public TargetEnum Target;
    /// <summary>0 = unlimited range.</summary>
    public float Range;

    [Header("Status Effects")]
    public List<SO_StatusEffect> StatusEffects;

    [Header("Stat Changes")]
    public List<StatsEnum> Stats;

    // ─────────────────────────────────────────────────────────────────

    public override void Init(Enemy enemy, EnemyAbility ability)
    {
        switch (TriggerMoment)
        {
            case EnemyTriggerMomentEnum.StartOfCombat:
                Trigger(enemy, ability);
                break;

            case EnemyTriggerMomentEnum.StartOfTurn:
                enemy.OnUnitTurnStartEvent.AddListener(() => OnTrigger(enemy, ability));
                break;

            case EnemyTriggerMomentEnum.EndOfTurn:
                enemy.OnUnitTurnEndEvent.AddListener(() => OnTrigger(enemy, ability));
                break;

            case EnemyTriggerMomentEnum.StartOfRound:
                CombatData.onRoundStart.AddListener(() => OnTrigger(enemy, ability));
                break;

            case EnemyTriggerMomentEnum.EndOfRound:
                CombatData.onRoundEnd.AddListener(() => OnTrigger(enemy, ability));
                break;

            case EnemyTriggerMomentEnum.OnDealDamage:
                if (enemy.OnDealDamage == null)
                    enemy.OnDealDamage = new UnityEngine.Events.UnityEvent<DamageDataCalculated>();
                enemy.OnDealDamage.AddListener(_ => OnTrigger(enemy, ability));
                break;

            case EnemyTriggerMomentEnum.OnTakeDamage:
                if (enemy.OnTakeDamage == null)
                    enemy.OnTakeDamage = new UnityEngine.Events.UnityEvent<DamageDataCalculated>();
                enemy.OnTakeDamage.AddListener(_ => OnTrigger(enemy, ability));
                break;

            case EnemyTriggerMomentEnum.OnDeath:
                enemy.OnDeathEvent.AddListener(() => OnTrigger(enemy, ability));
                break;
        }
    }

    // ─────────────────────────────────────────────────────────────────

    void OnTrigger(Enemy enemy, EnemyAbility ability)
    {
        if (TriggerOnce && ability.hasTriggered) return;

        if (ChargesToTrigger > 1)
        {
            ability.chargeCount++;
            if (ability.chargeCount < ChargesToTrigger) return;
            ability.chargeCount = 0;
        }

        Trigger(enemy, ability);

        if (TriggerOnce)
            ability.hasTriggered = true;
    }

    void Trigger(Enemy enemy, EnemyAbility ability)
    {
        var targets = GetTargets(enemy);
        var dm      = DamageManager.Instance;

        foreach (var target in targets)
        {
            switch (TriggerEffect)
            {
                case TriggerEffectEnum.DealDamage:
                    var data       = new DamageData { Caster = enemy, HitType = HitType, Power = Value };
                    var calculated = dm.CalculateDamageData(data, target);
                    if (HitType == HitTypeEnum.Healing)
                        dm.HealUnit(calculated);
                    else if (HitType == HitTypeEnum.Shield)
                        dm.ShieldUnit(calculated);
                    else
                        dm.DealDamage(calculated);
                    break;

                case TriggerEffectEnum.AddStatusEffect:
                    foreach (var se in StatusEffects)
                        StatusEffectManager.Instance.ApplyStatusEffect(se, new List<Unit> { target }, Value);
                    break;

                case TriggerEffectEnum.ModifyStat:
                    foreach (var stat in Stats)
                        ApplyStatChange(target, stat, Value);
                    break;
            }
        }
    }

    // ─────────────────────────────────────────────────────────────────

    List<Unit> GetTargets(Enemy enemy)
    {
        switch (Target)
        {
            case TargetEnum.Self:
                return new List<Unit> { enemy };

            case TargetEnum.closestTarget:
                var closestPool = FilterByRange(UnitData.Characters.Cast<Unit>().ToList(), enemy);
                if (closestPool.Count == 0) return closestPool;
                return new List<Unit> { closestPool.OrderBy(u => BoardManager.Instance.GetRangeBetweenTiles(enemy.Tile, u.Tile)).First() };

            case TargetEnum.LowestHealthTarget:
                var lowestPool = FilterByRange(UnitData.Characters.Cast<Unit>().ToList(), enemy);
                if (lowestPool.Count == 0) return lowestPool;
                return new List<Unit> { lowestPool.OrderBy(u => u.Hitpoints).First() };

            case TargetEnum.AllAllies:
                // From an enemy's perspective, allies are other enemies
                return FilterByRange(UnitData.Enemies.Cast<Unit>().ToList(), enemy);

            case TargetEnum.AllEnemies:
                // From an enemy's perspective, enemies are the player characters
                return FilterByRange(UnitData.Characters.Cast<Unit>().ToList(), enemy);

            case TargetEnum.AllUnits:
                return FilterByRange(UnitData.Units, enemy);

            default:
                return new List<Unit>();
        }
    }

    List<Unit> FilterByRange(List<Unit> units, Enemy enemy)
    {
        if (Range <= 0) return new List<Unit>(units);
        return units.Where(u => BoardManager.Instance.GetRangeBetweenTiles(enemy.Tile, u.Tile) <= Range).ToList();
    }

    void ApplyStatChange(Unit target, StatsEnum stat, int value)
    {
        switch (stat)
        {
            case StatsEnum.Power:        target.Power        += value; break;
            case StatsEnum.Defense:      target.Defense      += value; break;
            case StatsEnum.Shield:       target.ShieldPoints += value; break;
            case StatsEnum.Range:        target.Range        += value; break;
            case StatsEnum.MoveSpeed:    target.MoveSpeed    += value; break;
            case StatsEnum.MaxHitpoints: target.MaxHitpoints += value; break;
        }
    }
}
