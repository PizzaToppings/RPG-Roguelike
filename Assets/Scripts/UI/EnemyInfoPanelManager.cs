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
            string desc = GetDefaultIntentDescription(skill.IntentAction, skill, aiEnemy);
            intentDescriptionText.gameObject.SetActive(!string.IsNullOrEmpty(desc));
            intentDescriptionText.text = desc;
        }

        if (intentTargetDescriptionText != null)
        {
            string targetDesc = GetDefaultIntentTargetDescription(skill.GetIntentTarget(), skill, aiEnemy);
            intentTargetDescriptionText.gameObject.SetActive(!string.IsNullOrEmpty(targetDesc));
            intentTargetDescriptionText.text = targetDesc;
        }
    }

    private static string GetDefaultIntentDescription(IntentActionEnum action, SO_EnemySkill skill, EnemyBaseAI aiEnemy)
    {
        bool isMagical = action == IntentActionEnum.MagicalMeleeAttack || action == IntentActionEnum.MagicalRangedAttack;
        int totalDamage = 0, totalHealing = 0, totalShielding = 0;

        if (skill?.Skill != null && skill.Skill.Count > 0 && aiEnemy != null)
        {
            foreach (var part in skill.Skill)
            {
                if (part?.DamageEffects == null) continue;
                foreach (var effect in part.DamageEffects)
                {
                    if (effect == null) continue;
                    switch (effect.HitType)
                    {
                        case HitTypeEnum.Healing: totalHealing  += effect.Power; break;
                        case HitTypeEnum.Shield:  totalShielding += effect.Power; break;
                        default:
                            int casterPower = aiEnemy.Power;
                            totalDamage += Mathf.Max(0, effect.Power + casterPower);
                            break;
                    }
                }
            }
        }

        switch (action)
        {
            case IntentActionEnum.PhysicalMeleeAttack:
                return totalDamage > 0
                    ? $"Deals {totalDamage} physical melee damage."
                    : $"Deals physical melee damage.";
            case IntentActionEnum.PhysicalRangedAttack:
                return totalDamage > 0
                    ? $"Deals {totalDamage} physical ranged damage."
                    : $"Deals physical ranged damage.";
            case IntentActionEnum.MagicalMeleeAttack:
                return totalDamage > 0
                    ? $"Deals {totalDamage} magical melee damage."
                    : $"Deals magical melee damage.";
            case IntentActionEnum.MagicalRangedAttack:
                return totalDamage > 0
                    ? $"Deals {totalDamage} magical ranged damage."
                    : $"Deals magical ranged damage.";
            case IntentActionEnum.Debuff:   return "Applies a debuff.";
            case IntentActionEnum.Buff:     return "Buffs itself or an ally.";
            case IntentActionEnum.Heal:
                return totalHealing > 0
                    ? $"Restores {totalHealing} health."
                    : "Restores health.";
            case IntentActionEnum.AOE:
                return totalDamage > 0
                    ? $"Unleashes an area of effect attack dealing {totalDamage} damage."
                    : $"Unleashes an area of effect attack.";
            default: return string.Empty;
        }
    }

    private static string GetDefaultIntentTargetDescription(IntentTargetEnum target, SO_EnemySkill skill, EnemyBaseAI aiEnemy)
    {
            float maxRange = skill.FirstPart?.MaxRange > 0 ? skill.FirstPart.MaxRange : skill.OptimalRange;
            var threatRange = maxRange + aiEnemy.MoveSpeed;

        switch (target)
        {
            case IntentTargetEnum.Nearest:      return $"Targets the nearest unit within {threatRange} range.";
            case IntentTargetEnum.LowestHealth: return $"Targets the unit with the lowest health within {threatRange} range.";
            case IntentTargetEnum.Area:         return $"Targets an area within {threatRange} range.";
            case IntentTargetEnum.Self:         return "Targets itself.";
            case IntentTargetEnum.Random:       return $"Targets a random unit within {threatRange}.";
            default:                            return string.Empty;
        }
    }
}

