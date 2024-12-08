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
        currentTile.OnClick();
    }

    public override void OnMouseEnter()
	{
        currentTile.Target();
    }

    public void TargetEnemy()
	{
        var attackRange = skillsManager.GetBasicAttackRange();
        var tilesInAttackRange = TilesInAttackRange(attackRange);

        if (tilesInAttackRange != null && UnitData.CurrentAction == CurrentActionKind.Basic)
        {
            TargetEnemyBasicAttack(tilesInAttackRange, attackRange);
        }

        if (UnitData.CurrentAction == CurrentActionKind.CastingSkillshot)
        {
            uiManager.SetCursor(this, SkillData.CurrentActiveSkill.Cursor);
        }
    }
    
    void TargetEnemyBasicAttack(List<BoardTile> tilesInAttackRange, float attackRange)
    {
        closestTile = UnitData.CurrentActiveUnit.currentTile;

        if (CurrentUnitIsAdjacent() == false)
        {
            closestTile = tilesInAttackRange.FirstOrDefault();
            closestTile.Target();
        }

        if (SkillData.CurentSkillIsBasic())
        {
            var caster = UnitData.CurrentActiveUnit as Character;
            caster.basicSkill.SetTargetAndTile(this, currentTile);

            uiManager.SetCursor(this, SkillData.CurrentActiveSkill.Cursor);
            
            if (attackRange > 1.5f) // so more than melee
                skillFXManager.PreviewProjectileLine(closestTile.transform.position, transform.position);
        }
    }

    public override void OnClick()
	{
        base.OnClick();

        if (UnitData.CurrentAction == CurrentActionKind.Basic)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                skillFXManager.EndProjectileLine();
                UnitData.CurrentAction = CurrentActionKind.Animating;
                StartCoroutine(AttackEnemyBasicAttack());
            }
        }
    }

    IEnumerator AttackEnemyBasicAttack()
	{
        var attackRange = skillsManager.GetBasicAttackRange();
        if (TilesInAttackRange(attackRange).Any(x => x.currentUnit == UnitData.CurrentActiveUnit) == false)
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
        uiManager.SetCursor(this, CursorType.Normal);

		if (closestTile != null && UnitData.CurrentAction == CurrentActionKind.Basic)
		{
			closestTile.UnTarget();
		    SkillData.Reset();
            skillFXManager.EndProjectileLine();
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
