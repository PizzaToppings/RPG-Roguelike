using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyInfoPanelManager : MonoBehaviour
{
    public static EnemyInfoPanelManager Instance;

    [Header("Panel Root")]
    [SerializeField] private GameObject enemyInfoPanel;

    [Header("Basic Info")]
    [SerializeField] private TextMeshProUGUI enemyNameText;
    [SerializeField] private TextMeshProUGUI physicalPowerText;
    [SerializeField] private TextMeshProUGUI magicalPowerText;
    [SerializeField] private TextMeshProUGUI physicalDefenseText;
    [SerializeField] private TextMeshProUGUI magicalDefenseText;
    [SerializeField] private TextMeshProUGUI resistancesText;
    [SerializeField] private TextMeshProUGUI vulnerabilitiesText;

    [Header("Intent (EnemyBaseAI only)")]
    [SerializeField] private GameObject intentSection;
    [SerializeField] private Image intentActionImage;
    [SerializeField] private Image intentTargetImage;
    [SerializeField] private TextMeshProUGUI intentDescriptionText;
    [SerializeField] private TextMeshProUGUI intentTargetDescriptionText;

    private RectTransform panelRect;
    private Canvas parentCanvas;
    private bool isVisible;

    private void Awake()
    {
        Instance = this;
        panelRect = enemyInfoPanel.GetComponent<RectTransform>();
        parentCanvas = enemyInfoPanel.GetComponentInParent<Canvas>();
        enemyInfoPanel.SetActive(false);
    }

    private void Update()
    {
        if (!isVisible) return;
    }

    /// <summary>Call from Enemy.OnMouseEnter to show the panel.</summary>
    public void ShowPanel(Enemy enemy)
    {
        if (enemyInfoPanel == null) return;

        PopulateStats(enemy);
        PopulateIntent(enemy as EnemyBaseAI);

        enemyInfoPanel.SetActive(true);
        isVisible = true;
    }

    /// <summary>Call from Enemy.OnMouseExit to hide the panel.</summary>
    public void HidePanel()
    {
        if (enemyInfoPanel == null) return;
        enemyInfoPanel.SetActive(false);
        isVisible = false;
    }

    private void PopulateStats(Enemy enemy)
    {
        if (enemyNameText != null)
            enemyNameText.text = enemy.UnitName;

        if (physicalPowerText != null)            physicalPowerText.text = enemy.PhysicalPower.ToString();
        if (magicalPowerText != null)             magicalPowerText.text = enemy.MagicalPower.ToString();
        if (physicalDefenseText != null)          physicalDefenseText.text = enemy.PhysicalDefense.ToString();
        if (magicalDefenseText != null)           magicalDefenseText.text = enemy.MagicalDefense.ToString();

        if (resistancesText != null)
        {
            bool hasResist = enemy.Resistances != null && enemy.Resistances.Count > 0;
            resistancesText.gameObject.SetActive(hasResist);
            if (hasResist)
                resistancesText.text = "Resists: " + FormatDamageTypeList(enemy.Resistances);
        }

        if (vulnerabilitiesText != null)
        {
            bool hasVuln = enemy.Vulnerabilities != null && enemy.Vulnerabilities.Count > 0;
            vulnerabilitiesText.gameObject.SetActive(hasVuln);
            if (hasVuln)
                vulnerabilitiesText.text = "Weak to: " + FormatDamageTypeList(enemy.Vulnerabilities);
        }
    }

    private void PopulateIntent(EnemyBaseAI aiEnemy)
    {
        if (intentSection == null) return;

        bool isAI = aiEnemy != null;
        intentSection.SetActive(isAI);

        if (!isAI) return;

        var skill = aiEnemy.CurrentSkill;
        if (skill == null) return;

        var ui = UI_Singletons.Instance;

        if (intentActionImage != null && ui != null)
            intentActionImage.sprite = ui.GetIntentActionIcon(skill.IntentAction);

        if (intentTargetImage != null && ui != null)
            intentTargetImage.sprite = ui.GetIntentTargetIcon(skill.GetIntentTarget());

        if (intentDescriptionText != null)
        {
            string desc = GetDefaultIntentDescription(skill.IntentAction);
            intentDescriptionText.gameObject.SetActive(!string.IsNullOrEmpty(desc));
            intentDescriptionText.text = desc;
        }

        if (intentTargetDescriptionText != null)
        {
            string targetDesc = GetDefaultIntentTargetDescription(skill.GetIntentTarget());
            intentTargetDescriptionText.gameObject.SetActive(!string.IsNullOrEmpty(targetDesc));
            intentTargetDescriptionText.text = targetDesc;
        }
    }

    private static string GetDefaultIntentDescription(IntentActionEnum action)
    {
        switch (action)
        {
            case IntentActionEnum.PhysicalMeleeAttack:  return "The enemy will deal physical melee damage.";
            case IntentActionEnum.PhysicalRangedAttack: return "The enemy will deal physical ranged damage.";
            case IntentActionEnum.MagicalMeleeAttack:   return "The enemy will deal magical melee damage.";
            case IntentActionEnum.MagicalRangedAttack:  return "The enemy will deal magical ranged damage.";
            case IntentActionEnum.Debuff:               return "The enemy will apply a debuff.";
            case IntentActionEnum.Buff:                 return "The enemy will buff itself or an ally.";
            case IntentActionEnum.Heal:                 return "The enemy will restore health.";
            case IntentActionEnum.AOE:                  return "The enemy will unleash an area of effect attack.";
            default:                                    return string.Empty;
        }
    }

    private static string GetDefaultIntentTargetDescription(IntentTargetEnum target)
    {
        switch (target)
        {
            case IntentTargetEnum.Nearest:      return "Targets the nearest unit.";
            case IntentTargetEnum.LowestHealth: return "Targets the unit with the lowest health.";
            case IntentTargetEnum.Area:         return "Targets an area.";
            case IntentTargetEnum.Self:         return "Targets itself.";
            case IntentTargetEnum.Random:       return "Targets a random unit.";
            default:                            return string.Empty;
        }
    }

    private void ClampToScreen()
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

    private static string FormatDamageTypeList(List<DamageTypeEnum> types)
    {
        var sb = new StringBuilder();
        for (int i = 0; i < types.Count; i++)
        {
            sb.Append(types[i].ToString());
            if (i < types.Count - 1)
                sb.Append(", ");
        }
        return sb.ToString();
    }
}
