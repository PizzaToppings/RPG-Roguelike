using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class CharacterSelectSkillIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Skill skill;
    CharacterSelectUI characterSelectUI;
    int? displayPower;

    [SerializeField] Image image;

    public void Set(Skill _skill, int? casterPower = null)
    {
        skill = _skill;
        characterSelectUI = CharacterSelectUI.Instance;
        displayPower = casterPower;

        image.sprite = skill.mainSkillSO.Image;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (skill == null)
            return;

        // Pass this icon's RectTransform so the info panel can be positioned above it
        var rt = GetComponent<RectTransform>();
        characterSelectUI.StartShowSkillInformation(skill, rt, displayPower);
    }

    // Preserve parameterless methods for any existing EventTrigger calls in the Inspector
    public void OnPointerEnter()
    {
        OnPointerEnter(null);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (skill == null)
            return;

        characterSelectUI.EndShowSkillInformation(skill);
    }

    // Preserve parameterless methods for any existing EventTrigger calls in the Inspector
    public void OnPointerExit()
    {
        OnPointerExit(null);
    }
}
