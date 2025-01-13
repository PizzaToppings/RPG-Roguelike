using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDummyAI : Enemy
{
    public override IEnumerator StartTurn()
    {
        UnitData.CurrentAction = CurrentActionKind.Animating;

        yield return StartCoroutine(base.StartTurn());
        yield return null;

        yield return new WaitForSeconds(1);
        StartCoroutine(PassTurn());
    }

    public IEnumerator PassTurn()
	{
        StartCoroutine(uiManager.ShowActivityText("Pass turn"));

        yield return new WaitForSeconds(2);

        EndTurn();
    }
}
