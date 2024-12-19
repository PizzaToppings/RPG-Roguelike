using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] InfoScreen infoScreen;

    [Space]
    [SerializeField] SkillIcon mainSkillIcon;
    [SerializeField] SkillIcon[] skillIcons;

    [Space]
    [SerializeField] List<CharacterPortrait> CharacterPortraits;

    [Space]
    [SerializeField] Texture2D DefaultCursorTexture;
    [SerializeField] Texture2D MeleeAttackCursorTexture;
    [SerializeField] Texture2D RangedAttackCursorTexture;
    [SerializeField] Texture2D SpellCursorTexture;

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
        mainSkillIcon.Init();

        for(var i = 0; i < UnitData.Characters.Count; i++)
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
        mainSkillIcon.SetOrUpdate(CurrentActiveUnit.basicSkill);

        for (int i = 0; i < CurrentActiveUnit.skills.Count; i++)
        {
            skillIcons[i].SetOrUpdate(CurrentActiveUnit.skills[i]);
        }
    }

    public void ReadySkill(int skillIndex, bool active)
    {
        skillIcons[skillIndex].SetActiveColor(active);
    }

    public void SetCursor(CursorType cursorType)
	{
        Texture2D texture = DefaultCursorTexture;

  //      if (target is Enemy)
		//{
            if (cursorType == CursorType.Melee)
                texture = MeleeAttackCursorTexture;
            else if (cursorType == CursorType.Ranged)
                texture = RangedAttackCursorTexture;
            else if (cursorType == CursorType.Spell)
                texture = SpellCursorTexture;

            Cursor.SetCursor(texture, Vector2.zero, cursorMode);
		//}
    }
}
