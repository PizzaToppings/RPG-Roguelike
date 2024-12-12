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
        var attackRange = -1f;

        if (UnitData.CurrentAction == CurrentActionKind.Basic || UnitData.CurrentAction == CurrentActionKind.CastingSkillshot)
        {
            attackRange = skillsManager.GetSkillAttackRange();
            if (attackRange == 0)
                return;

            var tilesInAttackRange = boardManager.GetTilesInAttackRange(currentTile, attackRange);
            if (tilesInAttackRange != null)
                TargetEnemyBasicAttack(tilesInAttackRange, attackRange);
        }
    }
    
    void TargetEnemyBasicAttack(List<BoardTile> tilesInAttackRange, float attackRange)
    {
        closestTile = UnitData.CurrentActiveUnit.currentTile;
        var skill = SkillData.CurrentActiveSkill;

        if (boardManager.GetTilesInAttackRange(currentTile, attackRange).Any(x => x.currentUnit == UnitData.CurrentActiveUnit) == false)
        {
            closestTile = tilesInAttackRange.FirstOrDefault();
            closestTile.PreviewAttackWithinRange();
        }

        skill.SetTargetAndTile(this, currentTile);

        uiManager.SetCursor(SkillData.CurrentActiveSkill.Cursor);
            
        if (attackRange > 1.5f) // so more than melee
            skillFXManager.PreviewProjectileLine(closestTile.transform.position, transform.position);
    }

    public override void OnClick()
	{
        base.OnClick();

        if (UnitData.CurrentAction == CurrentActionKind.Basic || UnitData.CurrentAction == CurrentActionKind.CastingSkillshot)
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
        var attackRange = skillsManager.GetSkillAttackRange();
        if (boardManager.GetTilesInAttackRange(currentTile, attackRange).Any(x => x.currentUnit == UnitData.CurrentActiveUnit) == false)
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
        uiManager.SetCursor(CursorType.Normal);

        if (closestTile != null && 
            (UnitData.CurrentAction == CurrentActionKind.Basic || UnitData.CurrentAction == CurrentActionKind.CastingSkillshot))
        {
            closestTile.UnTarget();
            SkillData.Reset();
            skillFXManager.EndProjectileLine();
        }
    }

    public override void EndTurn()
    {
        Debug.Log("ending enemy turn");
        base.EndTurn();
    }
}
