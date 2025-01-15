using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InfoScreen : MonoBehaviour
{
    UI_Singletons ui_Singletons;

    [SerializeField] TextMeshProUGUI skillName;
    [SerializeField] TextMeshProUGUI energyAmount;
    [SerializeField] TextMeshProUGUI chargeAmount;
    [SerializeField] TextMeshProUGUI skillRange;
    [SerializeField] TextMeshProUGUI isMagicalText;
    [SerializeField] TextMeshProUGUI skillType;
    [SerializeField] List<Image> classIcons;
    [SerializeField] TextMeshProUGUI skillDescription;

    public void Activate(SO_MainSkill skill)
    {
        if (ui_Singletons == null)
            ui_Singletons = UI_Singletons.Instance;

        // basic
        skillName.text = skill.SkillName;
        energyAmount.text = "Energy: " + skill.EnergyCost.ToString();
        chargeAmount.text = "Charges: " + skill.DefaultCharges.ToString();
        skillRange.text = "Range: " + GetBaseRange(skill);
        isMagicalText.text = skill.IsMagical ? "magical" : "physical";
        isMagicalText.gameObject.SetActive(false);

        // skillIcons
        foreach (var skillIcon in classIcons)
            skillIcon.gameObject.SetActive(false);

        // basic skills or consumables
        if (skill.IsBasic)
		{
            skillType.text = "Basic skill";
		}
        else if (skill.IsConsumable)
        {
            skillType.text = "Consumable";
        }
        else
		{
            skillType.text = string.Empty;
            isMagicalText.gameObject.SetActive(true);

            for (var i = 0; i < skill.Classes.Count; i++)
			{
                classIcons[i].gameObject.SetActive(true);
                classIcons[i].sprite = ui_Singletons.GetClassIcon(skill.Classes[i]);
            }
        }

        skillDescription.text = skill.Description;

        gameObject.SetActive(true);
    }

    string GetBaseRange(SO_MainSkill skill)
    {
        string range = string.Empty;

        foreach (var spg in skill.SkillPartGroups)
        {
            foreach (var sp in spg.skillParts)
            {
                if (sp is SO_TargetSelfSkill)
                {
                    range = "Self";
                }
                else
                {
                    range = sp.MaxRange.ToString();
                    break;
                }
            }
        }
        return range;
    }

    public void Deactivate()
	{
        gameObject.SetActive(false);
    }
}
