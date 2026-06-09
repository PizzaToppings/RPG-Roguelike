using UnityEngine;

[CreateAssetMenu(fileName = "Duality_Trait", menuName = "ScriptableObjects/Traits/Duality_Trait")]
public class Duality_Trait : SO_Trait
{
    public int PowerBonus = 2;

    public override void Init(Character character, Trait trait)
    {
        // Track the currently applied status effect so it can be removed next turn
        StatChangeEffect currentlyApplied = null;

        character.OnUnitTurnStartEvent.AddListener(OnTurnStart);

        void OnTurnStart()
        {
            // Remove previous effect if present
            if (currentlyApplied != null)
            {
                currentlyApplied.EndEffect();
                currentlyApplied = null;
            }

            // Decide whether this turn grants magical or physical bonus.
            // We use the character's PendingCombatStyle parity as a simple alternator
            // Fallback: alternate based on combat round via CombatData.onRoundStart? Keep simple: alternate each turn by toggling a flag stored in trait instance.
            // Use trait.chargeCount as a persistent toggle counter.
            trait.chargeCount++;
            bool giveMagical = trait.chargeCount % 2 == 0;

            // Create and apply a StatChangeEffect so it appears in the info panel and has proper duration handling.
            var effect = new StatChangeEffect
            {
                IsBuff = true,
                statusEfectType = StatusEffectEnum.StatChange,
                Duration = 1,
                IsPermanent = false,
                DurationTrigger = TriggerMomentEnum.EndOfTurn,
                Description = giveMagical ? $"+{PowerBonus} Magical Power" : $"+{PowerBonus} Physical Power",
                Caster = character,
                Target = character,
                Stat = StatsEnum.Power,
                Power = PowerBonus,
                SourceCombatStyle = CombatStyle.None,
                SuppressFloating = false,
                HideInInfoPanel = false,
                UseCasterTurnForDuration = false,
                DurationOwner = DurationOwnerEnum.Target
            };

            // Apply the effect (this will modify the stat immediately and register duration)
            effect.Apply();

            currentlyApplied = effect;
        }
    }
}
