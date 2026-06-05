using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] SkillInfoScreen infoScreen;
    [SerializeField] TraitInfoScreen traitInfoScreen;
    [SerializeField] GameObject ActivityBar;

    [Space]
    [SerializeField] SkillIcon basicAttackIcon;
    [SerializeField] SkillIcon basicSkillIcon;
    [SerializeField] SkillIcon[] skillIcons;
    [SerializeField] SkillIcon[] consumableIcons;
    [Space]
    [SerializeField] TraitIcon[] traitIcons;
    [Header("Trait Panel")]
    [Tooltip("Parent transform where runtime trait icon prefabs will be instantiated")]
    [SerializeField] Transform traitParent;
    [Tooltip("Prefab containing a TraitIcon component and an Image. Instantiated per trait at turn start.")]
    [SerializeField] GameObject traitIconPrefab;

    // Instances created at runtime for the active unit's traits (object pool)
    List<GameObject> spawnedTraitIcons = new List<GameObject>();
    Stack<GameObject> traitIconPool = new Stack<GameObject>();

    //[Space]
    //[SerializeField] TextMeshProUGUI EnergyText;
    //public Color energyTextAvailable;
    //public Color energyTextUnavailable;

    [Space]
    public Sprite disabledSkillSprite;

    Coroutine showSkillCoroutine;
    Coroutine showTraitCoroutine;

    public void CreateInstance()
    {
        Instance = this;
    }

    public void Init()
    {
        basicAttackIcon.Init();
        basicSkillIcon.Init();

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
            // Spawn runtime trait icons under the configured parent
            ShowTraitPanel(character);
        }
        else
        {
            SetDisabledSkillIcons();
            // Hide any runtime trait icons
            HideTraitPanel();
            ClearTraitIcons();
        }
    }

    public void SetDisabledSkillIcons()
    {
        basicAttackIcon.SetDisabled();
        basicSkillIcon.SetDisabled();

        foreach (var skillIcon in skillIcons)
            skillIcon.SetDisabled();
    }

    public void SetTraitIcons(Character CurrentActiveUnit)
    {
        if (traitIcons == null) return;

        // Character.traits contains runtime Trait instances which reference SO_Trait in traitSO
        for (int i = 0; i < traitIcons.Length; i++)
        {
            if (i < CurrentActiveUnit.traits.Count && CurrentActiveUnit.traits[i]?.traitSO != null)
            {
                traitIcons[i].Set(CurrentActiveUnit.traits[i].traitSO);
            }
            else
            {
                traitIcons[i].Clear();
            }
        }
    }

    public void ClearTraitIcons()
    {
        if (traitIcons == null) return;
        foreach (var t in traitIcons)
            t.Clear();
    }

    // Instantiate prefab instances for the active character's traits.
    // Each prefab should contain a TraitIcon component (and an Image).
    public void ShowTraitPanel(Character character)
    {
        // Clear any existing spawned icons first
        HideTraitPanel();

        if (traitParent == null || traitIconPrefab == null || character == null)
            return;

        // Character.traits contains runtime Trait instances which reference SO_Trait in traitSO
        float baseDelay = 0f;
        float stepDelay = 0.06f; // quick succession
        for (int i = 0; i < character.traits.Count; i++)
        {
            var t = character.traits[i];
            if (t?.traitSO == null)
                continue;

            GameObject go = null;
            if (traitIconPool.Count > 0)
            {
                go = traitIconPool.Pop();
                go.transform.SetParent(traitParent, false);
                go.SetActive(true);
            }
            else
            {
                go = Instantiate(traitIconPrefab, traitParent);
            }

            spawnedTraitIcons.Add(go);

            var icon = go.GetComponent<TraitIcon>();
            if (icon != null)
            {
                // The prefab's Image is already on the same GameObject; Set will assign sprite if available
                icon.Set(t.traitSO);
                // Animate in with staggered delay
                icon.AnimateIn(baseDelay + i * stepDelay);
            }
        }
    }

    // Destroy spawned runtime trait icons
    public void HideTraitPanel()
    {
        if (spawnedTraitIcons == null || spawnedTraitIcons.Count == 0)
            return;

        for (int i = spawnedTraitIcons.Count - 1; i >= 0; i--)
        {
            var go = spawnedTraitIcons[i];
            if (go == null) continue;

            // Return to pool rather than destroy
            go.SetActive(false);
            go.transform.SetParent(this.transform, false);
            traitIconPool.Push(go);
        }
        spawnedTraitIcons.Clear();
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
        showSkillCoroutine = StartCoroutine(ShowSkillInformation(skill));
	}

    public void StartShowTraitInformation(SO_Trait trait, RectTransform anchor = null)
    {
        showTraitCoroutine = StartCoroutine(ShowTraitInformation(trait, anchor));
    }

    public void EndShowTraitInformation(SO_Trait trait)
    {
        if (showTraitCoroutine != null) StopCoroutine(showTraitCoroutine);
        if (traitInfoScreen != null) traitInfoScreen.Deactivate();
    }

    public IEnumerator ShowTraitInformation(SO_Trait trait, RectTransform anchor = null)
    {
        yield return new WaitForSeconds(0.2f);
        traitInfoScreen?.Activate(trait, anchor);
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
        yield return new WaitForSeconds(0.2f);
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

    //public void SetEnergy(int amount, int maxEnergy)
    //{
    //    EnergyText.text = amount.ToString() + "/" + maxEnergy.ToString();
    //}
}
