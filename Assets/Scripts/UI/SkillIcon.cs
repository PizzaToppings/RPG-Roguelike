using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class SkillIcon : MonoBehaviour, IPointerClickHandler
{
    SkillsManager skillsManager;
    UIManager uiManager;

    Skill skill;
    Character character;

    public Image icon;

    [Space]
    [SerializeField] GameObject ActiveBorder;
    [SerializeField] GameObject chargesImage;
    [SerializeField] GameObject energyCostImage;
    [SerializeField] TextMeshProUGUI chargesText;
    [SerializeField] TextMeshProUGUI EnergycostText;

    public void Init()
    {
        skillsManager = SkillsManager.Instance;
        uiManager = UIManager.Instance;
    }

    public void SetOrUpdate(Skill thisSkill, Character thisCharacter) 
    {
        skill = thisSkill;
        character = thisCharacter;
        SetIcon();
        SetCharges();
        SetEnergycost();
    }

    void SetIcon()
    {
        icon.gameObject.SetActive(true);

        if (skillsManager.CanCastSkill(skill, UnitData.ActiveUnit))
            icon.sprite = skill.mainSkillSO.Image;
        else if (skill.mainSkillSO.Image_Inactive != null)
            icon.sprite = skill.mainSkillSO.Image_Inactive;
    }

    void SetCharges()
    {
        if (chargesImage == null)
            return;

        if (skill.DefaultCharges == 1)
        {
            chargesImage.SetActive(false);
            chargesText.gameObject.SetActive(false);
        }
        else
        {
            chargesImage.SetActive(true);
            chargesText.gameObject.SetActive(true);
            chargesText.text = SkillData.GetCharges(skill).ToString();
        }
    }

    void SetEnergycost()
    {
        if (EnergycostText == null)
            return;

        energyCostImage.SetActive(true);
        EnergycostText.text = skill.EnergyCost.ToString();

        if (character.Energy >= skill.EnergyCost)
            EnergycostText.color = uiManager.energyTextAvailable;
        else
            EnergycostText.color = uiManager.energyTextUnavailable;
    }

    public void UpdateActiveBorder(Skill activeSkill)
    {
        if (ActiveBorder != null)
            ActiveBorder.SetActive(skill != null && skill == activeSkill);
    }

    public void SetDisabled()
    {
        icon.gameObject.SetActive(true);
        icon.sprite = uiManager.disabledSkillSprite;

        if (chargesImage != null)
            chargesImage.SetActive(false);
        if (chargesText != null)
            chargesText.gameObject.SetActive(false);
        energyCostImage.SetActive(false);

        if (ActiveBorder != null)
            ActiveBorder.SetActive(false);
    }

    public void Clear()
	{
        icon.gameObject.SetActive(false);
        if (ActiveBorder != null)
            ActiveBorder.SetActive(false);
    }

    public void CastSkill()
    {
        character.ToggleSkill(skill);
    }

    public void OnPointerEnter() 
    {
        if (skill == null)
            return;

        uiManager.StartShowSkillInformation(skill);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!CombatData.IsReady)
            return;

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (skill == null)
                return;

            CastSkill();
        }

        if (eventData.button == PointerEventData.InputButton.Right)
        {
            uiManager.LockSkillInformation(skill);
        }
    }

    public void OnPointerExit() 
    {
        if (skill == null)
            return; 
        
        uiManager.EndShowSkillInformation(skill);
    }
}
