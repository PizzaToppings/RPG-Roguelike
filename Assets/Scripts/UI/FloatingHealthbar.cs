using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FloatingHealthbar : Healthbar
{
    Transform unit;
    Vector3 offset = Vector3.up * 2f;

    Camera mainCamera;

    [Space]
    [Header("Intent")]
    [SerializeField] Image intentActionImage;
    [SerializeField] Image intentTargetImage;
    [SerializeField] TextMeshProUGUI orderText;

    UI_Singletons ui_Singletons => UI_Singletons.Instance;

    public void Init(Unit myUnit)
    {
        thisUnit = myUnit;
        mainCamera = Camera.main;
        unit = myUnit.transform;
    }

    public void InitIntent()
    {
        UpdateIntent(IntentActionEnum.Unknown, IntentTargetEnum.Unknown);
    }

    public void UpdateIntent(SO_EnemySkill skill)
    {
        UpdateIntent(skill.IntentAction, skill.GetIntentTarget());
    }

    public void UpdateIntent(IntentActionEnum action, IntentTargetEnum target)
    {
        if (intentActionImage != null)
            intentActionImage.sprite = ui_Singletons.GetIntentActionIcon(action);

        if (intentTargetImage != null)
            intentTargetImage.sprite = ui_Singletons.GetIntentTargetIcon(target);
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
    }
}
