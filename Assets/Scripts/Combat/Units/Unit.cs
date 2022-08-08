using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Unit : UnitStats
{
    public bool MouseOver;
    public UnityEvent OnTurnStart;
    public UnityEvent OnTurnEnd;

    public UnityEvent<Unit> OnClick;

    public UnityEvent OnDealDamage;
    public UnityEvent OnTakeDamage;

    public virtual void Init()
    {
        combatManager = CombatManager.combatManager;
        boardManager = BoardManager.boardManager;
        unitManager = UnitManager.unitManager;

        SetStats();
        RollInitiative();
    }

    public virtual void Update()
    {
        
    }

    void OnMouseEnter()
    {
        currentTile.Target();
    }

    void OnMouseDown()
    {
        if (OnClick != null)
            OnClick.Invoke(this);
    }

    void OnMouseExit()
    {
       currentTile.UnTarget();
    }

    void SetStats()
    {
        // where do I get stats?
        UnitName = nameGenerator();
        gameObject.name = UnitName;
        MoveSpeed = Random.Range(5, 15);
        PhysicalPower = Random.Range(0, 5);
        MagicalPower = Random.Range(0, 5);

        if (skillshots?.Count == 0)
            return;

        SetSkillShots();
    }

    void SetSkillShots()
    {
        skillshots.ForEach(x => x.SkillshotParts.ForEach(y => y.Caster = this));

        for (int i = 0; i < MaxSkillShotAmount; i++)
        {
            if (i >= skillshots.Count)
            {
                SkillshotsEquipped.Add(false);
                continue;
            }

            if (skillshots[i] != null)
                SkillshotsEquipped.Add(true);
            else
                SkillshotsEquipped.Add(true);
        }
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
        Initiative = Random.Range(0, 10);
    }

    public virtual void StartMoving(List<BoardTile> path)
    {
        StartCoroutine(Move(path));
    } 

    IEnumerator Move(List<BoardTile> path)
    {
        Vector3 startPosition = transform.position + Vector3.up;
        Vector3 endPosition = new Vector3();

        for (int i = 0; i < path.Count; i++)
        {
            endPosition = path[i].transform.position + Vector3.up;
            float distanceLeft = 0;

            while (distanceLeft <= 1)
            {
                distanceLeft += Time.deltaTime * (1f + MoveSpeed*0.1f);
                transform.position = Vector3.Lerp(startPosition, endPosition, distanceLeft);

                yield return new WaitForEndOfFrame();
            }

            path[i].currentCharacter = null;
            UnitData.CurrentActiveUnit.currentTile = path[i];
            path[i].currentCharacter = UnitData.CurrentActiveUnit;
            MoveSpeedLeft--;
            startPosition = endPosition;
        }
        var endTile = path[path.Count-1];
        boardManager.Clear();
        boardManager.SetAOE(MoveSpeedLeft, endTile, null);
    }

    void SetStartOfTurnStats()
    {
        MoveSpeedLeft = MoveSpeed;
    }

    public virtual void PreviewSkills(BoardTile mouseOverTile) 
    {
        boardManager.Clear();
    }

    public void CastSkill()
    {
        for (int i = 0; i < SkillshotsEquipped.Count; i++)
        {
            if (SkillshotData.CurrentSkillshotIndex == i + 1 && SkillshotsEquipped[i])
            {
                skillshots[i].Cast();
            }
        }
    }

    public DamageData DealDamage(SO_Skillshot skillshot, Unit target)
    {
        var roll = Random.Range(1, skillshot.Damage);
        var addition = skillshot.MagicalDamage ? MagicalPower : PhysicalPower;
        var damage = roll + addition;

        DamageData data = new DamageData();
        data.DamageType = skillshot.DamageType;
        data.Caster = this;
        data.Target = target;
        data.Damage = damage;

        if (OnDealDamage != null)
            OnDealDamage.Invoke();

        return data;
    }

    public void TakeDamage(DamageData data)
    {
        if (Resistances.Contains(data.DamageType))
        {
            data.Damage = data.Damage / 2;
        }
    }

    public virtual void StartTurn()
    {
        if (OnTurnStart != null)
            OnTurnStart.Invoke();

        boardManager.Clear();
        SetStartOfTurnStats();
        boardManager.SetAOE(MoveSpeed, currentTile, null);
    }

    public virtual void EndTurn()
    {
        if (OnTurnEnd != null)
            OnTurnEnd.Invoke();

        StartCoroutine(combatManager.EndTurn());
    }
}
