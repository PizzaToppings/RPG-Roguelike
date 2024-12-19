using UnityEngine;
using UnityEngine.UI;

public class SkillIcon : MonoBehaviour
{
    SkillsManager skillsManager;

    [SerializeField] int skillIndex;

    [HideInInspector] public InfoScreen infoScreen;
    [HideInInspector] public string infoText;

    SO_MainSkill skill;

    public Image icon;
    public Image backGround;
    public Button button; 

    public Color activeColor;
    public Color passiveColor;

   float infoDelay = 1.5f;
   float infoTimer = 0;
   bool isHovering = false;

    public void Init()
    {
        skillsManager = SkillsManager.Instance;
        infoScreen = InfoScreen.Instance;
        backGround = GetComponent<Image>();
    }

    public void Update()
    {
        ShowInformation();
    }

    public void SetOrUpdate(SO_MainSkill thisSkill) 
    {
        skill = thisSkill;
        infoText = skill.Description;
        SetIcon(thisSkill);
    }

    void SetIcon(SO_MainSkill thisSkill)
    {
        if (skillsManager.CanCastSkill(thisSkill))
            icon.sprite = skill.Image;
        else if (skill.Image_Inactive != null)
            icon.sprite = skill.Image_Inactive;
    }

    public void CastSkill()
    {
        Character caster = UnitData.CurrentActiveUnit as Character;
        caster.ToggleSkill(skill); // move to skillmanager?
    }

    public void SetActiveColor(bool active)
    {
        if (active)
            backGround.color = activeColor;
        else
            backGround.color = passiveColor;
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
