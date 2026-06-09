using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class Unit : UnitStats
{
    [HideInInspector] public CombatManager combatManager;
    [HideInInspector] public BoardManager boardManager;
    [HideInInspector] public UnitManager unitManager;
    [HideInInspector] public SkillsManager skillsManager;
    [HideInInspector] public SkillVFXManager skillVFXManager;
    [HideInInspector] public ConsumableManager consumableManager;
    [HideInInspector] public StatusEffectManager statusEffectManager;
    [HideInInspector] public DamageManager damageManager;
    [HideInInspector] public UIManager uiManager;
    [HideInInspector] public UI_Singletons ui_Singletons;
    [HideInInspector] public DisplacementPreviewManager displacementPreviewManager;
    [HideInInspector] public InitiativeInformation initiativeInformation;
    UnitHighlighter highlighter;


    [HideInInspector] public UnityEvent OnUnitTurnStartEvent = new UnityEvent();
    [HideInInspector] public UnityEvent OnUnitTurnEndEvent = new UnityEvent();
    [HideInInspector] public UnityEvent OnDeathEvent = new UnityEvent();
    [HideInInspector] public UnityEvent<DamagaDataResolved> OnUnitTakeDamageEvent = new UnityEvent<DamagaDataResolved>();
    [HideInInspector] public UnityEvent<Unit> OnKillEnemyEvent = new UnityEvent<Unit>();

    [HideInInspector] public UnityEvent<DamageDataCalculated> OnDealDamage;
    [HideInInspector] public UnityEvent<DamageDataCalculated> OnTakeDamage;
    [HideInInspector] public UnityEvent<CombatStyle, CombatStyle> OnStanceChangeEvent = new UnityEvent<CombatStyle, CombatStyle>();
    [HideInInspector] public Animator modelAnimator;
    [HideInInspector] public SpriteRenderer modelSprite;

    [HideInInspector] public float OutgoingDamageMultiplier = 1f;

    [HideInInspector] public Healthbar ThisHealthbar;

    [HideInInspector] public Vector3 position => transform.position;

    // for testing and debugging
    public int startXPosition = 0;
    public int startYPosition = 0;

    public virtual void Init()
    {
        combatManager = CombatManager.Instance;
        boardManager = BoardManager.Instance;
        unitManager = UnitManager.Instance;
        skillsManager = SkillsManager.Instance;
        skillVFXManager = SkillVFXManager.Instance;
        consumableManager = ConsumableManager.Instance;
        statusEffectManager = StatusEffectManager.Instance;
        damageManager = DamageManager.Instance;
        uiManager = UIManager.Instance;
        ui_Singletons = UI_Singletons.Instance;
        displacementPreviewManager = DisplacementPreviewManager.Instance;

        modelSprite   = GetComponentInChildren<SpriteRenderer>();
        modelAnimator = GetComponentInChildren<Animator>();

        highlighter = GetComponentInChildren<UnitHighlighter>();

        SetStats();
    }

    public virtual void Update()
    {
    }

    public virtual void MouseEnter()
    {
        if (BoardData.CurrentMouseTile != null)
            BoardData.CurrentMouseTile.UnTarget();

        Tile.Target();
    }

    public virtual void MouseDown()
    {
        Tile.OnClick();
    }

    public virtual void MouseExit()
    {
        Tile.UnTarget();
    }

    // MouseDown will call Onclick through the tile onclick
    public virtual void OnClick()
	{
	}

    // MouseEnter will call Target through the tile MouseEnter
    public virtual void Target()
    {
        IsTargeted = true;
        highlighter.SetHighlight(true);
        initiativeInformation.ToggleHover(true);
    }

    public virtual void Untarget()
    {
        IsTargeted = false;
        highlighter.SetHighlight(false);
        initiativeInformation.ToggleHover(false);
    }

    public virtual void SetStats()
    {
    }

    /// <summary>
    /// Call whenever CurrentCombatStyle changes to refresh the tile highlight and initiative UI.
    /// </summary>
    public void RefreshCombatStyleVisuals()
    {
        initiativeInformation?.RefreshStyleColor();
    }

    public IEnumerator Move(List<BoardTile> path)
    {
        if (modelAnimator != null) modelAnimator.SetBool("Run", true);

        Vector3 startPosition = transform.position; 
        Vector3 endPosition;

        Tile.OnExitTile();
        Tile.OverrideColor(boardManager.originalColor); // clear style tint while unit is in transit

        BoardTile previousTile = Tile;

        for (int i = 0; i < path.Count; i++)
        {
            endPosition = path[i].transform.position;
            float distanceLeft = 0;

            Rotate(endPosition);

            while (distanceLeft < 1f)
            {
                // move to next tile
                distanceLeft = Mathf.Min(distanceLeft + Time.deltaTime * (1f + MoveSpeed * 0.4f), 1f);
                transform.position = Vector3.Lerp(startPosition, endPosition, distanceLeft);

                yield return new WaitForEndOfFrame();
            }

            path[i].OnEnterTile(this);

            MoveSpeedLeft -= boardManager.GetRangeReduction(previousTile, path[i]);
            previousTile = path[i];
            startPosition = endPosition;

            if (i != path.Count - 1)
                path[i].OnExitTile();
        }
        var endTile = path[path.Count-1];

        if (Friendly)
            UnitData.CurrentAction = CurrentActionKind.Basic;

        boardManager.Clear();
        boardManager.SetAOE(MoveSpeedLeft, endTile, null);
        if (modelAnimator != null) modelAnimator.SetBool("Run", false);
    }

    protected void Rotate(Vector3 endPosition)
    {
        if (modelSprite == null) return;

        // Convert both positions to screen space and flip based on screen-left/right.
        // This works correctly on isometric tilemaps where world-X alone is unreliable.
        Vector3 screenCurrent = Camera.main.WorldToScreenPoint(transform.position);
        Vector3 screenTarget  = Camera.main.WorldToScreenPoint(endPosition);

        float screenDeltaX = screenTarget.x - screenCurrent.x;
        if (screenDeltaX < -0.5f)
            modelSprite.flipX = true;
        else if (screenDeltaX > 0.5f)
            modelSprite.flipX = false;
        // If delta is near zero (moving straight up/down in screen space), keep current facing.
    }

    /// <summary>
    /// Briefly dashes the unit toward <paramref name="targetWorldPos"/> and snaps back.
    /// Used as a temporary stand-in for skill VFX in 2D.
    /// </summary>
    public IEnumerator DashTowards(Vector3 targetWorldPos, float distance = 0.35f, float duration = 0.25f)
    {
        Vector3 origin = transform.position;
        Vector3 dir = (targetWorldPos - origin).normalized;

        // Fallback: use current facing direction when there is no valid target position
        if (dir == Vector3.zero)
            dir = modelSprite != null && modelSprite.flipX ? Vector3.left : Vector3.right;

        Vector3 dashPeak = origin + dir * distance;

        // Dash forward (40 % of duration)
        float t = 0f;
        while (t < 1f)
        {
            t = Mathf.Min(t + Time.deltaTime / (duration * 0.4f), 1f);
            transform.position = Vector3.Lerp(origin, dashPeak, t);
            yield return null;
        }

        // Snap back (60 % of duration)
        t = 0f;
        while (t < 1f)
        {
            t = Mathf.Min(t + Time.deltaTime / (duration * 0.6f), 1f);
            transform.position = Vector3.Lerp(dashPeak, origin, t);
            yield return null;
        }

        transform.position = origin;
    }

    /// <summary>
    /// Rapidly toggles the sprite's visibility to indicate a hit.
    /// </summary>
    IEnumerator BlinkOnHit(float duration = 0.4f, float interval = 0.07f)
    {
        if (modelSprite == null) yield break;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            modelSprite.enabled = !modelSprite.enabled;
            yield return new WaitForSeconds(interval);
            elapsed += interval;
        }

        modelSprite.enabled = true;
    }

    public virtual void SetStartOfTurnStats()
    {
        MoveSpeedLeft = MoveSpeed;

        if (statusEffectManager.UnitHasStatusEffect(this, StatusEffectEnum.Rooted))
            MoveSpeedLeft = 0;
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
            HitType = damageDataCalculated.HitType,
            DamageDone = damage,
            ShieldDamage = shieldDamage,
            AttackRange = boardManager.GetRangeBetweenTiles(damageDataCalculated.Caster.Tile, Tile)
        };

        if (OnUnitTakeDamageEvent != null)
            OnUnitTakeDamageEvent.Invoke(damagaDataResolved);

        OnTakeDamage?.Invoke(damageDataCalculated);

        ThisHealthbar.UpdateHealthbar();

        if (damage > 0 || shieldDamage > 0)
            StartCoroutine(BlinkOnHit());

        if (Hitpoints <= 0)
            Die();

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
            HitType = damageDataCalculated.HitType,
            DamageDone = correctedHealing,
            ShieldDamage = 0,
            AttackRange = boardManager.GetRangeBetweenTiles(damageDataCalculated.Caster.Tile, Tile)
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
            HitType = damageDataCalculated.HitType,
            DamageDone = 0,
            ShieldDamage = -damageDataCalculated.Damage,
            AttackRange = boardManager.GetRangeBetweenTiles(damageDataCalculated.Caster.Tile, Tile)
        };

        ThisHealthbar.UpdateHealthbar();

        return damagaDataResolved;
    }

    public virtual List<SO_SKillVFX> GetSkillVFXList()
    {
        return new List<SO_SKillVFX>();
    }

    public virtual IEnumerator StartTurn()
    {
        var turnStartText = $"Turn: {UnitData.ActiveUnit.UnitName}";

        var isStunned = statusEffectManager.UnitHasStatusEffect(this, StatusEffectEnum.Stunned);
        if (isStunned)
            turnStartText += " - stunned";

        var isIncapactated = statusEffectManager.UnitHasStatusEffect(this, StatusEffectEnum.Incapacitated);
        if (isIncapactated)
            turnStartText += " - incapacitated";

        boardManager.Clear();
        SetStartOfTurnStats();

        yield return new WaitForSeconds(0.5f);

        if (OnUnitTurnStartEvent != null)
            OnUnitTurnStartEvent.Invoke();

        if (isStunned || isIncapactated)
        {
            EndTurn();
            yield break;
        }

        if (statusEffectManager.UnitHasStatusEffect(this, StatusEffectEnum.Rooted) == false)
            boardManager.SetAOE(MoveSpeedLeft, Tile, null);
    }

    public virtual void EndTurn()
    {
        if (!gameObject.activeInHierarchy) return;

        if (OnUnitTurnEndEvent != null)
            OnUnitTurnEndEvent.Invoke();

        // Unit may have died during turn-end effects (e.g. Bleed), in which case
        // RemoveUnit already handles the turn transition.
        if (!gameObject.activeInHierarchy) return;

        // Apply the pending stance selected this turn at end of turn.
        if (PendingCombatStyle != CombatStyle.None)
        {
            var oldStyle = CurrentCombatStyle;
            CurrentCombatStyle = PendingCombatStyle;
            PendingCombatStyle = CombatStyle.None;
            RefreshCombatStyleVisuals();

            // Notify listeners about stance change (old, new)
            try { OnStanceChangeEvent?.Invoke(oldStyle, CurrentCombatStyle); } catch {}
 
            CombatStyleUtility.ApplyStanceEffects(CurrentCombatStyle, this);
        }

        StartCoroutine(combatManager.EndTurn());
    }

    public virtual void Die()
	{
        // Ensure any targeting visuals / panels are cleared when a unit dies.
        // Calling Tile.UnTarget will route through the tile to the unit's Untarget
        // implementation (which hides enemy panels for enemies), but also call
        // Untarget() directly as a fallback if Tile is null.
        try
        {
            if (Tile != null)
                Tile.UnTarget();
            else
                Untarget();
        }
        catch { }

        ThisHealthbar.RemoveHealthbar();
        StartCoroutine(unitManager.RemoveUnit(this));

        // Refresh enemy order numbers if an enemy died
        if (this is Enemy)
            HealthCanvas.Instance?.RefreshEnemyOrderNumbers();
    }
}
