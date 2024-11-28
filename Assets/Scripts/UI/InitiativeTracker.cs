using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InitiativeTracker : MonoBehaviour
{
    public GameObject InitiativeImage;

    List<InitiativeInformation> InitiativeList = new List<InitiativeInformation>();

    public void SetInitiative()
    {
        for (int i = 0; i < UnitData.Units.Count; i++)
        {
            var image = Instantiate(InitiativeImage, transform);
            var init = image.GetComponent<InitiativeInformation>();
            init.Init(UnitData.Units[i], i);

            InitiativeList.Add(init);
        }

        NextTurn();
    }

    public void NextTurn()
    {
        foreach (var initiative in InitiativeList)
            initiative.ToggleActive(false);

        InitiativeList[CombatData.currentCharacterTurn].ToggleActive(true);
    }

    public void NextRound()
    {
        //roundNumber.text = CombatData.currentRound.ToString();
    }
}
