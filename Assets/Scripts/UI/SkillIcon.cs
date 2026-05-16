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
    [SerializeField] Material greyscaleMaterial;
    Material materialInstance; // Unique instance for this icon

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

        // Create a unique material instance for this icon
        if (greyscaleMaterial != null)
        {
            materialInstance = Instantiate(greyscaleMaterial);
            icon.material = materialInstance;
        }
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
        icon.sprite = skill.mainSkillSO.Image;

        if (materialInstance != null)
        {
            if (skillsManager.CanCastSkill(skill, UnitData.ActiveUnit))
                materialInstance.SetFloat("_GrayscaleAmount", 0f);
            else
                materialInstance.SetFloat("_GrayscaleAmount", 1f);
        }
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

    void OnDestroy()
    {
        // Clean up the material instance to prevent memory leaks
        if (materialInstance != null)
            Destroy(materialInstance);
    }
}
