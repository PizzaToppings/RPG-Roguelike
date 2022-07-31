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
    }

    void ToggleSkills(int skillIndex)
    {
        if (UnitData.CurrentSkillshot == skillIndex)
        {
            UnitData.CurrentAction = UnitData.CurrentActionKind.Moving; 
            UnitData.CurrentSkillshot = null;
            boardManager.SetMovementLeft(MoveSpeedLeft, currentTile);
        }
        else
        {
            UnitData.CurrentAction = UnitData.CurrentActionKind.CastingSkillshot; 
            UnitData.CurrentSkillshot = skillIndex;
            boardManager.ClearMovement();
        }
    }

    public override void PreviewSkills()
    {   
        /// use forloop
        base.PreviewSkills();
        if (UnitData.CurrentSkillshot == 1 && SkillshotsEquipped[0])
        {
            skillshots[0].Preview();
        }
    }

    public override void StartTurn()
    {
        base.StartTurn();
        UnitData.CurrentAction = UnitData.CurrentActionKind.Moving;
    }
}
