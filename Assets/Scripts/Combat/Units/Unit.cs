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

    [HideInInspector] public UnityEvent OnTurnStart;
    [HideInInspector] public UnityEvent OnTurnEnd;

    [HideInInspector] public UnityEvent<DamageData> OnDealDamage;
    [HideInInspector] public UnityEvent<DamageData> OnTakeDamage;
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
        currentTile.UnTarget();
    }

    public virtual void OnClick()
	{
	}

    public virtual void SetStats()
    {
        // TODO: Get stats, not randomize
        UnitName = gameObject.name;
        MoveSpeed = Random.Range(5, 15);
    }

    public virtual void RollInitiative()
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

        currentTile.currentUnit = null;

        for (int i = 0; i < path.Count; i++)
        {
            endPosition = path[i].transform.position;
            float distanceLeft = 0;

            Rotate(endPosition);

            while (distanceLeft <= 1)
            {
                distanceLeft += Time.deltaTime * (1f + MoveSpeed * 0.1f);
                transform.position = Vector3.Lerp(startPosition, endPosition, distanceLeft);

                yield return new WaitForEndOfFrame();
            }

            MoveSpeedLeft--;
            startPosition = endPosition;
        }
        var endTile = path[path.Count-1];
        
        UnitData.CurrentActiveUnit.currentTile = endTile;
        endTile.currentUnit = UnitData.CurrentActiveUnit;

        UnitData.CurrentAction = CurrentActionKind.Basic;
        boardManager.Clear();
        boardManager.SetAOE(MoveSpeedLeft, endTile, null);
        modelAnimator.SetBool("Run", false);
    }

    void Rotate(Vector3 endPosition) 
    {
        transform.LookAt(new Vector3(endPosition.x, transform.position.y, endPosition.z));
    }

    void SetStartOfTurnStats()
    {
        MoveSpeedLeft = statusEffectManager.UnitHasStatuseffect(this, StatusEfectEnum.Rooted) == false ? MoveSpeed : 0;
        ReduceStatusEffects();
    }

    void ReduceStatusEffects()
    {
        var statusEffectToRemove = new  List<DefaultStatusEffect>();

        foreach (var statusEffect in statusEffects)
        {
            statusEffect.Duration--;
            if (statusEffect.Duration == 0)
                statusEffectToRemove.Add(statusEffect);
        }

        statusEffectToRemove.ForEach(x => statusEffects.Remove(x));
    }

    public virtual void PreviewSkills(BoardTile mouseOverTile)
    {
		boardManager.VisualClear();
	}

    public virtual void TakeDamage(int damage)
	{
        ShieldPoints -= damage;
        if (ShieldPoints < 0)
        {
            Hitpoints += ShieldPoints;
            ShieldPoints = 0;
        }

        ThisHealthbar.UpdateHealthbar();
	}

    public virtual int Heal(int healing)
    {
        var correctedHealing = healing;
        Hitpoints += healing;
        if (Hitpoints > MaxHitpoints)
        {
            correctedHealing -= Hitpoints - MaxHitpoints;
            Hitpoints = MaxHitpoints;
        }

        ThisHealthbar.UpdateHealthbar();

        return correctedHealing;
    }

    public virtual void Shield(int shield)
    {
        ShieldPoints += shield;

        ThisHealthbar.UpdateHealthbar();
    }

    public virtual IEnumerator StartTurn()
    {
        yield return null;
        if (OnTurnStart != null)
            OnTurnStart.Invoke();

        boardManager.Clear();
        SetStartOfTurnStats();

        if (statusEffectManager.CantTakeTurn(this))
        {
            EndTurn();
            yield break;
        }
        boardManager.SetMovementAOE(MoveSpeedLeft, currentTile);
    }

    public virtual void EndTurn()
    {
        if (OnTurnEnd != null)
            OnTurnEnd.Invoke();

        damageManager.TakeDotDamage(this);

        StartCoroutine(combatManager.EndTurn());
    }
}
