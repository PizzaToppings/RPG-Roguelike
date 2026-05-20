using UnityEngine;

public class UI_Singletons : MonoBehaviour
{
    public static UI_Singletons Instance;

    [SerializeField] SO_UI_Icons icons;

    public void CreateInstance()
    {
        Instance = this;
        icons.InitDefaultCursor();
    }

    public void SetCursor(CursorType cursorType) => icons.SetCursor(cursorType);
    public Color GetHitTypeColor(HitTypeEnum hitType) => icons.GetHitTypeColor(hitType);
    public Sprite GetClassIcon(ClassEnum thisClass) => icons.GetClassIcon(thisClass);
    public Sprite GetStatusIcon(StatusEffectEnum statusEffect) => icons.GetStatusIcon(statusEffect);
    public Sprite GetIntentActionIcon(IntentActionEnum action) => icons.GetIntentActionIcon(action);
    public Sprite GetIntentTargetIcon(IntentTargetEnum target) => icons.GetIntentTargetIcon(target);
}
