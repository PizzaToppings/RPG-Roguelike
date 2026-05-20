using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TraitSelectCard : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI traitNameText;
    [SerializeField] Image           traitIcon;
    [SerializeField] TextMeshProUGUI descriptionText;
    [SerializeField] List<Image>     classIcons;

    [Tooltip("One assign button per party slot (max 4). Wire up in the inspector.")]
    [SerializeField] TraitSelectAssignButton[] assignButtons;

    SO_Trait traitData;

    public void Setup(SO_Trait trait)
    {
        traitData = trait;

        if (traitNameText != null)
            traitNameText.text = trait.TraitName;

        if (traitIcon != null && trait.Image != null)
        {
            traitIcon.gameObject.SetActive(true);
            traitIcon.sprite = trait.Image;
        }

        if (descriptionText != null)
            descriptionText.text = trait.Description;

        if (classIcons != null)
        {
            for (int i = 0; i < classIcons.Count; i++)
            {
                if (i < trait.classes.Count)
                {
                    classIcons[i].gameObject.SetActive(true);
                    classIcons[i].sprite = TraitSelectUI.Instance.GetClassIcon(trait.classes[i]);
                }
                else
                {
                    classIcons[i].gameObject.SetActive(false);
                }
            }
        }

        if (assignButtons != null)
        {
            for (int i = 0; i < assignButtons.Length; i++)
            {
                if (i < RunData.Party.Count)
                {
                    assignButtons[i].gameObject.SetActive(true);
                    assignButtons[i].Setup(RunData.Party[i].Character, i, trait);
                }
                else
                {
                    assignButtons[i].gameObject.SetActive(false);
                }
            }
        }
    }
}
