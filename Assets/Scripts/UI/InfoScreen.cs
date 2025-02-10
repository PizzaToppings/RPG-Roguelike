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

    public void Activate(Skill skill, bool lockScreen)
    {
        if (ui_Singletons == null)
            ui_Singletons = UI_Singletons.Instance;

        if (lockScreen)
        {
            IsLocked = lockScreen;
            clickToLockText.text = ClickToUnlock;
        }

        // basic
        skillName.text = skill.mainSkillSO.SkillName;
        energyAmount.text = "Energy: " + skill.EnergyCost.ToString();
        chargeAmount.text = "Charges: " + skill.DefaultCharges.ToString();
        skillRange.text = "Range: " + GetBaseRange(skill);
        isMagicalText.text = skill.mainSkillSO.IsMagical ? "magical" : "physical";
        isMagicalText.gameObject.SetActive(false);

        // skillIcons
        foreach (var skillIcon in classIcons)
            skillIcon.gameObject.SetActive(false);

        // basic skills or consumables
        if (skill.mainSkillSO.IsBasic)
		{
            skillType.text = "Basic skill";
		}
        else if (skill.mainSkillSO.IsConsumable)
        {
            skillType.text = "Consumable";
        }
        else
		{
            skillType.text = string.Empty;
            isMagicalText.gameObject.SetActive(true);

            for (var i = 0; i < skill.mainSkillSO.Classes.Count; i++)
			{
                classIcons[i].gameObject.SetActive(true);
                classIcons[i].sprite = ui_Singletons.GetClassIcon(skill.mainSkillSO.Classes[i]);
            }
        }

        skillDescription.text = GetDescription(skill);

        gameObject.SetActive(true);
        skillDescription.ForceMeshUpdate();
    }

    string GetBaseRange(Skill skill)
    {
        string range = string.Empty;

        foreach (var spg in skill.SkillPartGroups)
        {
            if (range != string.Empty)
                break;

            foreach (var sp in spg.skillParts)
            {
                if (sp is SO_TargetSelfSkill ||
                    sp is SO_LineSkill ||
                    sp is SO_ConeSkill || 
                    sp is SO_AOE_Skill ||
                    sp is SO_HalfCircleSkill)
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

    string GetDescription(Skill skill)
    {
        var description = skill.mainSkillSO.Description;
        description = ReplaceEffectText(description, skill);
        description += CannotCastText(skill);

        return description;
    }

    string ReplaceEffectText(string description, Skill skill)
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
                    string effectName = statusEffect.StatusEffectType.ToString().ToLower();
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
                        var totalPower = CalculateTotalPower(statusEffect, caster);
                        var durationText = GetDurationText(statusEffect);
                        var statusText = $"{totalPower} <link={effectName}><u><color={colorCode}>{effectName}</color></u></link>{durationText}.";
                        description = description.Replace(statusPlaceholder, statusText);
                    }
                }
            }
        }

        return description;
    }

    string CalculateTotalPower(SO_StatusEffect statusEffect, Unit caster)
    {
        var power = statusEffect.Power;
        var bonusDamage = statusEffect.IsMagical ? caster.MagicalPower : caster.PhysicalPower;

        switch (statusEffect.StatusEffectType)
        {
            case StatusEffectEnum.Bleed:
            case StatusEffectEnum.Burn:
                return (power + bonusDamage).ToString();
        }

        return power.ToString();
    }

    string GetDurationText(SO_StatusEffect statusEffect)
    {
        switch (statusEffect.StatusEffectType)
        {
            case StatusEffectEnum.Bleed:
            case StatusEffectEnum.Thorns:
                return $" for {statusEffect.Duration} turn(s)";
        }

        return string.Empty;
    }

    string CannotCastText(Skill skill)
    {
        var statusEffectManager = StatusEffectManager.Instance;
        var caster = UnitData.ActiveUnit as Character;
        var text = string.Empty;

        // silenced
        if (skill.mainSkillSO.IsMagical && statusEffectManager.UnitHasStatusEffect(caster, StatusEffectEnum.Silenced))
            text += "<br>   Silenced.";

        // blinded
        if (skill.mainSkillSO.IsMagical == false && statusEffectManager.UnitHasStatusEffect(caster, StatusEffectEnum.Blinded))
            text += "<br>   Blinded.";

        if (skill.Charges == 0)
            text += "<br>   No charges left.";

        if (caster.Energy >= skill.EnergyCost)
            text += "<br>   Not enough Energy.";

        if (text == string.Empty)
            return text;

        return $"<br> <i> <color=#ff0000ff>{text}</color></i>";
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
