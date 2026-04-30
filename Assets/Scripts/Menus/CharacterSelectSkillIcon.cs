using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class CharacterSelectSkillIcon : MonoBehaviour
{
    Skill skill;
    CharacterSelectUI characterSelectUI;

    [SerializeField] Image image;

    public void Set(Skill _skill)
    {
        skill = _skill;
        characterSelectUI = CharacterSelectUI.Instance;

        image.sprite = skill.mainSkillSO.Image;
    }

    public void OnPointerEnter()
    {
        if (skill == null)
            return;

        characterSelectUI.StartShowSkillInformation(skill);
    }

    public void OnPointerExit()
    {
        if (skill == null)
            return;

        characterSelectUI.EndShowSkillInformation(skill);
    }
}
