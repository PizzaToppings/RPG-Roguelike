using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] InfoScreen infoScreen;

    [Space]
    [SerializeField] SkillIcon basicAttackIcon;
    [SerializeField] SkillIcon basicSkillIcon;
    [SerializeField] SkillIcon[] skillIcons;

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
    }

    public void StartTurn(Unit CurrentActiveUnit)
    {
        if (CurrentActiveUnit is Character)
        {
            var character = CurrentActiveUnit as Character;
            SetSkillIcons(character);
        }
    }

    public void SetSkillIcons(Character CurrentActiveUnit)
    {
        basicAttackIcon.SetOrUpdate(CurrentActiveUnit.basicAttack);
        basicSkillIcon.SetOrUpdate(CurrentActiveUnit.basicSkill);

        for (int i = 0; i < CurrentActiveUnit.skills.Count; i++)
        {
            skillIcons[i].SetOrUpdate(CurrentActiveUnit.skills[i]);
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
}
