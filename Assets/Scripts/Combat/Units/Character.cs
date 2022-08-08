﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : Unit
{
    public override void Init()
    {
        Friendly = true;
        base.Init();
    }

    public override void Update()
    {
        if (UnitData.CurrentActiveUnit == this)
        {
            base.Update();

            if (Input.GetKeyDown(KeyCode.Space))
                EndTurn();

            UseSkills();
        }
    }

    void UseSkills()
    {
        if (UnitData.CurrentActiveUnit != this)
            return;

        if (Input.GetKeyDown(KeyCode.Alpha1))
            ToggleSkills(1);

        if (Input.GetKeyDown(KeyCode.Alpha2))
            ToggleSkills(2);
    }

    void ToggleSkills(int skillIndex)
    {
        if (SkillshotData.CurrentSkillshotIndex == skillIndex)
        {
            UnitData.CurrentAction = UnitData.CurrentActionKind.Moving; 
            SkillshotData.CurrentSkillshotIndex = null;
            boardManager.SetAOE(MoveSpeedLeft, currentTile, null);
        }
        else
        {

            if (skillshots[skillIndex-1].MagicalDamage && statusEffects.Find(x => x.statusEfectType == StatusEfectEnum.Silenced) != null)
                return;

            UnitData.CurrentAction = UnitData.CurrentActionKind.CastingSkillshot; 
            SkillshotData.CurrentSkillshotIndex = skillIndex;
            boardManager.Clear();
        }
    }

    public override void PreviewSkills(BoardTile mouseOverTile)
    {   
        base.PreviewSkills(mouseOverTile);

        for (int i = 0; i < SkillshotsEquipped.Count; i++)
        {
            if (SkillshotData.CurrentSkillshotIndex == i + 1 && SkillshotsEquipped[i])
            {
                skillshots[i].Preview(mouseOverTile);
            }
        }
    }

    public override void StartTurn()
    {
        base.StartTurn();
        UnitData.CurrentAction = UnitData.CurrentActionKind.Moving;
    }
}
