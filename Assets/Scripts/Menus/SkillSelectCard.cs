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
    [SerializeField] TextMeshProUGUI stanceText;
    [SerializeField] TextMeshProUGUI isMagicalText;
    [SerializeField] TextMeshProUGUI ChargesText;
    [SerializeField] Image           skillIcon;
    [Tooltip("One assign button per party slot (max 4). Wire up in the inspector.")]
    [SerializeField] SkillSelectAssignButton[] assignButtons;

    SO_MainSkill skillData;
    string stanceColorCode;

    public void Setup(SO_MainSkill skill)
    {
        skillData = skill;

        skillNameText.text   = skill.SkillName;

        for (int i = 0; i < skillData.Classes.Count; i++)
        {
            classIcons[i].gameObject.SetActive(true);
            classIcons[i].sprite = SkillSelectUI.Instance.GetClassIcon(skillData.Classes[i]);
        }

        stanceColorCode = ColorUtility.ToHtmlStringRGB(CombatStyleUtility.GetStyleColor(skill.SkillCombatStyle));
        stanceText.text = $"Stance: \n<color=#{stanceColorCode}>{skill.SkillCombatStyle}</color>";

        isMagicalText.text = skill.IsMagical ? "Magical" : "Physical";
        rangeText.text = $"Range: {GetBaseRange(skill)}";
        ChargesText.text = $"Charges: {skill.DefaultCharges}";

        descriptionText.text = GetDescription(skill);

        //if (energyCostText != null)
        //    energyCostText.text = $"Cost: {skill.EnergyCost}";

        if (skillIcon != null && skill.Image != null)
        {
            skillIcon.gameObject.SetActive(true);
            skillIcon.sprite = skill.Image;
        }

        // Set up one assign button per party member.
        if (assignButtons != null)
        {
            for (int i = 0; i < assignButtons.Length; i++)
            {
                if (i < RunData.Party.Count)
                {
                    assignButtons[i].gameObject.SetActive(true);
                    assignButtons[i].Setup(RunData.Party[i].Character, i, skill);
                }
                else
                {
                    assignButtons[i].gameObject.SetActive(false);
                }
            }
        }
    }

    string GetDescription(SO_MainSkill skill)
    {
        var description = skill.Description;
        var foundEffects = new Dictionary<string, string>();
        (description, foundEffects) = ReplaceEffectText(description, skill);

        var stanceDesc = CombatStyleUtility.GetStanceDescription(skill.SkillCombatStyle);
        var stanceName = $@"<b><color=#{stanceColorCode}>{skill.SkillCombatStyle}</color></b>";
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

    (string, Dictionary<string, string>) ReplaceEffectText(string description, SO_MainSkill skill)
    {
        var foundEffects = new Dictionary<string, string>();

        foreach (var spg in skill.SkillPartGroups)
        {
            foreach (var skillPart in spg.skillParts)
            {
                // Replace Damage Text
                if (skillPart.DamageEffects.Count > 0)
                {
                    foreach (var damageEffect in skillPart.DamageEffects)
                    {
                        var damagePlaceholder = $"<damage{skill.SkillPartGroups.IndexOf(spg)}-{spg.skillParts.IndexOf(skillPart)}-{skillPart.DamageEffects.IndexOf(damageEffect)}>";
                        if (description.Contains(damagePlaceholder))
                        {
                            var damageText = $"{damageEffect.Power} damage";
                            description = description.Replace(damagePlaceholder, damageText);
                        }
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
                        "dodge" => "#1E90FF",
                        "taunt" => "#FF8C00",
                        "regen" => "#00FF7F",
                        _ => "#FFFFFF"
                    };

                    var statusPlaceholder = $"<{effectName}{skill.SkillPartGroups.IndexOf(spg)}-{spg.skillParts.IndexOf(skillPart)}-{skillPart.StatusEffects.IndexOf(statusEffect)}>";
                    effectName = effectName.Substring(0, 1).ToUpper() + effectName.Substring(1).ToLower();

                    var statusIdx = description.IndexOf(statusPlaceholder, System.StringComparison.OrdinalIgnoreCase);
                    if (statusIdx >= 0)
                    {
                        var durationText = GetDurationText(statusEffect);
                        var statusText = $"{statusEffect.Power} <link={effectName}><u><color={colorCode}>{effectName}</color></u></link>{durationText}.";
                        description = description.Remove(statusIdx, statusPlaceholder.Length).Insert(statusIdx, statusText);

                        var displayName = $"<u><color={colorCode}>{effectName}</color></u>";
                        if (!foundEffects.ContainsKey(displayName))
                        {
                            var resolved = StatusEffectDescriptions.Resolve(statusEffect);
                            foundEffects.Add(displayName, resolved);
                        }
                    }
                }
            }
        }

        return (description, foundEffects);
    }

    string GetDurationText(SO_StatusEffect statusEffect)
    {
        switch (statusEffect.StatusEffectType)
        {
            case StatusEffectEnum.Bleed:
            case StatusEffectEnum.Thorns:
            case StatusEffectEnum.Dodge:
            case StatusEffectEnum.Taunt:
            case StatusEffectEnum.Regen:
                return $" for {statusEffect.Duration} turn(s)";
        }
        return string.Empty;
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
}
