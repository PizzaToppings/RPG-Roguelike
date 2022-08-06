using System.Collections;
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
            {
                Debug.Log("ending turn: " + this);
                EndTurn();
            }

            UseSkills();
        }
    }

    void UseSkills()
    {
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
            UnitData.CurrentAction = UnitData.CurrentActionKind.CastingSkillshot; 
            SkillshotData.CurrentSkillshotIndex = skillIndex;
            boardManager.Clear();
        }
    }

    public override void PreviewSkills(BoardTile mouseOverTile)
    {   
        unitManager.ClearTargets();
        /// use forloop
        base.PreviewSkills(mouseOverTile);
        if (SkillshotData.CurrentSkillshotIndex == 1 && SkillshotsEquipped[0])
        {
            skillshots[0].Preview(mouseOverTile);
        }

        if (SkillshotData.CurrentSkillshotIndex == 2 && SkillshotsEquipped[1])
        {
            skillshots[1].Preview(mouseOverTile);
        }
    }

    public override void StartTurn()
    {
        base.StartTurn();
        UnitData.CurrentAction = UnitData.CurrentActionKind.Moving;
    }
}
