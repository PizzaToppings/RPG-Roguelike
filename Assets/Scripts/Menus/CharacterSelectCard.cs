using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// One selectable card on the character selection screen.
///
/// Required child UI elements (wire up in Inspector):
///   - Name Text        : TextMeshProUGUI – character name
///   - Classes Text     : TextMeshProUGUI – comma-separated class list
///   - Stats Text       : TextMeshProUGUI – HP / Energy / Power / Defense / Speed summary
///   - Portrait         : Image           – character portrait (optional)
///   - Select Button    : Button          – confirm the selection
/// </summary>
public class CharacterSelectCard : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI classesText;
    [SerializeField] TextMeshProUGUI statsText;
    [SerializeField] Image           portrait;
    [SerializeField] Button          selectButton;

    SO_Character characterData;

    public void Setup(SO_Character character)
    {
        characterData = character;

        nameText.text    = character.Name;
        classesText.text = string.Join(", ", character.Classes);
        statsText.text   = $"HP {character.MaxHealth}  |  Energy {character.MaxEnergy}\n" +
                           $"Phys {character.PhysicalPower} / {character.PhysicalDefense}\n" +
                           $"Magic {character.MagicalPower} / {character.MagicalDefense}\n" +
                           $"Speed {character.MoveSpeed}  |  Init {character.Initiative}";

        selectButton.onClick.RemoveAllListeners();
        selectButton.onClick.AddListener(OnSelect);
    }

    void OnSelect()
    {
        RunManager.Instance.SelectCharacter(characterData);
    }
}
