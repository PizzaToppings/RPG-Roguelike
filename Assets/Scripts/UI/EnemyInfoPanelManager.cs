using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyInfoPanelManager : BaseInfoPanelManager
{
    public static EnemyInfoPanelManager Instance;

    [Header("Panel Root")]
    [SerializeField] private GameObject enemyInfoPanel;

    [Header("Intent (EnemyBaseAI only)")]
    [SerializeField] private GameObject intentSection;
    [SerializeField] private Image intentActionImage;
    [SerializeField] private Image intentTargetImage;
    [SerializeField] private TextMeshProUGUI intentDescriptionText;
    [SerializeField] private TextMeshProUGUI intentTargetDescriptionText;

    protected override GameObject PanelRoot => enemyInfoPanel;

    protected override void Awake()
    {
        Instance = this;
        base.Awake();
    }

    private void Update()
    {
        if (!isVisible) return;
    }

    /// <summary>Call from Enemy.OnMouseEnter to show the panel.</summary>
    public void ShowPanel(Enemy enemy)
    {
        if (enemyInfoPanel == null) return;

        PopulateBaseStats(enemy);
        PopulateIntent(enemy as EnemyBaseAI);
        PopulateStatusEffects(enemy);

        enemyInfoPanel.SetActive(true);
        isVisible = true;
    }

    /// <summary>Call from Enemy.OnMouseExit to hide the panel.</summary>
    public override void HidePanel()
    {
        base.HidePanel();
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
}

