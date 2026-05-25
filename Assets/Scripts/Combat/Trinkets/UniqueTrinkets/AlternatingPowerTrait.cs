using UnityEngine;

[CreateAssetMenu(fileName = "Duality_Trait", menuName = "ScriptableObjects/Traits/Duality_Trait")]
public class Duality_Trait : SO_Trait
{
    public int PowerBonus = 2;

    public override void Init(Character character, Trait trait)
    {
        bool isPhysicalTurn = true; // First turn grants Physical Power
        int lastApplied = 0;        // 0 = none, 1 = physical, -1 = magical

        character.OnUnitTurnStartEvent.AddListener(OnTurnStart);

        void OnTurnStart()
        {
            // Remove the previous bonus before applying the new one
            if (lastApplied == 1)
                character.PhysicalPower -= PowerBonus;
            else if (lastApplied == -1)
                character.MagicalPower -= PowerBonus;

            // Apply the new bonus
            if (isPhysicalTurn)
            {
                character.PhysicalPower += PowerBonus;
                lastApplied = 1;
            }
            else
            {
                character.MagicalPower += PowerBonus;
                lastApplied = -1;
            }

            isPhysicalTurn = !isPhysicalTurn;
        }
    }
}
