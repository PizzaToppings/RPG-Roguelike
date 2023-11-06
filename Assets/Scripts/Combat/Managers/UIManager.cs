using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
            SetSkillIcons(CurrentActiveUnit);
        }
    }

    public void SetSkillIcons(Unit CurrentActiveUnit)
    {
        for (int i = 0; i < CurrentActiveUnit.skillshots.Count; i++)
        {
            skillIcons[i].Set(CurrentActiveUnit.skillshots[i]);
        }
    }

    public void ReadySkill(int skillIndex, bool active)
    {
        skillIcons[skillIndex].SetActiveColor(active);
    }
}
