using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillInfoScreen : MonoBehaviour
{
    UI_Singletons ui_Singletons;

    [SerializeField] TextMeshProUGUI skillName;
    [SerializeField] TextMeshProUGUI energyAmount;
    [SerializeField] TextMeshProUGUI skillRange;
    [SerializeField] TextMeshProUGUI stanceText;
    [SerializeField] TextMeshProUGUI skillType;
    [SerializeField] List<Image> classIcons;
    [SerializeField] TextMeshProUGUI skillDescription;

    string stanceColorCode = ColorUtility.ToHtmlStringRGB(CombatStyleUtility.GetStyleColor(CombatStyle.None));

    public void Activate(Skill skill, bool lockScreen)
    {
        if (ui_Singletons == null)
            ui_Singletons = UI_Singletons.Instance;

        // basic
        skillName.text = skill.mainSkillSO.SkillName;
        //energyAmount.text = "Energy: " + skill.EnergyCost.ToString();
        skillRange.text = "Range: " + GetBaseRange(skill);
        stanceColorCode = ColorUtility.ToHtmlStringRGB(CombatStyleUtility.GetStyleColor(skill.mainSkillSO.SkillCombatStyle));
        stanceText.text = $"<color=#{stanceColorCode}>{skill.mainSkillSO.SkillCombatStyle}</color>";

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
        var foundEffects = new Dictionary<string, string>();
        (description, foundEffects) = ReplaceEffectText(description, skill);
        // description += CannotCastText(skill);

        var stanceDesc = CombatStyleUtility.GetStanceDescription(skill.mainSkillSO.SkillCombatStyle);
        var stanceName = $@"<b><color=#{stanceColorCode}>{skill.mainSkillSO.SkillCombatStyle}</color></b>";
        description += $"\n\n<size=16>Enter {stanceName} Stance at end of turn: \n {stanceDesc}</size>";

        if (foundEffects.Count > 0)
        {
            foreach (var kvp in foundEffects)
            {
                description += $"\n\n<size=15><b>({kvp.Key}:</b> \n<i>{kvp.Value})</i></size>";
            }
        }

        return description;
    }

    (string, Dictionary<string, string>) ReplaceEffectText(string description, Skill skill)
    {
        var caster = UnitData.ActiveUnit;
        var foundEffects = new System.Collections.Generic.Dictionary<string, string>();

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
                        var bonusDamage = caster.Power;
                        var totalDamage = (skillDamage + bonusDamage).ToString();
                        var damageText = $"{totalDamage} damage";
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
                    
                    var statusIdx = description.IndexOf(statusPlaceholder, System.StringComparison.OrdinalIgnoreCase);
                    if (statusIdx >= 0)
                    {
                        var damage = statusEffect.Power;
                        var totalPower = CalculateTotalPower(statusEffect, caster);
                        var durationText = GetDurationText(statusEffect);
                        var statusText = $"{totalPower} <link={effectName}><u><color={colorCode}>{effectName}</color></u></link>{durationText}.";
                        description = description.Remove(statusIdx, statusPlaceholder.Length).Insert(statusIdx, statusText);

                        // Collect a readable description for this status effect to append below the skill description
                        var displayName = $"<u><color={colorCode}>{effectName}</color></u>";
                        if (!foundEffects.ContainsKey(displayName))
                        {
                            // Use the StatusEffectDescriptions helper to resolve a description. Pass the base power from the SO.
                            var resolved = StatusEffectDescriptions.Resolve(statusEffect);
                            foundEffects.Add(displayName, resolved);
                        }
                    }
                }
            }
        }

        return (description, foundEffects);
    }

    string CalculateTotalPower(SO_StatusEffect statusEffect, Unit caster)
    {
        var power = statusEffect.Power;
        var bonusDamage = caster.Power;

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

        //if (caster.Energy < skill.EnergyCost)
        //    text += "<br>   Not enough Energy.";

        if (caster != null && caster.HasUsedSkillThisTurn)
            text += "<br>   Already used a skill this turn.";

        if (text == string.Empty)
            return text;

        return $"<br> <i> <color=#ff0000ff>{text}</color></i>";
    }

    public void Deactivate()
	{
        gameObject.SetActive(false);
    }
}
