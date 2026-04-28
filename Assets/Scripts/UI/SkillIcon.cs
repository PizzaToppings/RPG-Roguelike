using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillIcon : MonoBehaviour
{
    SkillsManager skillsManager;
    UIManager uiManager;

    SO_MainSkill skill;
    Character character;

    public Image icon;

    [Space]
    [SerializeField] GameObject ActiveBorder;
    [SerializeField] GameObject chargesImage;
    [SerializeField] TextMeshProUGUI chargesText;
    [SerializeField] TextMeshProUGUI EnergycostText;

    public void Init()
    {
        skillsManager = SkillsManager.Instance;
        uiManager = UIManager.Instance;
    }

    public void SetOrUpdate(SO_MainSkill thisSkill, Character thisCharacter) 
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
            icon.sprite = skill.Image;
        else if (skill.Image_Inactive != null)
            icon.sprite = skill.Image_Inactive;
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
        EnergycostText.text = skill.EnergyCost.ToString();

        if (character.Energy >= skill.EnergyCost)
            EnergycostText.color = uiManager.energyTextAvailable;
        else
            EnergycostText.color = uiManager.energyTextUnavailable;
    }

    public void UpdateActiveBorder(SO_MainSkill activeSkill)
    {
        if (ActiveBorder != null)
            ActiveBorder.SetActive(skill != null && skill == activeSkill);
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

    public void OnPointerClick()
    {
        if (skill == null)
            return; 
        
        CastSkill();
    }

    public void OnPointerExit() 
    {
        if (skill == null)
            return; 
        
        uiManager.EndShowSkillInformation(skill);
    }
}
