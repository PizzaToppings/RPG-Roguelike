using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] InfoScreen infoScreen;
    [SerializeField] GameObject ActivityBar;

    [Space]
    [SerializeField] SkillIcon basicAttackIcon;
    [SerializeField] SkillIcon basicSkillIcon;
    [SerializeField] SkillIcon[] skillIcons;
    [SerializeField] SkillIcon[] consumableIcons;

    [Space]
    [SerializeField] TextMeshProUGUI EnergyText;
    public Color energyTextAvailable;
    public Color energyTextUnavailable;

    [Space]
    public Sprite disabledSkillSprite;

    [Space]
    [SerializeField] List<CharacterPortrait> CharacterPortraits;

    Coroutine showSkillCoroutine;

    public void CreateInstance()
    {
        Instance = this;
    }

    public void Init()
    {
        basicAttackIcon.Init();
        basicSkillIcon.Init();

        for (var i = 0; i < UnitData.Characters.Count; i++)
		{
            CharacterPortraits[i].gameObject.SetActive(true);
            CharacterPortraits[i].thisUnit = UnitData.Characters[i];
            CharacterPortraits[i].Set();
            CharacterPortraits[i].UpdateHealthbar();
            UnitData.Characters[i].ThisHealthbar = CharacterPortraits[i];
        }

        foreach(var skillIcon in skillIcons)
            skillIcon.Init();

        foreach (var consumableIcon in consumableIcons)
            consumableIcon.Init();
    }

    public void StartTurn(Unit CurrentActiveUnit)
    {
        if (CurrentActiveUnit is Character)
        {
            var character = CurrentActiveUnit as Character;
            SetSkillIcons(character);
            SetConsumableIcons(character);
        }
        else
        {
            SetDisabledSkillIcons();
        }
    }

    public void SetDisabledSkillIcons()
    {
        basicAttackIcon.SetDisabled();
        basicSkillIcon.SetDisabled();

        foreach (var skillIcon in skillIcons)
            skillIcon.SetDisabled();
    }

    public void SetSkillIcons(Character CurrentActiveUnit)
    {
        basicAttackIcon.SetOrUpdate(CurrentActiveUnit.basicAttack, CurrentActiveUnit);
        basicSkillIcon.SetOrUpdate(CurrentActiveUnit.basicSkill, CurrentActiveUnit);

        for (int i = 0; i < CurrentActiveUnit.skills.Count; i++)
        {
            var skill = CurrentActiveUnit.skills[i];

            if (skill != null)
                skillIcons[i].SetOrUpdate(skill, CurrentActiveUnit);
            else
                skillIcons[i].Clear();
        }
    }

    public void SetConsumableIcons(Character CurrentActiveUnit)
    {
        for (int i = 0; i < CurrentActiveUnit.consumables.Count; i++)
        {
            var consumable = CurrentActiveUnit.consumables[i];

            if (consumable != null)
                consumableIcons[i].SetOrUpdate(consumable, CurrentActiveUnit);
            else
                consumableIcons[i].Clear();
        }
    }

    public void TriggerActivityText(string activityName)
	{
        StartCoroutine(ShowActivityText(activityName));
	}

    public IEnumerator ShowActivityText(string activityName)
	{
        ActivityBar.SetActive(true);
        
        var image = ActivityBar.GetComponent<Image>();
        var text = ActivityBar.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        text.text = activityName;

        float alpha = 0;
        float fadespeed = 3;

        while (alpha < 1)
		{
            alpha += Time.deltaTime * fadespeed;
            image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
            text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);
            yield return null;
        }

        yield return new WaitForSeconds(1.8f);

        while (alpha > 0)
        {
            alpha -= Time.deltaTime * fadespeed;
            image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
            text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);
            yield return null;
        }

        ActivityBar.SetActive(true);
    }

    public void StartShowSkillInformation(Skill skill)
	{
        if (infoScreen.IsLocked == false)
            showSkillCoroutine = StartCoroutine(ShowSkillInformation(skill));
	}

    public void LockSkillInformation(Skill skill)
    {
        infoScreen.Activate(skill, true);
    }

    public void EndShowSkillInformation(Skill skill)
    {
        StopCoroutine(showSkillCoroutine);

        infoScreen.Deactivate();
    }

    public IEnumerator ShowSkillInformation(Skill skill)
	{
        yield return new WaitForSeconds(1);
        infoScreen.Activate(skill, false);
    }

    public void SetActiveSkillBorder(Skill skill)
    {
        basicAttackIcon.UpdateActiveBorder(skill);
        basicSkillIcon.UpdateActiveBorder(skill);

        foreach (var icon in skillIcons)
            icon.UpdateActiveBorder(skill);

        foreach (var icon in consumableIcons)
            icon.UpdateActiveBorder(skill);
    }

    public void ResetSkills()
    {
        foreach (var skillIcon in skillIcons)
            skillIcon.Clear();
    }

    public void SetEnergy(int amount, int maxEnergy)
    {
        EnergyText.text = amount.ToString() + "/" + maxEnergy.ToString();
    }
}
