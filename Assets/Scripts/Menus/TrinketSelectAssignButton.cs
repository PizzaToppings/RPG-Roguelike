using UnityEngine;
using UnityEngine.UI;

public class TraitSelectAssignButton : MonoBehaviour
{
    Image characterIcon;
    Button button;

    public void Setup(SO_Character character, int partyIndex, SO_Trait trait)
    {
        characterIcon = GetComponent<Image>();
        characterIcon.sprite = character.Image;

        button = GetComponent<Button>();

        bool compatible = trait.classes.Count == 0 ||
                          trait.classes.Exists(c => character.Classes.Contains(c));

        button.interactable = compatible;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => RunManager.Instance.SelectTrait(trait, partyIndex));
    }
}
