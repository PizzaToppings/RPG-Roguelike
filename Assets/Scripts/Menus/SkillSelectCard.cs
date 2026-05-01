using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillSelectCard : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI skillNameText;
    [SerializeField] List<Image>     classIcons;
    [SerializeField] TextMeshProUGUI descriptionText;
    [SerializeField] TextMeshProUGUI energyCostText;
    [SerializeField] TextMeshProUGUI rangeText;
    [SerializeField] TextMeshProUGUI DamageTypeText;
    [SerializeField] TextMeshProUGUI ChargesText;
    [SerializeField] Image           skillIcon;
    [SerializeField] Button          selectButton;

    SO_MainSkill skillData;

    public void Setup(SO_MainSkill skill)
    {
        skillData = skill;

        skillNameText.text   = skill.SkillName;

        for (int i = 0; i < skillData.Classes.Count; i++)
        {
            classIcons[i].gameObject.SetActive(true);
            classIcons[i].sprite = SkillSelectUI.Instance.GetClassIcon(skillData.Classes[i]);
        }
        descriptionText.text = skill.Description;

        if (DamageTypeText != null)
        {
            if (skill.IsMagical)
                DamageTypeText.text = "Damage Type: Magical";
            else
                DamageTypeText.text = "Damage Type: Physical";
        }

        if (ChargesText != null)
            ChargesText.text = $"Charges: {skill.DefaultCharges}";

        if (rangeText != null)
            rangeText.text = $"Range: {skill.GetAttackRange()}";

        if (energyCostText != null)
            energyCostText.text = $"Cost: {skill.EnergyCost}";

        if (skillIcon != null && skill.Image != null)
            skillIcon.sprite = skill.Image;

        selectButton.onClick.RemoveAllListeners();
        selectButton.onClick.AddListener(OnSelect);
    }

    void OnSelect()
    {
        RunManager.Instance.SelectSkill(skillData);
    }
}
