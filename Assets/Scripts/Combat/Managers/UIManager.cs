using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    [SerializeField] List<CharacterPortrait> CharacterPortraits;

    [Space]
    [SerializeField] Texture2D DefaultCursorTexture;
    [SerializeField] Texture2D MeleeAttackCursorTexture;
    [SerializeField] Texture2D RangedAttackCursorTexture;
    [SerializeField] Texture2D SpellCursorTexture;
    [SerializeField] Texture2D CrossCursorTexture;

    CursorMode cursorMode = CursorMode.ForceSoftware;

    public void CreateInstance()
    {
        Instance = this;
    }

    public void Init()
    {
        InitUI();

        Cursor.SetCursor(DefaultCursorTexture, Vector2.zero, cursorMode);
    }

    void InitUI()
    {
        infoScreen.Init();
        basicAttackIcon.Init();
        basicSkillIcon.Init();

        for (var i = 0; i < UnitData.Characters.Count; i++)
		{
            CharacterPortraits[i].thisUnit = UnitData.Characters[i];
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
    }

    public void SetSkillIcons(Character CurrentActiveUnit)
    {
        basicAttackIcon.SetOrUpdate(CurrentActiveUnit.basicAttack);
        basicSkillIcon.SetOrUpdate(CurrentActiveUnit.basicSkill);

        for (int i = 0; i < CurrentActiveUnit.skills.Count; i++)
        {
            var skill = CurrentActiveUnit.skills[i];

            if (skill != null)
                skillIcons[i].SetOrUpdate(skill);
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
                consumableIcons[i].SetOrUpdate(consumable);
            else
                consumableIcons[i].Clear();
        }
    }

    public void SetCursor(CursorType cursorType)
	{
        Texture2D texture = DefaultCursorTexture;

        switch (cursorType)
        {
            case CursorType.Melee:
                texture = MeleeAttackCursorTexture;
                break;
            case CursorType.Ranged:
                texture = RangedAttackCursorTexture;
                break;
            case CursorType.Spell:
                texture = SpellCursorTexture;
                break;
            case CursorType.Cross:
                texture = CrossCursorTexture;
                break;
        }

        Cursor.SetCursor(texture, Vector2.zero, cursorMode);
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
}
