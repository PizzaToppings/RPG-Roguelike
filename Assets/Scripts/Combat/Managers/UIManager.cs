using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    [SerializeField] InfoScreen infoScreen;
    [SerializeField] SkillIcon[] skillIcons;

    public void Init()
    {
        Instance = this;
        InitUI();
    }

    void InitUI()
    {
        infoScreen.Init();
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
        for (int i = 0; i < CurrentActiveUnit.skills.Count; i++)
        {
            skillIcons[i].Set(CurrentActiveUnit.skills[i]);
        }
    }

    public void ReadySkill(int skillIndex, bool active)
    {
        skillIcons[skillIndex].SetActiveColor(active);
    }
}
