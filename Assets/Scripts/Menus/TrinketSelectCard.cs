using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TrinketSelectCard : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI trinketNameText;
    [SerializeField] Image           trinketIcon;
    [SerializeField] TextMeshProUGUI rarityText;
    [SerializeField] TextMeshProUGUI descriptionText;
    [SerializeField] List<Image>     classIcons;

    [Tooltip("One assign button per party slot (max 4). Wire up in the inspector.")]
    [SerializeField] TrinketSelectAssignButton[] assignButtons;

    SO_Trinket trinketData;

    public void Setup(SO_Trinket trinket)
    {
        trinketData = trinket;

        if (trinketNameText != null)
            trinketNameText.text = trinket.TrinketName;

        if (trinketIcon != null && trinket.Image != null)
        {
            trinketIcon.gameObject.SetActive(true);
            trinketIcon.sprite = trinket.Image;
        }

        if (rarityText != null)
            rarityText.text = new string('?', trinket.Rarity);

        if (descriptionText != null)
            descriptionText.text = trinket.Description;

        if (classIcons != null)
        {
            for (int i = 0; i < classIcons.Count; i++)
            {
                if (i < trinket.classes.Count)
                {
                    classIcons[i].gameObject.SetActive(true);
                    classIcons[i].sprite = TrinketSelectUI.Instance.GetClassIcon(trinket.classes[i]);
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
                    assignButtons[i].Setup(RunData.Party[i].Character, i, trinket);
                }
                else
                {
                    assignButtons[i].gameObject.SetActive(false);
                }
            }
        }
    }
}
