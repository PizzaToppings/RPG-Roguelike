using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Drives the Rest Zone scene.
/// On Start it applies healing to every party member and updates the HP display.
/// Wire the Leave button's OnClick to <see cref="Leave"/>.
///
/// Scene setup:
///   - One GameObject with this component
///   - A "Leave" Button wired to RestZoneUI.Leave()
///   - (Optional) Assign partyMemberRows to show per-character HP
/// </summary>
public class RestZoneUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI healAmountLabel;

    void Start()
    {
        if (RunManager.Instance == null)
        {
            Debug.LogWarning("RestZoneUI: RunManager not found. Start from MainMenuScene or ensure RunManager GameObject exists.");
            return;
        }

        ApplyHealing();
    }

    void ApplyHealing()
    {
        int healAmount  = RunManager.Instance.RestHealAmount;
        bool isPercent  = RunManager.Instance.RestHealIsPercentage;

        foreach (var member in RunData.Party)
        {
            int maxHP   = member.Character.MaxHealth;
            int current = member.CurrentHitpoints > 0 ? member.CurrentHitpoints : maxHP;
            int heal    = isPercent ? Mathf.RoundToInt(maxHP * healAmount / 100f) : healAmount;

            member.CurrentHitpoints = Mathf.Min(current + heal, maxHP);
        }

        if (healAmountLabel != null)
        {
            string label = isPercent ? $"Your party recovers {healAmount}% HP." : $"Your party recovers {healAmount} HP.";
            healAmountLabel.text = label;
        }
    }

    /// <summary>Call this from the Leave button's OnClick event.</summary>
    public void Leave()
    {
        if (RunManager.Instance == null) return;
        RunManager.Instance.OnRestZoneLeft();
    }
}
