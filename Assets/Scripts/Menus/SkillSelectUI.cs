using UnityEngine;

/// <summary>
/// Attach to a root GameObject in SkillSelectScene.
/// Populates the skill cards with randomly drawn options from the SkillPool,
/// filtered to skills that match the selected character's classes.
///
/// Scene setup:
///   - Create 3 UI card prefabs based on SkillSelectCard and assign them to the Cards array.
/// </summary>
public class SkillSelectUI : MonoBehaviour
{
    [SerializeField] SkillSelectCard[] cards;

    void Start()
    {
        if (RunManager.Instance == null)
        {
            Debug.LogWarning("SkillSelectUI: RunManager not found. Start from MainMenuScene or ensure RunManager GameObject exists.");
            return;
        }

        var options = RunManager.Instance.GetSkillOptions();

        if (options.Length == 0)
        {
            Debug.LogWarning("SkillSelectUI: No eligible skills available. Make sure SO_SkillPool has skills matching this character's classes.");
        }

        for (int i = 0; i < cards.Length; i++)
        {
            if (i < options.Length)
                cards[i].Setup(options[i]);
            else
                cards[i].gameObject.SetActive(false);
        }
    }
}
