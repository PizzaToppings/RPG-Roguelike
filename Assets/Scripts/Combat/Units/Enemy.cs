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

    public override void SetStats()
    {
        if (enemySO != null)
        {
            UnitName         = enemySO.Name;
            MaxHitpoints     = enemySO.MaxHealth;
            Hitpoints        = enemySO.MaxHealth;
            MoveSpeed        = enemySO.MoveSpeed;
            Power            = enemySO.Power;
            Armor            = enemySO.Armor;
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
        boardManager.ShowEnemyThreatRange(this);

        var aiEnemy = this as EnemyBaseAI;
        if (aiEnemy != null && aiEnemy.NextSkillPreviewTiles != null && aiEnemy.NextSkillPreviewTiles.Count > 0)
            boardManager.ShowEnemySkillPreview(aiEnemy.NextSkillPreviewTiles);
    }

    void HideEnemyThreat()
    {
        boardManager.ClearEnemySkillPreview();
    }

	public override void EndTurn()
    {
        base.EndTurn();
    }
}
