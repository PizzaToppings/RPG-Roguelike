using System.Collections;
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
    [SerializeField] TextMeshProUGUI skillType;
    [SerializeField] List<Image> classIcons;
    [SerializeField] TextMeshProUGUI skillDescription;

    public void Activate(SO_MainSkill skill)
    {
        if (ui_Singletons == null)
            ui_Singletons = UI_Singletons.Instance;

        skillName.text = skill.SkillName;
        energyAmount.text = "Energy: " + skill.EnergyCost.ToString();
        chargeAmount.text = "Charges: " + skill.DefaultCharges.ToString();

        foreach (var skillIcon in classIcons)
            skillIcon.gameObject.SetActive(false);

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

            for (var i = 0; i < skill.Classes.Count; i++)
			{
                classIcons[i].gameObject.SetActive(true);
                classIcons[i].sprite = ui_Singletons.GetClassIcon(skill.Classes[i]);
            }
        }



        skillDescription.text = skill.Description;


        gameObject.SetActive(true);
    }

    public void Deactivate()
	{
        gameObject.SetActive(false);
    }
}
