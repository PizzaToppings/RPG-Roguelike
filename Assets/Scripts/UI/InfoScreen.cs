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
    [SerializeField] TextMeshProUGUI clickToLockText;

    const string ClickTolock = "Rightclick to lock this panel.";
    const string ClickToUnlock = "Click to remove this panel.";

    public bool IsLocked;

    public void Activate(SO_MainSkill skill, bool lockScreen)
    {
        if (ui_Singletons == null)
            ui_Singletons = UI_Singletons.Instance;

        if (lockScreen)
        {
            IsLocked = lockScreen;
            clickToLockText.text = ClickToUnlock;
        }

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

        skillDescription.text = GetDescription(skill);

        gameObject.SetActive(true);
        skillDescription.ForceMeshUpdate();
    }

    string GetBaseRange(SO_MainSkill skill)
    {
        string range = string.Empty;

        foreach (var spg in skill.SkillPartGroups)
        {
            if (range != string.Empty)
                break;

            foreach (var sp in spg.skillParts)
            {
                if (sp is SO_TargetSelfSkill)
                {
                    range = "Self";
                }
                else
                {
                    if (sp.MaxRange == 1.5f)
                        range = "Melee";
                    else
                        range = sp.MaxRange.ToString();
                    
                    break;
                }
            }
        }
        return range;
    }

    string GetDescription(SO_MainSkill skill)
    {
        var description = skill.Description;
        description = ReplaceEffectText(description, skill);

        return description;
    }

    string ReplaceEffectText(string description, SO_MainSkill skill)
    {
        var caster = UnitData.ActiveUnit;

        foreach (var spg in skill.SkillPartGroups)
        {
            foreach (var skillPart in spg.skillParts)
            {
                // Replace Damage Text
                foreach (var damageEffect in skillPart.DamageEffects)
                {
                    var damagePlaceholder = $"<damage{skill.SkillPartGroups.IndexOf(spg)}-{spg.skillParts.IndexOf(skillPart)}-{skillPart.DamageEffects.IndexOf(damageEffect)}>";
                    if (description.Contains(damagePlaceholder))
                    {
                        var skillDamage = damageEffect.Power;
                        var bonusDamage = skillPart.MagicalDamage ? caster.MagicalPower : caster.PhysicalPower;
                        var totalDamage = (skillDamage + bonusDamage).ToString();
                        var damageType = damageEffect.DamageType.ToString();
                        var damageText = $"{totalDamage} {damageType} damage";
                        description = description.Replace(damagePlaceholder, damageText);
                    }
                }

                // Replace Status Effect Text
                foreach (var statusEffect in skillPart.StatusEffects)
                {
                    string effectName = statusEffect.StatusEfectType.ToString().ToLower();
                    string colorCode = effectName switch
                    {
                        "bleed" => "#BF0000",
                        "burn" => "#ff0000ff",
                        "poison" => "#00BE01",
                        _ => "#FFFFFF"
                    };

                    var statusPlaceholder = $"<{effectName}{skill.SkillPartGroups.IndexOf(spg)}-{spg.skillParts.IndexOf(skillPart)}-{skillPart.StatusEffects.IndexOf(statusEffect)}>";
                    effectName = effectName.Substring(0, 1).ToUpper() + effectName.Substring(1).ToLower();
                    
                    if (description.Contains(statusPlaceholder))
                    {
                        var damage = statusEffect.Power;
                        var bonusDamage = statusEffect.IsMagical ? caster.MagicalPower : caster.PhysicalPower;
                        var totalDamage = (damage + bonusDamage).ToString();
                        var durationText = statusEffect.StatusEfectType == StatusEfectEnum.Bleed ? $" for {statusEffect.Duration} turn(s)" : "";
                        var statusText = $"{totalDamage} <link={effectName}><u><color={colorCode}>{effectName}</color></u></link>{durationText}.";
                        description = description.Replace(statusPlaceholder, statusText);
                    }
                }
            }
        }

        return description;
    }

    public void Unlock()
    {
        IsLocked = false;
        gameObject.SetActive(false);
        clickToLockText.text = ClickTolock;
    }

    public void Deactivate()
	{
        if (IsLocked == false)
            gameObject.SetActive(false);
    }
}
