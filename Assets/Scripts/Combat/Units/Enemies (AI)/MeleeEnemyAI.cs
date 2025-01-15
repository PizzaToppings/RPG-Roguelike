using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemyAI : EnemyBaseAI
{
    public override IEnumerator StartTurn()
    {
        yield return StartCoroutine(base.StartTurn());

        FindOptimalTile();
        yield return StartCoroutine(MoveToTile());

        yield return new WaitForSeconds(1);
        yield return StartCoroutine(Attack());
    }

    IEnumerator Attack()
	{
        Debug.Log("ATTACKING!");
        yield return new WaitForSeconds(1);
	}
}
