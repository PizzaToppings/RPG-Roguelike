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
    [Tooltip("One assign button per party slot (max 4). Wire up in the inspector.")]
    [SerializeField] SkillSelectAssignButton[] assignButtons;

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
        descriptionText.text = ReplaceEffectText(skill.Description, skill);

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

    string ReplaceEffectText(string description, SO_MainSkill skill)
    {
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
                            var damageType = damageEffect.DamageType.ToString();
                            var damageText = $"{damageEffect.Power} {damageType} damage";
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
                    }
                }
            }
        }

        return description;
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
}
