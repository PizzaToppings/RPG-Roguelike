using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Unit
{
    public SO_Enemy enemySO;

    [HideInInspector] public List<EnemyAbility> abilities = new List<EnemyAbility>();
    [HideInInspector] public List<BoardTile> PossibleMovementTiles;
    [HideInInspector] public int encounterTurnOrder = 0; // Turn order index set by EncounterManager

    BoardTile closestTile;

    public override void SetStats()
    {
        if (enemySO != null)
        {
            UnitName        = enemySO.Name;
            MaxHitpoints    = enemySO.MaxHealth;
            Hitpoints       = enemySO.MaxHealth;
            MoveSpeed       = enemySO.MoveSpeed;
            PhysicalPower   = enemySO.PhysicalPower;
            MagicalPower    = enemySO.MagicalPower;
            PhysicalDefense = enemySO.PhysicalDefense;
            MagicalDefense  = enemySO.MagicalDefense;
        }
        else
        {
            base.SetStats();
        }
    }

    public override void RollInitiative()
    {
        // Initiative is now set by CombatManager based on encounter turn order configuration
        // This method is kept for compatibility but does nothing
        Initiative = 0;
    }

    public override void Init()
    {
        PossibleMovementTiles = new List<BoardTile>();
        Friendly = false;
        base.Init();
        InitAbilities();
    }

    public void InitAbilities()
    {
        abilities.Clear();
        if (enemySO == null) return;
        foreach (var so in enemySO.Abilities)
        {
            if (so == null) continue;
            var ability = new EnemyAbility();
            ability.Init(so, this);
            abilities.Add(ability);
        }
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
        UnitData.CurrentAction = CurrentActionKind.Animating;

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
        EnemyInfoPanelManager.Instance?.ShowPanel(this);
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

        ui_Singletons.SetCursor(SkillData.CurrentActiveSkill.mainSkillSO.Cursor);
            
        if (attackRange > 1.5f) // so more than melee
            skillVFXManager.PreviewProjectileLine(closestTile.transform.position, transform.position, 1);
    }

    public override void OnClick()
	{
        base.OnClick();

        if (UnitData.CurrentAction == CurrentActionKind.Basic/* || UnitData.CurrentAction == CurrentActionKind.CastingSkillshot*/)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) && SkillData.GetCharges(SkillData.CurrentActiveSkill) != 0)
            {
                skillVFXManager.EndProjectileLine();
                UnitData.CurrentAction = CurrentActionKind.Animating;
                SkillData.Caster = UnitData.ActiveUnit;
                StartCoroutine(AttackEnemyBasicAttack());
            }
        }
    }

    IEnumerator AttackEnemyBasicAttack()
	{
        var attackRange = skillsManager.GetSkillAttackRange();
        if (boardManager.GetTilesInDirectAttackRange(Tile, attackRange, true).Any(x => x.currentUnit == UnitData.ActiveUnit) == false)
            yield return StartCoroutine(boardManager.MoveToTile());

        var character = UnitData.ActiveUnit as Character;
        var basicAttack = character.basicAttack;
        SkillData.SetCharges(basicAttack, SkillData.GetCharges(basicAttack) - 1);
        yield return StartCoroutine(skillsManager.CastSkill(basicAttack, character));
    }

    public override void OnMouseExit()
    {
        Tile.UnTarget();
        EnemyInfoPanelManager.Instance?.HidePanel();
    }

    public void UnTargetEnemy()
	{
        ui_Singletons.SetCursor(CursorType.Normal);

        if (closestTile != null && 
            (UnitData.CurrentAction == CurrentActionKind.Basic || UnitData.CurrentAction == CurrentActionKind.CastingSkillshot))
        {
            var tile = closestTile;
            closestTile = null;
            tile.UnTarget();
            SkillData.Reset();
            skillVFXManager.EndProjectileLine();
        }
    }

	public override void EndTurn()
    {
        base.EndTurn();
    }
}
