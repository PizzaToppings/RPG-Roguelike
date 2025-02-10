using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class SkillIcon : MonoBehaviour, IPointerClickHandler
{
    SkillsManager skillsManager;
    UIManager uiManager;

    Skill skill;

    public Image icon;

    [Space]
    [SerializeField] GameObject chargesImage;
    [SerializeField] TextMeshProUGUI chargesText;

    public void Init()
    {
        skillsManager = SkillsManager.Instance;
        uiManager = UIManager.Instance;
    }

    public void SetOrUpdate(Skill thisSkill) 
    {
        skill = thisSkill;
        SetIcon();
        SetCharges();
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
            chargesText.text = skill.Charges.ToString();
        }
    }

    public void Clear()
	{
        icon.gameObject.SetActive(false);
    }

    public void CastSkill()
    {
        Character caster = UnitData.ActiveUnit as Character;
        caster.ToggleSkill(skill);
    }

    public void OnPointerEnter() 
    {
        if (skill == null)
            return;

        uiManager.StartShowSkillInformation(skill);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
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
