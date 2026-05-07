using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Trinket", menuName = "ScriptableObjects/Trinkets/DefaultTrinket")]
public class DefaultTrinket : SO_Trinket
{
    public TriggerMomentEnum TriggerMoment;
    public TriggerEffectEnum TriggerEffect;
    public int ChargesToTrigger;
    public bool TriggerOnce;

    [Space]
    public int Power;
    public DamageTypeEnum DamageType;

    [Space]
    public TargetEnum Target;
    public float Range;

    [Space]
    public StatusEffectEnum StatusEffect;
    public SO_StatusEffect StatusEffectSO;
    public List<StatsEnum> Stat;

    public override void Init(Character character, Trinket trinket)
    {
        switch (TriggerMoment)
        {
            case TriggerMomentEnum.StartOfGame:
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
            case TriggerMomentEnum.OnDealDamage:
                if (character.OnDealDamage == null)
                    character.OnDealDamage = new UnityEngine.Events.UnityEvent<DamageDataCalculated>();
                character.OnDealDamage.AddListener(_ => OnTrigger(character, trinket));
                break;
            case TriggerMomentEnum.OnTakeDamage:
                if (character.OnTakeDamage == null)
                    character.OnTakeDamage = new UnityEngine.Events.UnityEvent<DamageDataCalculated>();
                character.OnTakeDamage.AddListener(_ => OnTrigger(character, trinket));
                break;
        }
    }

    private void OnTrigger(Character character, Trinket trinket)
    {
        if (TriggerOnce && trinket.hasTriggered) return;

        if (ChargesToTrigger > 1)
        {
            trinket.chargeCount++;
            if (trinket.chargeCount < ChargesToTrigger) return;
            trinket.chargeCount = 0;
        }

        Trigger(character);
        trinket.hasTriggered = true;
    }

    private void Trigger(Character character)
    {
        switch (TriggerEffect)
        {
            case TriggerEffectEnum.DealDamage:
                var damageData = new DamageData { Caster = character, DamageType = DamageType, Power = Power };
                var damageManager = DamageManager.Instance;
                foreach (var target in GetTargets(character))
                    damageManager.DealDamage(damageManager.CalculateDamageData(damageData, target));
                break;

            case TriggerEffectEnum.TakeDamage:
                var selfDamageData = new DamageData { Caster = character, DamageType = DamageType, Power = Power };
                var selfDamageManager = DamageManager.Instance;
                selfDamageManager.DealDamage(selfDamageManager.CalculateDamageData(selfDamageData, character));
                break;

            case TriggerEffectEnum.AddStatusEffect:
                if (StatusEffectSO == null) break;
                StatusEffectManager.Instance.ApplyStatusEffect(StatusEffectSO, GetTargets(character));
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
                // TODO: find closest enemy within Range using BoardManager
                return new List<Unit>();
            case TargetEnum.LowestHealthTarget:
                // TODO: find lowest health enemy within Range using BoardManager
                return new List<Unit>();
            default:
                return new List<Unit>();
        }
    }
}
