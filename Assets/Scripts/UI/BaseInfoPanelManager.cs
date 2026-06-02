using UnityEngine;
using TMPro;

/// <summary>
/// Shared base for EnemyInfoPanelManager and CharacterInfoPanelManager.
/// Handles common stat display and the status-effect list.
/// </summary>
public abstract class BaseInfoPanelManager : MonoBehaviour
{
    [Header("Basic Info")]
    [SerializeField] protected TextMeshProUGUI unitNameText;
    [SerializeField] protected TextMeshProUGUI powerText;
    [SerializeField] protected TextMeshProUGUI defenseText;

    [Header("Status Effects")]
    [SerializeField] protected Transform statusEffectsContainer;
    [SerializeField] protected GameObject statusEffectEntryPrefab;

    protected RectTransform panelRect;
    protected Canvas parentCanvas;
    protected bool isVisible;

    protected abstract GameObject PanelRoot { get; }

    protected virtual void Awake()
    {
        panelRect = PanelRoot.GetComponent<RectTransform>();
        parentCanvas = PanelRoot.GetComponentInParent<Canvas>();
        PanelRoot.SetActive(false);
    }

    protected void PopulateBaseStats(Unit unit)
    {
        if (unitNameText != null)
            unitNameText.text = unit.UnitName;

        if (powerText != null)
            powerText.text = unit.Power.ToString();
        if (defenseText != null)
            defenseText.text = unit.Defense.ToString();
    }

    protected void PopulateStatusEffects(Unit unit)
    {
        if (statusEffectsContainer == null || statusEffectEntryPrefab == null) return;

        // Only remove previously spawned entries, not sibling panels like the main stats panel
        for (int i = statusEffectsContainer.childCount - 1; i >= 0; i--)
        {
            var child = statusEffectsContainer.GetChild(i);
            if (child.GetComponent<StatusEffectEntryUI>() != null)
                Destroy(child.gameObject);
        }

        // Show the current stance as the first entry
        if (unit.CurrentCombatStyle != CombatStyle.None)
        {
            var stanceEntry = Instantiate(statusEffectEntryPrefab, statusEffectsContainer);
            var stanceUI = stanceEntry.GetComponent<StatusEffectEntryUI>();
            if (stanceUI != null)
                stanceUI.Populate(
                    unit.CurrentCombatStyle.ToString(),
                    CombatStyleUtility.GetStanceDescription(unit.CurrentCombatStyle),
                    "Active");
        }

        foreach (var effect in unit.statusEffects)
        {
            var entry = Instantiate(statusEffectEntryPrefab, statusEffectsContainer);
            var entryUI = entry.GetComponent<StatusEffectEntryUI>();
            if (entryUI != null)
                entryUI.Populate(effect);
        }
    }

    public virtual void HidePanel()
    {
        if (PanelRoot == null) return;
        PanelRoot.SetActive(false);
        isVisible = false;
    }

    protected void ClampToScreen()
    {
        Vector3[] corners = new Vector3[4];
        panelRect.GetWorldCorners(corners);

        float rightOverflow  = corners[2].x - Screen.width;
        float bottomOverflow = corners[0].y;

        Vector2 pos = panelRect.position;

        if (rightOverflow > 0)
            pos.x -= rightOverflow;

        if (bottomOverflow < 0)
            pos.y -= bottomOverflow;

        panelRect.position = pos;
    }
}
