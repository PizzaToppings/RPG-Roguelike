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

    void Update()
    {
        if (CombatData.CurrentActiveUnit == this)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("ending turn: " + this);
                EndTurn();
            }
        }
    }

    public override void StartTurn()
    {
        base.StartTurn();

    }
}
