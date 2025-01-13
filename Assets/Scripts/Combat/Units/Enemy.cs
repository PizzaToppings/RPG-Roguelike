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
        if (UnitData.ActiveUnit == this)
        {
            base.Update();
        }
    }

    public override IEnumerator StartTurn()
    {
        yield return null;
        PossibleMovementTiles = new List<BoardTile>();
        yield return StartCoroutine(base.StartTurn());
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
        var caster = UnitData.ActiveUnit;

        if (skillsManager.CanCastSkill(skill, caster) == false) 
            return;

        if (UnitData.CurrentAction == CurrentActionKind.Basic || UnitData.CurrentAction == CurrentActionKind.CastingSkillshot)
        {
            var attackRange = skillsManager.GetSkillAttackRange();
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

        if (UnitData.CurrentAction == CurrentActionKind.Basic || UnitData.CurrentAction == CurrentActionKind.CastingSkillshot)
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
        basicAttack.Charges--;
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

	public override void EndTurn()
    {
        base.EndTurn();
    }
}
