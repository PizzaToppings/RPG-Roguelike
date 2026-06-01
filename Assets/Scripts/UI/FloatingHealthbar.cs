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
            if (intentDamageText != null)
                intentDamageText.text = "";
            if (attackRangeText != null)
                attackRangeText.text = "";
            return;
        }

        // Calculate total damage from all damage effects
        if (intentDamageText != null)
        {
            int totalDamage = 0;
            if (skill.Skill.DamageEffects != null && skill.Skill.DamageEffects.Count > 0)
            {
                foreach (var damageData in skill.Skill.DamageEffects)
                {
                    if (damageData != null)
                        totalDamage += damageData.Power;
                }
                intentDamageText.text = totalDamage.ToString();
            }
            else
            {
                intentDamageText.text = "";
            }
        }

        // Display range
        if (attackRangeText != null)
        {
            var totalThreat = skill.Skill.MaxRange + thisUnit.MoveSpeed;
            attackRangeText.text = totalThreat.ToString(); 
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
                            IsMagical = activeSkill.mainSkillSO.IsMagical,
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

        // Only show damage preview if the skill actually has damaging effects
        // (Show 0 if defense negates all damage, but don't show for healing/support skills)
        if (hasDamageEffects)
        {
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
        if (damagePreviewParent == null || damagePreviewText == null || thisUnit == null || enemy == null)
            return;

        if (enemy.CurrentSkill?.Skill == null)
        {
            HideDamagePreview();
            return;
        }

        // Calculate predicted damage from the enemy's skill
        int totalPredictedDamage = 0;
        var skillPart = enemy.CurrentSkill.Skill;
        bool hasDamageEffects = false;

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

                    // Create a copy of the damage data with the enemy as caster
                    var damageDataCopy = new DamageData
                    {
                        Power = damageData.Power,
                        HitType = damageData.HitType,
                        IsMagical = damageData.IsMagical,
                        Caster = enemy,
                        Modifiers = damageData.Modifiers,
                        Prerequisites = damageData.Prerequisites
                    };

                    // Calculate the damage using the DamageManager
                    var calculatedDamage = damageManager.CalculateDamageData(damageDataCopy, thisUnit);
                    totalPredictedDamage += calculatedDamage.Damage;
                }
            }
        }

        // Only show damage preview if the enemy actually has damaging effects
        // (Show 0 if defense negates all damage, but don't show for healing/support skills)
        if (hasDamageEffects)
        {
            damagePreviewParent.SetActive(true);
            damagePreviewText.text = Mathf.Max(0, totalPredictedDamage).ToString();
        }
        else
        {
            HideDamagePreview();
        }
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
