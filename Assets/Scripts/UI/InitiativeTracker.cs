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
        InitiativeList.Clear();

        for (int i = 0; i < UnitData.Characters.Count; i++)
        {
            var character = UnitData.Characters[i];
            AddToInitiative(character);
        }
        for (int i = 0; i < UnitData.Enemies.Count; i++)
        {
            var enemy = UnitData.Enemies[i];
            AddToInitiative(enemy);
        }

        InitiativeList.Sort((x, y) => x.Initiative);

        NextTurn();
    }

    public void AddToInitiative(Unit unit)
	{
        var image = Instantiate(InitiativeImage, transform);
        var init = image.GetComponent<InitiativeInformation>();
        init.Init(unit, InitiativeList.Count);

        InitiativeList.Add(init);

        InitiativeList.Sort((x, y) => x.Initiative);
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
