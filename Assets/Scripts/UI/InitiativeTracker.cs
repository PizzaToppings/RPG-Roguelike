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
        for (int i = 0; i < UnitData.Characters.Count; i++)
        {
            var character = UnitData.Characters[i];

            var image = Instantiate(InitiativeImage, transform);
            var init = image.GetComponent<InitiativeInformation>();
            init.Init(character, i);

            InitiativeList.Add(init);
        }
        for (int i = 0; i < UnitData.Enemies.Count; i++)
        {
            var enemy = UnitData.Enemies[i];

            var image = Instantiate(InitiativeImage, transform);
            var init = image.GetComponent<InitiativeInformation>();
            init.Init(enemy, i);

            InitiativeList.Add(init);
        }

        InitiativeList.Sort((x, y) => x.Initiative);

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
