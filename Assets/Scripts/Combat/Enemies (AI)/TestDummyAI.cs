using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDummyAI : Enemy
{
    public override IEnumerator StartTurn()
    {
        StartCoroutine(base.StartTurn());
        yield return null;

        Debug.Log("starting enemy turn: " + UnitName);
        //FindOptimalTile();
        //yield return new WaitUntil(() => CurrentUnitAction == currentUnitAction.Nothing);
        yield return new WaitForSeconds(1);
        StartCoroutine(PassTurn());
    }

    public IEnumerator PassTurn()
	{
        uiManager.ShowActivityText("Pass turn");

        Debug.Log("passing enemy turn: " + UnitName);


        yield return new WaitForSeconds(2);

        Debug.Log("ending enemy turn: " + UnitName);

        EndTurn();
    }
}
