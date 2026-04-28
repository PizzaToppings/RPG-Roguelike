using UnityEngine;

/// <summary>
/// Attach to a root GameObject in CharacterSelectScene.
/// Populates the three character cards with randomly drawn options from the CharacterRoster.
///
/// Scene setup:
///   - Create 3 UI card prefabs based on CharacterSelectCard and assign them to the Cards array.
/// </summary>
public class CharacterSelectUI : MonoBehaviour
{
    [SerializeField] CharacterSelectCard[] cards;

    void Start()
    {
        if (RunManager.Instance == null)
        {
            Debug.LogWarning("CharacterSelectUI: RunManager not found in scene. Start from MainMenuScene or ensure RunManager GameObject exists.");
            return;
        }

        var options = RunManager.Instance.GetCharacterOptions();

        for (int i = 0; i < cards.Length; i++)
        {
            if (i < options.Length)
                cards[i].Setup(options[i]);
            else
                cards[i].gameObject.SetActive(false);
        }
    }
}
