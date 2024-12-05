using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Unit
{
    [HideInInspector] public List<BoardTile> PossibleMovementTiles;

    BoardTile closestTile;

    public override void Init()
    {
        PossibleMovementTiles = new List<BoardTile>();
        Friendly = false;
        base.Init();
    }

    public override void Update()
    {
        if (UnitData.CurrentActiveUnit == this)
        {
            base.Update();
        }
    }

    public override IEnumerator StartTurn()
    {
        yield return null;
        Debug.Log("starting enemy turn");
        PossibleMovementTiles = new List<BoardTile>();
        StartCoroutine(base.StartTurn());

        //StartCoroutine(holdturnforasec());
    }

    IEnumerator holdturnforasec() // temp
    {
        yield return new WaitForSeconds(3);
        EndTurn();
    }

    public override void OnMouseDown()
	{
        if (UnitData.CurrentAction == CurrentActionKind.Basic)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                StartCoroutine(AttackEnemyBasicAttack());
            }
        }
    }

    public override void OnMouseEnter()
	{
        currentTile.Target();
    }

    public void TargetEnemyBasicAttack() // might adapt to work for all direct attacks (target one enemy, including ranged)
	{
        if (TilesInAttackRange() != null && UnitData.CurrentAction == CurrentActionKind.Basic)
        {
            uiManager.SetCursor(this, true);
            if (CurrentUnitIsAdjacent() == false)
			{
                closestTile = TilesInAttackRange().FirstOrDefault();
                closestTile.Target();

                if (SkillData.CurentSkillIsBasic())
				{
                    (UnitData.CurrentActiveUnit as Character).basicSkill.SetTargetAndTile(this, currentTile);
                }
            }
        }

        if (UnitData.CurrentAction == CurrentActionKind.CastingSkillshot)
        {
            uiManager.SetCursor(this, true);
        }
    }

    IEnumerator AttackEnemyBasicAttack()
	{
        if (CurrentUnitIsAdjacent() == false)
            yield return StartCoroutine(boardManager.MoveToTile());

        var basicSkill = (UnitData.CurrentActiveUnit as Character).basicSkill;
        yield return StartCoroutine(skillsManager.CastSkills(basicSkill));
    }

    public override void OnMouseExit()
    {
        currentTile.UnTarget();
    }

    public void UnTargetEnemy()
	{
        uiManager.SetCursor(this, false);

		if (closestTile != null && UnitData.CurrentAction == CurrentActionKind.Basic)
		{
			closestTile.UnTarget();
		    SkillData.Reset();
		}
	}

    bool CurrentUnitIsAdjacent()
    {
        return currentTile.connectedTiles.Where(x => x != null).Any(x => x.currentUnit == UnitData.CurrentActiveUnit);
    }

    public override void EndTurn()
    {
        Debug.Log("ending enemy turn");
        base.EndTurn();
    }
}
