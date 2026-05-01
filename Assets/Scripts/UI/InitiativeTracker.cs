using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class InitiativeTracker : MonoBehaviour
{
    public GameObject InitiativeImagePrefab;

    List<InitiativeInformation> initiativeList = new List<InitiativeInformation>();

    public void SetInitiative()
    {
        initiativeList.Clear();

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

        SortAndRefresh();

        NextTurn();
    }

    public void AddToInitiative(Unit unit)
	{
        var image = Instantiate(InitiativeImagePrefab, transform);
        var init = image.GetComponent<InitiativeInformation>();
        init.Init(unit, initiativeList.Count);

        initiativeList.Add(init);
        SortAndRefresh();
    }

    public void RemoveFromInitiative(Unit unit)
	{
        var init = initiativeList.First(x => x.thisUnit == unit);

        initiativeList.Remove(init);
        Destroy(init.gameObject);
        SortAndRefresh();
    }

    void SortAndRefresh()
    {
        initiativeList.Sort((x, y) => x.Initiative - y.Initiative);

        for (int i = 0; i < initiativeList.Count; i++)
        {
            initiativeList[i].transform.SetSiblingIndex(i);
            initiativeList[i].SetName();
            initiativeList[i].RefreshColor();
        }
    }

    public void NextTurn()
    {
        foreach (var initiative in initiativeList)
            initiative.ToggleActive(false);

        initiativeList[CombatData.CurrentUnitTurn].ToggleActive(true);
    }

    public void NextRound()
    {
        //roundNumber.text = CombatData.currentRound.ToString();
    }
}
