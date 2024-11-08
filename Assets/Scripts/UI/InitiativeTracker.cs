using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InitiativeTracker : MonoBehaviour
{
    [SerializeField] Color activeColor;
    [SerializeField] Color inactiveColor;

    [SerializeField] TextMeshProUGUI roundNumber;
    [SerializeField] List<TextMeshProUGUI> initiativeNames = new List<TextMeshProUGUI>();


    void Start()
    {

    }

    public void SetInitiative()
    {
        for (int i = 0; i < UnitData.Units.Count; i++)
        {
            var name = UnitData.Units[i].UnitName == "" ? UnitData.Units[i].name : UnitData.Units[i].UnitName;
            initiativeNames[i].text = name;
        }

        NextTurn();
    }

    public void NextTurn()
    {
        foreach (var initiativeName in initiativeNames)
            initiativeName.color = inactiveColor;

        initiativeNames[CombatData.currentCharacterTurn].color = activeColor;
    }

    public void NextRound()
    {
        roundNumber.text = CombatData.currentRound.ToString();
    }
}
