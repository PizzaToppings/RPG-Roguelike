using UnityEngine;
using TMPro;

/// <summary>
/// Populates a single status-effect row inside an info panel.
/// Attach this to the StatusEffectEntry prefab and wire up the three TMP fields.
/// </summary>
public class StatusEffectEntryUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI durationText;

    public void Populate(StatusEffect effect)
    {
        if (nameText != null)
            nameText.text = effect.statusEfectType.ToString();

        if (descriptionText != null)
            descriptionText.text = effect.Description;

        if (durationText != null)
            durationText.text = $"{effect.Duration} turns";
    }
}
