using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : Unit
{
    public override void Awake()
    {
        Friendly = true;
        base.Awake();
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
        {
            BoardData.canMove = !BoardData.canMove;

            if (BoardData.canMove)
                boardManager.SetMovementLeft(MoveSpeedLeft, currentTile);
            else
                boardManager.ClearMovement();
        }
    }

    public override void StartTurn()
    {
        base.StartTurn();
        BoardData.canMove = true;
    }
}
