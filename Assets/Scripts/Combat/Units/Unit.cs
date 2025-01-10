using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Unit : UnitStats
{
    [HideInInspector] public CombatManager combatManager;
    [HideInInspector] public BoardManager boardManager;
    [HideInInspector] public UnitManager unitManager;
    [HideInInspector] public SkillsManager skillsManager;
    [HideInInspector] public SkillVFXManager skillVFXManager;
    [HideInInspector] public StatusEffectManager statusEffectManager;
    [HideInInspector] public DamageManager damageManager;
    [HideInInspector] public UIManager uiManager;
    [HideInInspector] public UI_Singletons ui_Singletons;

    [HideInInspector] public UnityEvent OnUnitTurnStartEvent = new UnityEvent();
    [HideInInspector] public UnityEvent OnUnitTurnEndEvent = new UnityEvent();
    [HideInInspector] public UnityEvent<DamagaDataResolved> OnUnitTakeDamageEvent = new UnityEvent<DamagaDataResolved>();

    [HideInInspector] public UnityEvent<DamageDataCalculated> OnDealDamage;
    [HideInInspector] public UnityEvent<DamageDataCalculated> OnTakeDamage;
    [HideInInspector] public Animator modelAnimator;

    [HideInInspector] public Healthbar ThisHealthbar;

    [HideInInspector] public Vector3 position => transform.position;

    // for testing and debugging
    public int startXPosition = 0;
    public int startYPosition = 0;

    public bool MouseOver;


    public virtual void Init()
    {
        combatManager = CombatManager.Instance;
        boardManager = BoardManager.Instance;
        unitManager = UnitManager.Instance;
        skillsManager = SkillsManager.Instance;
        skillVFXManager = SkillVFXManager.Instance;
        statusEffectManager = StatusEffectManager.Instance;
        damageManager = DamageManager.Instance;
        uiManager = UIManager.Instance;
        ui_Singletons = UI_Singletons.Instance;

        modelAnimator = transform.GetChild(0).GetComponent<Animator>();

        SetStats();
        RollInitiative();
    }

    public virtual void Update()
    {
    }

    public virtual void OnMouseEnter()
    {
    }

    public virtual void OnMouseDown()
    {
    }

    public virtual void OnMouseExit()
    {
        Tile.UnTarget();
    }

    public virtual void OnClick()
	{
	}

    public virtual void SetStats()
    {
        MoveSpeed = Random.Range(5, 15);
    }

    public virtual void RollInitiative() // Move to a manager?
    {
        if (Friendly)
            Initiative = Random.Range(0, 4);
        else
            Initiative = Random.Range(5, 10);
    }

    public IEnumerator Move(List<BoardTile> path)
    {
        modelAnimator.SetBool("Run", true);

        Vector3 startPosition = transform.position; 
        Vector3 endPosition;

        Tile.OnExitTile();

        for (int i = 0; i < path.Count; i++)
        {
            endPosition = path[i].transform.position;
            float distanceLeft = 0;

            Rotate(endPosition);

            while (distanceLeft <= 1)
            {
                // move to next tile
                distanceLeft += Time.deltaTime * (1f + MoveSpeed * 0.1f);
                transform.position = Vector3.Lerp(startPosition, endPosition, distanceLeft);

                yield return new WaitForEndOfFrame();
            }

            path[i].OnEnterTile(this);

            MoveSpeedLeft--;
            startPosition = endPosition;

            if (i != path.Count - 1)
                path[i].OnExitTile();
        }
        var endTile = path[path.Count-1];

        UnitData.CurrentAction = CurrentActionKind.Basic;
        boardManager.Clear();
        boardManager.SetAOE(MoveSpeedLeft, endTile, null);
        modelAnimator.SetBool("Run", false);
    }

    void Rotate(Vector3 endPosition) 
    {
        transform.LookAt(new Vector3(endPosition.x, transform.position.y, endPosition.z));
    }

    public virtual void SetStartOfTurnStats()
    {
        MoveSpeedLeft = MoveSpeed;
    }

    public virtual void PreviewSkills(BoardTile mouseOverTile)
    {
		boardManager.VisualClear();
	}

    public virtual DamagaDataResolved TakeDamage(DamageDataCalculated damageDataCalculated)
	{
        var shieldDamage = damageDataCalculated.Damage < ShieldPoints ? damageDataCalculated.Damage : ShieldPoints;
        var damage = 0;

        ShieldPoints -= shieldDamage;

        if (ShieldPoints == 0)
        {
            damage = damageDataCalculated.Damage - shieldDamage;
            Hitpoints -= damage;
        }

        var damagaDataResolved = new DamagaDataResolved
        {
            Attacker = damageDataCalculated.Caster,
            Target = this,
            DamageType = damageDataCalculated.DamageType,
            DamageDone = damage,
            ShieldDamage = shieldDamage,
            AttackRange = boardManager.GetRangeBetweenTiles(damageDataCalculated.Caster.Tile, Tile),
            IsMagical = damageDataCalculated.IsMagical
        };

        if (OnUnitTakeDamageEvent != null)
            OnUnitTakeDamageEvent.Invoke(damagaDataResolved);

        ThisHealthbar.UpdateHealthbar();

        return damagaDataResolved;
	}

    public virtual DamagaDataResolved Heal(DamageDataCalculated damageDataCalculated)
    {
        var correctedHealing = damageDataCalculated.Damage;
        Hitpoints += correctedHealing;
        if (Hitpoints > MaxHitpoints)
        {
            correctedHealing -= Hitpoints - MaxHitpoints;
            Hitpoints = MaxHitpoints;
        }

        var damagaDataResolved = new DamagaDataResolved
        {
            Attacker = damageDataCalculated.Caster,
            Target = this,
            DamageType = damageDataCalculated.DamageType,
            DamageDone = correctedHealing,
            ShieldDamage = 0,
            AttackRange = boardManager.GetRangeBetweenTiles(damageDataCalculated.Caster.Tile, Tile),
            IsMagical = damageDataCalculated.IsMagical
        };

        ThisHealthbar.UpdateHealthbar();

        return damagaDataResolved;
    }

    public virtual DamagaDataResolved Shield(DamageDataCalculated damageDataCalculated)
    {
        ShieldPoints += damageDataCalculated.Damage;

        var damagaDataResolved = new DamagaDataResolved
        {
            Attacker = damageDataCalculated.Caster,
            Target = this,
            DamageType = damageDataCalculated.DamageType,
            DamageDone = 0,
            ShieldDamage = -damageDataCalculated.Damage,
            AttackRange = boardManager.GetRangeBetweenTiles(damageDataCalculated.Caster.Tile, Tile),
            IsMagical = damageDataCalculated.IsMagical
        };

        ThisHealthbar.UpdateHealthbar();

        return damagaDataResolved;
    }

    public virtual IEnumerator StartTurn()
    {
        var turnStartText = $"Turn: {UnitData.ActiveUnit.UnitName}";

        var isStunned = statusEffectManager.UnitHasStatusEffect(this, StatusEfectEnum.Stunned);
        if (isStunned)
            turnStartText += " - stunned";

        var isIncapactated = statusEffectManager.UnitHasStatusEffect(this, StatusEfectEnum.Incapacitated);
        if (isIncapactated)
            turnStartText += " - incapacitated";

        uiManager.TriggerActivityText(turnStartText);

        boardManager.Clear();
        SetStartOfTurnStats();

        yield return new WaitForSeconds(1.5f);

        if (OnUnitTurnStartEvent != null)
            OnUnitTurnStartEvent.Invoke();

        if (isStunned || isIncapactated)
        {
            EndTurn();
            yield break;
        }

        if (statusEffectManager.UnitHasStatusEffect(this, StatusEfectEnum.Rooted) == false)
            boardManager.SetMovementAOE(MoveSpeedLeft, Tile);
    }

    public virtual void EndTurn()
    {
        if (OnUnitTurnEndEvent != null)
            OnUnitTurnEndEvent.Invoke();

        StartCoroutine(combatManager.EndTurn());
    }
}
