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
    [SerializeField] TextMeshProUGUI intentDamageText;
    [SerializeField] TextMeshProUGUI intentRangeText;
    [SerializeField] TextMeshProUGUI orderText;

    UI_Singletons ui_Singletons => UI_Singletons.Instance;

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
    }

    public void InitIntent(SO_EnemySkill skill)
    {
        if (intentParent != null)
            intentParent.SetActive(true);

        UpdateIntent(skill);
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
            if (intentRangeText != null)
                intentRangeText.text = "";
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
        if (intentRangeText != null)
        {
            float minRange = skill.Skill.MinRange;
            float maxRange = skill.Skill.MaxRange;

            if (minRange == maxRange)
                intentRangeText.text = maxRange.ToString("F0");
            else if (minRange == 0)
                intentRangeText.text = $"{maxRange:F0}";
            else
                intentRangeText.text = $"{minRange:F0}-{maxRange:F0}";
        }
    }

    public void SetOrderNumber(int order)
    {
        if (orderText != null)
            orderText.text = order.ToString();
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
