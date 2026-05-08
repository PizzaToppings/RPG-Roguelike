using UnityEngine;
using UnityEngine.UI;

public class TrinketSelectAssignButton : MonoBehaviour
{
    Image characterIcon;
    Button button;

    public void Setup(SO_Character character, int partyIndex, SO_Trinket trinket)
    {
        characterIcon = GetComponent<Image>();
        characterIcon.sprite = character.Image;

        button = GetComponent<Button>();

        bool compatible = trinket.classes.Count == 0 ||
                          trinket.classes.Exists(c => character.Classes.Contains(c));

        button.interactable = compatible;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => RunManager.Instance.SelectTrinket(trinket, partyIndex));
    }
}
