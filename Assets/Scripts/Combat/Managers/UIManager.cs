using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] InfoScreen infoScreen;
    [SerializeField] SkillIcon mainSkillIcon;
    [SerializeField] SkillIcon[] skillIcons;

    [SerializeField] Texture2D DefaultCursorTexture;
    [SerializeField] Texture2D MeleeAttackCursorTexture;
    [SerializeField] Texture2D RangedAttackCursorTexture;

    CursorMode cursorMode = CursorMode.ForceSoftware;


    public void Init()
    {
        Instance = this;
        InitUI();

        Cursor.SetCursor(DefaultCursorTexture, Vector2.zero, cursorMode);
    }

    void InitUI()
    {
        infoScreen.Init();
        mainSkillIcon.Init();

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
        mainSkillIcon.Set(CurrentActiveUnit.basicSkill);

        for (int i = 0; i < CurrentActiveUnit.skills.Count; i++)
        {
            skillIcons[i].Set(CurrentActiveUnit.skills[i]);
        }
    }

    public void ReadySkill(int skillIndex, bool active)
    {
        skillIcons[skillIndex].SetActiveColor(active);
    }

    public void SetCursor(Unit target, bool enter)
	{
        Texture2D texture;

        if (target is Enemy)
		{
            if (UnitData.CurrentAction == CurrentActionKind.Basic)
		    {
                //if (target.currentTile.connectedTiles.Contains())
		    }
		}

        if (target is Enemy && enter)
            texture = MeleeAttackCursorTexture;
        else
            texture = DefaultCursorTexture;

        Cursor.SetCursor(texture, Vector2.zero, cursorMode);
    }
}
