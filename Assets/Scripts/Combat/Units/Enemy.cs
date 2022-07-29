using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Unit
{
    public override void Awake()
    {
        Friendly = false;
        base.Awake();
    }

    public override void StartTurn()
    {
        Debug.Log("starting enemy turn");
        base.StartTurn();

        StartCoroutine(holdturnforasec());
    }

    IEnumerator holdturnforasec() 
    {
        yield return new WaitForSeconds(3);
        EndTurn();
    }

    public override void EndTurn()
    {
        Debug.Log("ending enemy turn");
        base.EndTurn();
    }
}
