using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// One selectable card on the skill selection screen.
///
/// Required child UI elements (wire up in Inspector):
///   - Skill Name Text  : TextMeshProUGUI – skill name
///   - Classes Text     : TextMeshProUGUI – compatible classes (or "Universal")
///   - Description Text : TextMeshProUGUI – skill description
///   - Skill Icon       : Image           – skill icon sprite
///   - Energy Cost Text : TextMeshProUGUI – energy cost (optional)
///   - Select Button    : Button          – confirm the selection
/// </summary>
public class SkillSelectCard : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI skillNameText;
    [SerializeField] TextMeshProUGUI classesText;
    [SerializeField] TextMeshProUGUI descriptionText;
    [SerializeField] TextMeshProUGUI energyCostText;
    [SerializeField] Image           skillIcon;
    [SerializeField] Button          selectButton;

    SO_MainSkill skillData;

    public void Setup(SO_MainSkill skill)
    {
        skillData = skill;

        skillNameText.text   = skill.SkillName;
        classesText.text     = skill.Classes.Count > 0 ? string.Join(", ", skill.Classes) : "Universal";
        descriptionText.text = skill.Description;

        if (energyCostText != null)
            energyCostText.text = $"Cost: {skill.EnergyCost}";

        if (skillIcon != null && skill.Image != null)
            skillIcon.sprite = skill.Image;

        selectButton.onClick.RemoveAllListeners();
        selectButton.onClick.AddListener(OnSelect);
    }

    void OnSelect()
    {
        RunManager.Instance.SelectSkill(skillData);
    }
}
