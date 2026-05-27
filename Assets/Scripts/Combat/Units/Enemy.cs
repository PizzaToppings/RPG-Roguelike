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
    BoardTile highlightedTargetTile; // Tile highlighted when showing enemy intent target

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
        
        bool isPlacementPhase = CharacterPlacementManager.Instance != null && CharacterPlacementManager.Instance.IsPlacementPhase;
        bool isPlayerTurn = UnitData.ActiveUnit != null && UnitData.ActiveUnit.Friendly;
        bool playerIsCastingSkillshot = UnitData.CurrentAction == CurrentActionKind.CastingSkillshot && isPlayerTurn;

        if (isPlacementPhase || (isPlayerTurn && !playerIsCastingSkillshot))
        {
            boardManager.ShowEnemyThreatRange(this);
        }

        // Show damage prediction if player is actively aiming a skillshot at this enemy
        if (ThisHealthbar is FloatingHealthbar floatingHealthbar && playerIsCastingSkillshot)
        {
            floatingHealthbar.ShowDamagePreview(SkillData.CurrentActiveSkill);
        }

        // Show enemy intent: highlight target and show expected damage
        if (this is EnemyBaseAI aiEnemy && aiEnemy.CurrentSkill != null)
        {
            // Determine the target based on the enemy's targeting preference
            Unit predictedTarget = null;
            if (UnitData.Characters != null && UnitData.Characters.Count > 0)
            {
                // Filter characters that are within the enemy's threat range
                var maxRange = aiEnemy.CurrentSkill.OptimalRange + MoveSpeed;
                var targetCandidates = UnitData.Characters
                    .Where(c => c != null && c.Tile != null && 
                           boardManager.GetRangeBetweenTiles(Tile, c.Tile) <= maxRange)
                    .ToList();

                if (targetCandidates.Count > 0)
                {
                    predictedTarget = aiEnemy.GetTargetPreference(aiEnemy.CurrentSkill.TargetPreference, targetCandidates);
                }
            }

            if (predictedTarget != null)
            {
                // Highlight the target's tile
                highlightedTargetTile = predictedTarget.Tile;
                if (highlightedTargetTile != null)
                {
                    var targetColor = boardManager.GetTileColor(TileColorKind.EnemyIntent);
                    targetColor.FillCenter = true; // Make it more visible
                    highlightedTargetTile.SetColor(targetColor);
                }

                // Show damage preview on target's healthbar
                if (predictedTarget.ThisHealthbar is FloatingHealthbar targetHealthbar)
                {
                    targetHealthbar.ShowDamagePreviewFromEnemy(aiEnemy);
                }
            }
        }
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
        boardManager.ClearEnemyThreatRange();

        // Hide damage prediction on this enemy (from player targeting)
        if (ThisHealthbar is FloatingHealthbar floatingHealthbar)
        {
            floatingHealthbar.HideDamagePreview();
        }

        // Clear enemy intent visualization
        if (highlightedTargetTile != null)
        {
            // Reset the target tile color
            var originalColor = boardManager.GetTileColor(TileColorKind.Original);
            highlightedTargetTile.OverrideColor(originalColor);
            
            // Reapply tile effect color if present
            if (highlightedTargetTile.hasTileEffect && highlightedTargetTile.tileEffectColor != null)
                highlightedTargetTile.SetColor(highlightedTargetTile.tileEffectColor);
        }

        // Hide damage preview on any character healthbars
        if (this is EnemyBaseAI && UnitData.Characters != null)
        {
            foreach (var character in UnitData.Characters)
            {
                if (character?.ThisHealthbar is FloatingHealthbar targetHealthbar)
                {
                    targetHealthbar.HideDamagePreview();
                }
            }
        }
        
        highlightedTargetTile = null;
    }

    public void UnTargetEnemy()
	{
        ui_Singletons.SetCursor(CursorType.Normal);

        // Hide damage prediction
        if (ThisHealthbar is FloatingHealthbar floatingHealthbar)
        {
            floatingHealthbar.HideDamagePreview();
        }

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
