using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillIcon : MonoBehaviour
{
    SkillsManager skillsManager;

    [HideInInspector] public InfoScreen infoScreen;
    [HideInInspector] public string infoText;

    SO_MainSkill skill;

    public Image icon;

    [Space]
    [SerializeField] GameObject chargesImage;
    [SerializeField] TextMeshProUGUI chargesText;

   float infoDelay = 1.5f;
   float infoTimer = 0;
   bool isHovering = false;

    public void Init()
    {
        skillsManager = SkillsManager.Instance;
        infoScreen = InfoScreen.Instance;
    }

    public void Update()
    {
        ShowInformation();
    }

    public void SetOrUpdate(SO_MainSkill thisSkill) 
    {
        skill = thisSkill;
        infoText = skill.Description;
        SetIcon();
        SetCharges();
    }

    void SetIcon()
    {
        if (skillsManager.CanCastSkill(skill))
            icon.sprite = skill.Image;
        else if (skill.Image_Inactive != null)
            icon.sprite = skill.Image_Inactive;
    }

    void SetCharges()
    {
        if (skill.DafaultCharges == 1)
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

    public void CastSkill()
    {
        Character caster = UnitData.CurrentActiveUnit as Character;
        caster.ToggleSkill(skill);
    }

    public void OnPointerEnter() 
    {
        isHovering = true;
    }

    public void OnPointerClick()
    {
        CastSkill();
    }

    public void OnPointerExit() 
    {
        isHovering = false;
        infoScreen.SetActive(false);
    }

    void ShowInformation()
    {
        //if (isHovering && !infoScreen.IsActive)
        //{
        //    infoTimer += Time.deltaTime;
            
        //    if (infoTimer > infoDelay) 
        //    {
        //        infoScreen.SetActive(true);
        //    }
        //}
    }
}
