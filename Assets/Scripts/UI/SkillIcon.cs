using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillIcon : MonoBehaviour
{
    [SerializeField] int skillIndex;

    public InfoScreen infoScreen;

   public Image icon;
   public Image backGround;
   public string infoText;
   public Button button; 

    public Color activeColor;
    public Color passiveColor;

   float infoDelay = 1.5f;
   float infoTimer = 0;
   bool isHovering = false;



    public void Init()
    {
        infoScreen = InfoScreen.Instance;
        backGround = GetComponent<Image>();
    }

    public void Update()
    {
        ShowInformation();
    }

    public void Set(SO_MainSkillshot skillShot) 
    {
        icon.sprite = skillShot.Image;
        infoText = skillShot.Description;
    }

    public void CastSkill()
    {
        Character caster = UnitData.CurrentActiveUnit as Character;
        caster.ToggleSkill(skillIndex);
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

    public void OnPointerExit() 
    {
        isHovering = false;
        infoScreen.SetActive(false);
    }

    void ShowInformation()
    {
        if (isHovering && !infoScreen.IsActive)
        {
            infoTimer += Time.deltaTime;
            
            if (infoTimer > infoDelay) 
            {
                infoScreen.SetActive(true);
            }
        }
    }
}
