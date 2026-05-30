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
        // Mark that an enemy has the active turn. Individual enemy actions may
        // set Animating temporarily when performing animations/attacks.
        UnitData.CurrentAction = CurrentActionKind.EnemyTurn;

        PossibleMovementTiles = new List<BoardTile>();
        yield return StartCoroutine(base.StartTurn());
    }

    public override void MouseEnter()
	{
        base.MouseEnter();
    }

    public override void Target()
    {
        base.Target();

        EnemyInfoPanelManager.Instance?.ShowPanel(this);

        ShowEnemyThreat();
    }

    public override void Untarget()
    {
        base.Untarget();

        EnemyInfoPanelManager.Instance?.HidePanel();
        boardManager.ClearEnemyThreatRange();

        HideEnemyThreat();
    }

    public override void OnClick()
	{
        base.OnClick();
    }

    public override void MouseExit()
    {
        Tile.UnTarget();

        
    }

    void ShowEnemyThreat()
    {
        if (UnitData.CurrentAction == CurrentActionKind.CharacterPlacement || UnitData.CurrentAction == CurrentActionKind.Basic)
        {
            boardManager.ShowEnemyThreatRange(this);
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

        // Show damage prediction if player is actively aiming a skillshot at this enemy
        if (ThisHealthbar is FloatingHealthbar floatingHealthbar && UnitData.CurrentAction == CurrentActionKind.CastingSkillshot)
        {
            floatingHealthbar.ShowDamagePreview(SkillData.CurrentActiveSkill);
        }
    }

    void HideEnemyThreat()
    {
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

        // Hide damage prediction on this enemy (from player targeting)
        if (ThisHealthbar is FloatingHealthbar floatingHealthbar)
        {
            floatingHealthbar.HideDamagePreview();
        }
    }

	public override void EndTurn()
    {
        base.EndTurn();
    }
}
