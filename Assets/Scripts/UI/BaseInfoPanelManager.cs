using System.Collections.Generic;
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
    [SerializeField] protected TextMeshProUGUI ArmorText;

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
        if (ArmorText != null)
            ArmorText.text = unit.Armor.ToString();
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

        // Show the current stance as the first entry (colored style name)
        if (unit.CurrentCombatStyle != CombatStyle.None)
        {
            var styleColor = CombatStyleUtility.GetStyleColor(unit.CurrentCombatStyle);
            var hex = ColorUtility.ToHtmlStringRGB(styleColor);
            var name = $"Stance: <color=#{hex}>{unit.CurrentCombatStyle}</color>";

            var stanceEntry = Instantiate(statusEffectEntryPrefab, statusEffectsContainer);
            var stanceUI = stanceEntry.GetComponent<StatusEffectEntryUI>();
            if (stanceUI != null)
                stanceUI.Populate(
                    name,
                    CombatStyleUtility.GetStanceDescription(unit.CurrentCombatStyle),
                    "Active");
        }

        // Show pending stance if present and different from current
        if (unit.PendingCombatStyle != CombatStyle.None && unit.PendingCombatStyle != unit.CurrentCombatStyle)
        {
            var pColor = CombatStyleUtility.GetStyleColor(unit.PendingCombatStyle);
            var pHex = ColorUtility.ToHtmlStringRGB(pColor);
            var pName = $"Pending: <color=#{pHex}>{unit.PendingCombatStyle}</color>";

            var pendingEntry = Instantiate(statusEffectEntryPrefab, statusEffectsContainer);
            var pendingUI = pendingEntry.GetComponent<StatusEffectEntryUI>();
            if (pendingUI != null)
                pendingUI.Populate(
                    pName,
                    CombatStyleUtility.GetStanceDescription(unit.PendingCombatStyle),
                    "Pending");
        }

        // Collect visible effects (excluding ones explicitly hidden)
        var visibleEffects = new List<StatusEffect>();
        foreach (var effect in unit.statusEffects)
        {
            if (!effect.HideInInfoPanel)
                visibleEffects.Add(effect);
        }

        // Group stat-change effects that originated from a CombatStyle so we can
        // display a single compact entry per style (e.g. "Control: -1 Power").
        var styleGroups = new Dictionary<CombatStyle, List<StatChangeEffect>>();
        var nonStyleEffects = new List<StatusEffect>();

        foreach (var effect in visibleEffects)
        {
            if (effect is StatChangeEffect sce && sce.SourceCombatStyle != CombatStyle.None)
            {
                if (!styleGroups.ContainsKey(sce.SourceCombatStyle))
                    styleGroups[sce.SourceCombatStyle] = new List<StatChangeEffect>();
                styleGroups[sce.SourceCombatStyle].Add(sce);
            }
            else
            {
                nonStyleEffects.Add(effect);
            }
        }

        // First emit entries for each style group
        foreach (var kv in styleGroups)
        {
            var style = kv.Key;
            var list = kv.Value;
            if (list == null || list.Count == 0) continue;

            // Aggregate by stat
            var statTotals = new Dictionary<StatsEnum, int>();
            int duration = list[0].Duration;
            foreach (var s in list)
            {
                if (!statTotals.ContainsKey(s.Stat)) statTotals[s.Stat] = 0;
                statTotals[s.Stat] += s.Power;
            }

            // Build description text (join multiple stat lines with a space)
            var descParts = new List<string>();
            foreach (var st in statTotals)
            {
                var d = StatusEffectDescriptions.GetDefault(StatusEffectEnum.StatChange, st.Key, st.Value);
                descParts.Add(d);
            }

            var styleColor = CombatStyleUtility.GetStyleColor(style);
            var hex = ColorUtility.ToHtmlStringRGB(styleColor);
            string name = style.ToString();
            string desc = string.Join(" ", descParts);

            var entry = Instantiate(statusEffectEntryPrefab, statusEffectsContainer);
            var entryUI = entry.GetComponent<StatusEffectEntryUI>();
            if (entryUI != null)
            {
                var coloredName = $"Stance: <color=#{hex}>{style}</color>";
                entryUI.Populate(coloredName, desc, $"{duration} turns");
            }
        }

        // Then emit the remaining non-style effects using existing stacking logic
        var processed = new HashSet<StatusEffect>();

        foreach (var effect in nonStyleEffects)
        {
            if (processed.Contains(effect)) continue;

            var stackGroup = new List<StatusEffect> { effect };
            foreach (var other in nonStyleEffects)
            {
                if (other == effect || processed.Contains(other)) continue;
                if (CanStack(effect, other))
                    stackGroup.Add(other);
            }

            foreach (var e in stackGroup)
                processed.Add(e);

            var entry = Instantiate(statusEffectEntryPrefab, statusEffectsContainer);
            var entryUI = entry.GetComponent<StatusEffectEntryUI>();
            if (entryUI == null) continue;

            if (stackGroup.Count == 1)
            {
                entryUI.Populate(effect);
            }
            else if (effect is StatChangeEffect sce)
            {
                int totalPower = 0;
                foreach (var e in stackGroup)
                    totalPower += ((StatChangeEffect)e).Power;

                string statName  = StatusEffectDescriptions.GetStatDisplayName(sce.Stat);
                string direction = totalPower >= 0 ? "Up" : "Down";
                string name2     = $"{statName} {direction}";
                string desc      = StatusEffectDescriptions.GetDefault(StatusEffectEnum.StatChange, sce.Stat, totalPower);
                entryUI.Populate(name2, desc, $"{effect.Duration} turns");
            }
            else
            {
                string name2 = $"{StatusEffectDescriptions.GetDisplayName(effect)} x{stackGroup.Count}";
                entryUI.Populate(name2, effect.Description, $"{effect.Duration} turns");
            }
        }
    }

    private static bool CanStack(StatusEffect a, StatusEffect b)
    {
        if (a.statusEfectType != b.statusEfectType) return false;
        if (a.Duration       != b.Duration)         return false;
        if (a.DurationTrigger != b.DurationTrigger) return false;

        if (a is StatChangeEffect sa && b is StatChangeEffect sb)
            return sa.Stat == sb.Stat;

        // Non-StatChange effects of the same type stack freely
        return !(a is StatChangeEffect);
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
