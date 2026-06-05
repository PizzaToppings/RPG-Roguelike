using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterSelectCard : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] Image classIconOne;
    [SerializeField] TextMeshProUGUI ClassNameOne;
    [SerializeField] Image classIconTwo;
    [SerializeField] TextMeshProUGUI ClassNameTwo;
    [SerializeField] TextMeshProUGUI SpeedText;
    [SerializeField] TextMeshProUGUI HitpointsText;
    [SerializeField] TextMeshProUGUI PowerText;
    [SerializeField] TextMeshProUGUI ArmorText;
    [SerializeField] Image           portrait;
    [SerializeField] Image           traitIcon;

    [Space]
    [SerializeField] CharacterSelectSkillIcon basicAttackIcon;
    [SerializeField] CharacterSelectSkillIcon basicSkillIcon;

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
        ClassNameOne.text = character.Classes[0].ToString();

        classIconTwo.sprite = CharacterSelectUI.Instance.GetClassIcon(character.Classes[1]);
        ClassNameTwo.text = character.Classes[1].ToString();

        HitpointsText.text = character.MaxHealth.ToString();
        SpeedText.text = character.MoveSpeed.ToString();
        PowerText.text = character.Power.ToString();
        ArmorText.text = character.Armor.ToString();

        selectButton.onClick.RemoveAllListeners();
        selectButton.onClick.AddListener(OnSelect);

        basicAttack.Init(characterData.basicAttack);
        basicSkill.Init(characterData.basicSkill);

        // Pass the character's power so the info screen can show correct damage in character select
        basicAttackIcon.Set(basicAttack, character.Power);
        basicSkillIcon.Set(basicSkill, character.Power);

        // Trait icon (basic trait)
        if (character.BasicTrait != null && traitIcon != null)
        {
            traitIcon.gameObject.SetActive(true);
            if (character.BasicTrait.Image != null)
                traitIcon.sprite = character.BasicTrait.Image;

            // Attach or get helper to forward hover events to CharacterSelectUI
            var hover = traitIcon.GetComponent<CharacterSelectTraitIcon>();
            if (hover == null) hover = traitIcon.gameObject.AddComponent<CharacterSelectTraitIcon>();
            hover.Set(character.BasicTrait);
        }
    }

    void OnSelect()
    {
        RunManager.Instance.SelectCharacter(characterData);
    }
}
