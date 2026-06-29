using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FloatingHealthbar : Healthbar
{
    Transform unit;
    Vector3 offset = Vector3.up * 0.6f;

    Camera mainCamera;
    float referenceOrthoSize;

    [Space]
    [Header("Intent")]
    [SerializeField] GameObject intentParent;
    [SerializeField] Image intentActionImage;
    [SerializeField] Image intentTargetImage;
    [SerializeField] Image intentStyleColorImage;
    [SerializeField] TextMeshProUGUI intentDamageText;
    [SerializeField] TextMeshProUGUI attackRangeText;
    [SerializeField] TextMeshProUGUI orderText;

    [Space]
    [Header("Damage Prediction")]
    [SerializeField] GameObject damagePreviewParent;
    [SerializeField] TextMeshProUGUI damagePreviewText;

    UI_Singletons ui_Singletons => UI_Singletons.Instance;
    DamageManager damageManager => DamageManager.Instance;

    public void Init(Unit myUnit)
    {
        thisUnit = myUnit;


        if (myUnit.Friendly)
        {
                healthbar.sprite = characterHealthbarSprite;
        }
        else
        {
                healthbar.sprite = enemyHealthbarSprite;
        }
        mainCamera = Camera.main;
        unit = myUnit.transform;
        referenceOrthoSize = mainCamera.orthographicSize;

        if (intentParent != null)
            intentParent.SetActive(false);

        if (damagePreviewParent != null)
            damagePreviewParent.SetActive(false);
    }

    public void InitIntent(SO_EnemySkill skill)
    {
        if (intentParent != null)
            intentParent.SetActive(true);

        UpdateIntent(skill);
        RefreshStyleColor();
    }

    public void RefreshStyleColor()
    {
        if (intentStyleColorImage == null) return;
        if (thisUnit == null || thisUnit.CurrentCombatStyle == CombatStyle.None)
        {
            intentStyleColorImage.gameObject.SetActive(false);
            return;
        }
        intentStyleColorImage.gameObject.SetActive(true);
        intentStyleColorImage.color = CombatStyleUtility.GetStyleColor(thisUnit.CurrentCombatStyle);
    }

    public void UpdateIntent(SO_EnemySkill skill)
    {
        if (skill == null)
            return;

        UpdateIntent(skill.IntentAction, skill.GetIntentTarget());
        UpdateIntentDamageAndRange(skill);
    }

    public void UpdateIntent(IntentActionEnum action, IntentTargetEnum target)
    {
        if (intentActionImage != null)
            intentActionImage.sprite = ui_Singletons.GetIntentActionIcon(action);

        if (intentTargetImage != null)
            intentTargetImage.sprite = ui_Singletons.GetIntentTargetIcon(target);
    }

    void UpdateIntentDamageAndRange(SO_EnemySkill skill)
    {
        if (skill?.Skill == null)
        {
            if (intentDamageText != null) intentDamageText.text = "";
            if (attackRangeText  != null) attackRangeText.text  = "";
            return;
        }

        // Sum damage power across all parts in the first SkillPartGroup
        if (intentDamageText != null)
        {
            int totalDamage = 0;
            bool hasDamage = false;
            if (skill.Skill.SkillPartGroups.Count > 0)
            {
                foreach (var sp in skill.Skill.SkillPartGroups[0].skillParts)
                {
                    if (sp?.DamageEffects == null) continue;
                    foreach (var dmg in sp.DamageEffects)
                    {
                        if (dmg != null && dmg.HitType == HitTypeEnum.Damage)
                        {
                            totalDamage += dmg.Power;
                            hasDamage = true;
                        }
                    }
                }
            }
            intentDamageText.text = hasDamage ? totalDamage.ToString() : "";
        }

        // Display attack range
        if (attackRangeText != null)
        {
            float range = skill.Skill.GetAttackRange();
            attackRangeText.text = range > 0f ? range.ToString("0.#") : "";
        }
    }

    public void SetOrderNumber(int order)
    {
        if (orderText != null)
            orderText.text = order.ToString();
    }

    public void ShowDamagePreview(Skill activeSkill)
    {
        if (damagePreviewParent == null || damagePreviewText == null || thisUnit == null)
            return;

        if (activeSkill == null || SkillData.Caster == null)
        {
            HideDamagePreview();
            return;
        }

        // Temporarily apply the skill's combat style so the style multiplier matches what CastSkill will use
        var caster = SkillData.Caster;
        var originalCasterStyle = caster.CurrentCombatStyle;
        if (activeSkill.mainSkillSO.SkillCombatStyle != CombatStyle.None)
            caster.CurrentCombatStyle = activeSkill.mainSkillSO.SkillCombatStyle;

        // Calculate predicted damage from all damage effects in the active skill
        int totalPredictedDamage = 0;
        bool hasDamageEffects = false;
        // IMPORTANT: Use the runtime Skill.SkillPartGroups for damage calculations
        var currentSpg = activeSkill.SkillPartGroups[SkillData.SkillPartGroupIndex];

        foreach (var skillPart in currentSpg.skillParts)
        {
            // Only calculate damage for skill parts that actually target this unit
            // This correctly handles skills with multiple parts hitting the same enemy
            if (skillPart.PartData?.TargetsHit == null || !skillPart.PartData.TargetsHit.Contains(thisUnit))
                continue;

            if (skillPart.DamageEffects != null)
            {
                foreach (var damageData in skillPart.DamageEffects)
                {
                    if (damageData != null)
                    {
                        // Skip healing and shield effects
                        if (damageData.HitType == HitTypeEnum.Healing || 
                            damageData.HitType == HitTypeEnum.Shield)
                            continue;

                        hasDamageEffects = true;

                        // Create a copy of the damage data with the correct caster
                        var damageDataCopy = new DamageData
                        {
                            Power = damageData.Power,
                            HitType = damageData.HitType,
                            Caster = SkillData.Caster,
                            Modifiers = damageData.Modifiers,
                            Prerequisites = damageData.Prerequisites
                        };

                        // Calculate the damage using the DamageManager
                        var calculatedDamage = damageManager.CalculateDamageData(damageDataCopy, thisUnit);
                        totalPredictedDamage += calculatedDamage.Damage;
                    }
                }
            }
        }

        // Restore the caster's original combat style after preview calculation
        caster.CurrentCombatStyle = originalCasterStyle;

        // Only show damage preview if the skill actually has damaging effects
        // (Show 0 if armor negates all damage, but don't show for healing/support skills)
        if (hasDamageEffects)
        {
            // Apply the caster's OutgoingDamageMultiplier, matching DealDamageWithDelay
            if (caster != null && caster.OutgoingDamageMultiplier != 1f)
                totalPredictedDamage = Mathf.CeilToInt(totalPredictedDamage * caster.OutgoingDamageMultiplier);

            damagePreviewParent.SetActive(true);
            damagePreviewText.text = Mathf.Max(0, totalPredictedDamage).ToString();
        }
        else
        {
            HideDamagePreview();
        }
    }

    public void HideDamagePreview()
    {
        if (damagePreviewParent != null)
            damagePreviewParent.SetActive(false);
    }

    /// <summary>
    /// Shows damage preview based on an enemy's intent. Used when hovering over an enemy to see what damage they will deal.
    /// </summary>
    public void ShowDamagePreviewFromEnemy(EnemyBaseAI enemy)
    {
        // TODO AI: Check and fix code below based on new skill system. The old code was commented out because it referenced properties that no longer exist in the new skill system.

        // if (damagePreviewParent == null || damagePreviewText == null || thisUnit == null || enemy == null)
        //     return;

        // if (enemy.CurrentSkill?.Skill == null || enemy.CurrentSkill.Skill.Count == 0)
        // {
        //     HideDamagePreview();
        //     return;
        // }

        // // All skill parts deal the same damage; calculate for the first part and multiply by count
        // var firstPart = enemy.CurrentSkill.FirstPart;
        // int perPartDamage = 0;
        // bool hasDamageEffects = false;

        // if (firstPart?.DamageEffects != null)
        // {
        //     foreach (var damageData in firstPart.DamageEffects)
        //     {
        //         if (damageData == null) continue;
        //         if (damageData.HitType == HitTypeEnum.Healing ||
        //             damageData.HitType == HitTypeEnum.Shield)
        //             continue;

        //         hasDamageEffects = true;

        //         var damageDataCopy = new DamageData
        //         {
        //             Power = damageData.Power,
        //             HitType = damageData.HitType,
        //             Caster = enemy,
        //             Modifiers = damageData.Modifiers,
        //             Prerequisites = damageData.Prerequisites
        //         };

        //         var calculatedDamage = damageManager.CalculateDamageData(damageDataCopy, thisUnit);
        //         perPartDamage += calculatedDamage.Damage;
        //     }
        // }

        // if (hasDamageEffects)
        // {
        //     int count = enemy.CurrentSkill.Skill.Count;
        //     damagePreviewParent.SetActive(true);
        //     damagePreviewText.text = count > 1
        //         ? $"{count}x{Mathf.Max(0, perPartDamage)}"
        //         : Mathf.Max(0, perPartDamage).ToString();
        // }
        // else
        // {
        //     HideDamagePreview();
        // }
    }

    void Update()
    {
        if (unit == null || mainCamera == null)
            return;

        Vector3 screenPosition = mainCamera.WorldToScreenPoint(unit.position + offset);
        transform.position = screenPosition;

        float scale = referenceOrthoSize / mainCamera.orthographicSize;
        transform.localScale = Vector3.one * scale;
    }
}
