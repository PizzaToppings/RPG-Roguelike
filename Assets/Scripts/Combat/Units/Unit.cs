﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Unit : UnitStats
{
    [HideInInspector] public CombatManager combatManager;
    [HideInInspector] public BoardManager boardManager;
    [HideInInspector] public UnitManager unitManager;
    [HideInInspector] public SkillsManager skillsManager;
    [HideInInspector] public SkillFXManager skillFXManager;
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
        skillFXManager = SkillFXManager.Instance;
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
        UnitName = nameGenerator();
        gameObject.name = UnitName;
        MoveSpeed = Random.Range(5, 15);
    }

    /// temp
    string nameGenerator()
    {
        string name = "";
        string n = "bleh";
        string n1 = "boi";
        string n2 = "si";
        string n3 = "ma";
        string n4 = "nay";
        string n5 = "art";
        string n6 = "sli";
        string n7 = "buzz";
        string n8 = "wick";
        string n9 = "tom";
        string n0 = "Jebus";
        string n10 = "Iamic";
        string n11 = "Ian";
        string n12 = "Acid";
        string n13 = "Hobag";
        string n14 = "Swag";
        string n15 = "gonad";
        string n16 = "doc";
        string n17 = "aouth";
        string n18 = "crook";
        string n19 = "twi";
        string n20 = "lite";
        string n21 = "amai";
        string n22 = "brood";
        string n23 = "justin";
        string n24 = "daniel";
        string n25 = "bart";
        string n26 = "kaas";
        string n27 = "eli";
        string n28 = "oliver";
        string n29 = "the master";

        string[] parts = new string[31];
        parts[0] = n;
        parts[1] = n1;
        parts[2] = n2;
        parts[3] = n3;
        parts[4] = n4;
        parts[5] = n5;
        parts[6] = n6;
        parts[7] = n7;
        parts[8] = n8;
        parts[9] = n9;
        parts[10] = n10;
        parts[11] = n11;
        parts[12] = n12;
        parts[13] = n13;
        parts[14] = n14;
        parts[15] = n15;
        parts[16] = n16;
        parts[17] = n17;
        parts[18] = n18;
        parts[19] = n19;
        parts[20] = n20;
        parts[21] = n21;
        parts[22] = n22;
        parts[23] = n23;
        parts[24] = n24;
        parts[25] = n25;
        parts[26] = n26;
        parts[27] = n27;
        parts[28] = n28;
        parts[29] = n29;
        parts[30] = n0;

        int randomPartAmountA = Random.Range(2, 6);
        int randomPartAmount = Random.Range(2, randomPartAmountA);

        for (int i = 0; i < randomPartAmount; i++)
        {
            int randomPart = Random.Range(0, parts.Length);
            name += parts[randomPart];

            int randomSpace = Random.Range(0, 10);
            if (randomSpace == 0)
                name += " "; 
        }

        if (!Friendly)
            name += " (Enemy)";

        return name;
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
