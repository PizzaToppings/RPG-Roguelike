using UnityEngine;

public class ConsumableManager : MonoBehaviour
{
    public static ConsumableManager Instance;

    [SerializeField] GameObject ConsumablesPouch;

    public void CreateInstance()
    {
        Instance = this;
    }


    public void Init()
    {

    }

    public void ToggleConsumablesPouch()
	{
        ConsumablesPouch.SetActive(!ConsumablesPouch.activeInHierarchy);
	}

    public void SetConsumables(Character character)
    {
        foreach (var Consumable in character.consumables)
        {
            if (Consumable != null)
                Consumable.Init();
        }
    }

    public void DeleteConsumable(Skill consumable)
	{
        var character = UnitData.ActiveUnit as Character;

        for (int i = 0; i < character.consumables.Count; i++)
		{
            var c = character.consumables[i];

            if (c == null)
                continue;

            if (consumable == c)
                character.consumables[i] = null;
        }
	}
}
