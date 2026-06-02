using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterSelectCard : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] Image classIconOne;
    [SerializeField] Image classIconTwo;
    [SerializeField] TextMeshProUGUI SpeedText;
    [SerializeField] TextMeshProUGUI InitiativeText;
    [SerializeField] TextMeshProUGUI PowerText;
    [SerializeField] TextMeshProUGUI DefenseText;
    [SerializeField] Image           portrait;

    [Space]
    [SerializeField] CharacterSelectSkillIcon basicAttackIcon;
    [SerializeField] CharacterSelectSkillIcon basicSkillIcon;
    //[SerializeField] CharacterSelectSkillIcon SkillIconThree;

    [Space]
    [SerializeField] Button          selectButton;

    SO_Character characterData;

    Skill basicAttack = new Skill();
    Skill basicSkill = new Skill();

    public void Setup(SO_Character character)
    {
        characterData = character;

        nameText.text    = character.Name;
        if (portrait != null) portrait.sprite = character.Image;
        classIconOne.sprite = CharacterSelectUI.Instance.GetClassIcon(character.Classes[0]);
        classIconTwo.sprite = CharacterSelectUI.Instance.GetClassIcon(character.Classes[1]);

        SpeedText.text = character.MoveSpeed.ToString();
        PowerText.text = character.Power.ToString();
        DefenseText.text = character.Defense.ToString();

        selectButton.onClick.RemoveAllListeners();
        selectButton.onClick.AddListener(OnSelect);

        basicAttack.Init(characterData.basicAttack);
        basicSkill.Init(characterData.basicSkill);

        basicAttackIcon.Set(basicAttack);
        basicSkillIcon.Set(basicSkill);
    }

    void OnSelect()
    {
        RunManager.Instance.SelectCharacter(characterData);
    }
}
