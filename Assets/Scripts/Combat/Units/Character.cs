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
        if (CombatData.CurrentActiveUnit == this)
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
        if (UnitData.CurrentSkillshot == skillIndex)
        {
            UnitData.CurrentAction = UnitData.CurrentActionKind.Moving; 
            UnitData.CurrentSkillshot = null;
            boardManager.SetMovementLeft(MoveSpeedLeft, currentTile, unitManager.movementColor);
        }
        else
        {
            UnitData.CurrentAction = UnitData.CurrentActionKind.CastingSkillshot; 
            UnitData.CurrentSkillshot = skillIndex;
            boardManager.Clear();
        }
    }

    public override void PreviewSkills(BoardTile mouseOverTile)
    {   
        /// use forloop
        base.PreviewSkills(mouseOverTile);
        if (UnitData.CurrentSkillshot == 1 && SkillshotsEquipped[0])
        {
            skillshots[0].Preview(mouseOverTile);
        }

        if (UnitData.CurrentSkillshot == 2 && SkillshotsEquipped[1])
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
