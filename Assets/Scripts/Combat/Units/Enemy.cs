﻿using System.Linq;
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
        if (UnitData.ActiveUnit == this)
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
        Tile.OnClick();
    }

    public override void OnMouseEnter()
	{
        Tile.Target();
    }

    public void TargetEnemy()
	{
        var skill = SkillData.CurrentActiveSkill;

        if (skillsManager.CanCastSkill(skill) == false) 
            return;

        var attackRange = 0f;

        if (UnitData.CurrentAction == CurrentActionKind.Basic || UnitData.CurrentAction == CurrentActionKind.CastingSkillshot)
        {
            attackRange = skillsManager.GetSkillAttackRange();
            if (attackRange == 0)
                return;

            var tilesInAttackRange = boardManager.GetTilesInDirectAttackRange(Tile, attackRange, true);
            if (tilesInAttackRange != null)
                TargetEnemyBasicAttack(tilesInAttackRange, attackRange);
        }
    }
    
    void TargetEnemyBasicAttack(List<BoardTile> tilesInAttackRange, float attackRange)
    {
        closestTile = UnitData.ActiveUnit.Tile;
        var skill = SkillData.CurrentActiveSkill;

        if (boardManager.GetTilesInDirectAttackRange(Tile, attackRange, true).Any(x => x.currentUnit == UnitData.ActiveUnit) == false)
        {
            closestTile = tilesInAttackRange.FirstOrDefault();
            closestTile.PreviewAttackWithinRange();
        }

        skill.SetTargetAndTile(this, Tile);

        ui_Singletons.SetCursor(SkillData.CurrentActiveSkill.Cursor);
            
        if (attackRange > 1.5f) // so more than melee
            skillVFXManager.PreviewProjectileLine(closestTile.transform.position, transform.position, 1);
    }

    public override void OnClick()
	{
        base.OnClick();

        if (UnitData.CurrentAction == CurrentActionKind.Basic/* || UnitData.CurrentAction == CurrentActionKind.CastingSkillshot*/)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) && SkillData.CurrentActiveSkill.Charges != 0)
            {
                skillVFXManager.EndProjectileLine();
                UnitData.CurrentAction = CurrentActionKind.Animating;
                StartCoroutine(AttackEnemyBasicAttack());
            }
        }
    }

    IEnumerator AttackEnemyBasicAttack()
	{
        var attackRange = skillsManager.GetSkillAttackRange();
        if (boardManager.GetTilesInDirectAttackRange(Tile, attackRange, true).Any(x => x.currentUnit == UnitData.ActiveUnit) == false)
            yield return StartCoroutine(boardManager.MoveToTile());

        var basicAttack = (UnitData.ActiveUnit as Character).basicAttack;
        yield return StartCoroutine(skillsManager.CastSkill(basicAttack));
    }

    public override void OnMouseExit()
    {
        Tile.UnTarget();
    }

    public void UnTargetEnemy()
	{
        ui_Singletons.SetCursor(CursorType.Normal);

        if (closestTile != null && 
            (UnitData.CurrentAction == CurrentActionKind.Basic || UnitData.CurrentAction == CurrentActionKind.CastingSkillshot))
        {
            closestTile.UnTarget();
            SkillData.Reset();
            skillVFXManager.EndProjectileLine();
        }
    }

	public override void TakeDamage(int damage)
	{
		base.TakeDamage(damage);
	}

	public override void EndTurn()
    {
        Debug.Log("ending enemy turn");
        base.EndTurn();
    }
}
