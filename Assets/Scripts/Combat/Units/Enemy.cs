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

    public override void OnMouseEnter()
	{
        IsTargeted = true;
        currentTile.Target();
    }

    public void TargetEnemy()
	{
        if (TilesInAttackRange() != null && UnitData.CurrentAction == CurrentActionKind.Basic)
        {
            uiManager.SetCursor(this, true);
            if (!currentTile.connectedTiles.Where(x => x != null).Any(x => x.currentUnit == UnitData.CurrentActiveUnit))
			{
                closestTile = TilesInAttackRange().FirstOrDefault();
                closestTile.Target();

                if (SkillData.CurrentActiveSkill == null)
				{
                    (UnitData.CurrentActiveUnit as Character).basicSkill.SetTargetAndTile(this, currentTile);
                }
            }
        }

        if (UnitData.CurrentAction == CurrentActionKind.CastingSkillshot)
        {
            uiManager.SetCursor(this, true);

            if (SkillData.CurrentActiveSkill == null)
			{
                //(UnitData.CurrentActiveUnit as Character).basicSkill.

            }
        }
    }

    public override void OnMouseExit()
    {
        IsTargeted = false;
        currentTile.UnTarget();
    }

    public void UnTargetEnemy()
	{
        uiManager.SetCursor(this, false);

		if (closestTile != null && UnitData.CurrentAction == CurrentActionKind.Basic)
			closestTile.UnTarget();

		if (UnitData.CurrentAction == CurrentActionKind.CastingSkillshot)
			currentTile.UnTarget();

		SkillData.Reset();
    }

    public override void EndTurn()
    {
        Debug.Log("ending enemy turn");
        base.EndTurn();
    }
}
