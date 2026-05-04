using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class SkillSelectAssignButton : MonoBehaviour
{
    Image characterIcon;
    Button button;

    public void Setup(SO_Character character, int partyIndex, SO_MainSkill skill)
    {
        if (characterIcon != null)
            characterIcon.sprite = character.Image;

        characterIcon = GetComponent<Image>();
        button = GetComponent<Button>();

        bool compatible = skill.Classes.Count == 0 ||
                          skill.Classes.Exists(c => character.Classes.Contains(c));

        button.interactable = compatible;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => RunManager.Instance.SelectSkill(skill, partyIndex));
    }
}
