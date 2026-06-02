using UnityEngine;

[CreateAssetMenu(fileName = "Duality_Trait", menuName = "ScriptableObjects/Traits/Duality_Trait")]
public class Duality_Trait : SO_Trait
{
    public int PowerBonus = 2;

    public override void Init(Character character, Trait trait)
    {
        int lastApplied = 0; // 0 = none, 1 = applied

        character.OnUnitTurnStartEvent.AddListener(OnTurnStart);

        void OnTurnStart()
        {
            // Remove the previous bonus before applying the new one
            if (lastApplied != 0)
                character.Power -= PowerBonus;

            // Apply Power bonus each turn
            character.Power += PowerBonus;
            lastApplied = 1;
        }
    }
}
