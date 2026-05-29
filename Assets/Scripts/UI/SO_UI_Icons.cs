using UnityEngine;

[CreateAssetMenu(fileName = "UI_ICons", menuName = "ScriptableObjects/UI/Icons")]
public class SO_UI_Icons : ScriptableObject
{
    CursorMode cursorMode = CursorMode.ForceSoftware;

    [Header(" - Cursors")]
    [SerializeField] Texture2D DefaultCursorTexture;
    [SerializeField] Texture2D MeleeAttackCursorTexture;
    [SerializeField] Texture2D RangedAttackCursorTexture;
    [SerializeField] Texture2D SpellCursorTexture;
    [SerializeField] Texture2D CrossCursorTexture;

    [Space]
    [Header(" - Class Icons")]
    [SerializeField] Sprite AthleticsIcon;
    [SerializeField] Sprite AcrobaticsIcon;
    [SerializeField] Sprite MarksmanshipIcon;
    [SerializeField] Sprite SubtletyIcon;
    [SerializeField] Sprite ProtectionIcon;
    [SerializeField] Sprite SorceryIcon;
    [SerializeField] Sprite ElementalismIcon;
    [SerializeField] Sprite NatureIcon;
    [SerializeField] Sprite HolyIcon;
    [SerializeField] Sprite UnholyIcon;

    [Space]
    [Header(" - Damage Colors")]
    [SerializeField] Color PhysicalDamageColor;
    [SerializeField] Color HealingColor;
    [SerializeField] Color ShieldColor;

    [Space]
    [Header(" - Intent Action Icons")]
    [SerializeField] Sprite IntentUnknownIcon;
    [SerializeField] Sprite IntentMeleePhysicalIcon;
    [SerializeField] Sprite IntentRangedPhysicalIcon;
    [SerializeField] Sprite IntentMeleeMagicalIcon;
    [SerializeField] Sprite IntentRangedMagicalIcon;
    [SerializeField] Sprite IntentDebuffIcon;
    [SerializeField] Sprite IntentBuffIcon;
    [SerializeField] Sprite IntentHealIcon;
    [SerializeField] Sprite IntentShieldIcon;
    [SerializeField] Sprite IntentWaitIcon;

    [Space]
    [Header(" - Intent Target Icons")]
    [SerializeField] Sprite IntentTargetUnknownIcon;
    [SerializeField] Sprite IntentTargetNearestIcon;
    [SerializeField] Sprite IntentTargetLowestHealthIcon;
    [SerializeField] Sprite IntentTargetFullestHealthIcon;
    [SerializeField] Sprite IntentTargetAreaIcon;
    [SerializeField] Sprite IntentTargetSelfIcon;
    [SerializeField] Sprite IntentTargetRandomIcon;
    [SerializeField] Sprite IntentAOEIcon;

    [Space]
    [Header(" - Status Icons")]
    [SerializeField] Sprite BleedIcon;
    [SerializeField] Sprite PoisonIcon;
    [SerializeField] Sprite BurnedIcon;
    [SerializeField] Sprite HiddenIcon;
    [SerializeField] Sprite BlindedIcon;
    [SerializeField] Sprite SilencedIcon;
    [SerializeField] Sprite FrightenedIcon;
    [SerializeField] Sprite IncapacitatedIcon;
    [SerializeField] Sprite StunnedIcon;
    [SerializeField] Sprite RootedIcon;
    [SerializeField] Sprite TauntIcon;
    [SerializeField] Sprite ThornsIcon;

    public void InitDefaultCursor()
    {
        //Cursor.SetCursor(DefaultCursorTexture, Vector2.zero, cursorMode);
    }

    public void SetCursor(CursorType cursorType)
    {
        Texture2D texture = DefaultCursorTexture;

        switch (cursorType)
        {
            case CursorType.Melee:
                texture = MeleeAttackCursorTexture;
                break;
            case CursorType.Ranged:
                texture = RangedAttackCursorTexture;
                break;
            case CursorType.Spell:
                texture = SpellCursorTexture;
                break;
            case CursorType.Cross:
                texture = CrossCursorTexture;
                break;
        }

        //Cursor.SetCursor(texture, Vector2.zero, cursorMode);
    }

    public Color GetHitTypeColor(HitTypeEnum hitType)
    {
        switch (hitType)
        {
            case HitTypeEnum.Healing:
                return HealingColor;
            case HitTypeEnum.Shield:
                return ShieldColor;
        }
        return PhysicalDamageColor;
    }

    public Sprite GetClassIcon(ClassEnum thisClass)
    {
        switch (thisClass)
        {
            case ClassEnum.Athletics:
                return AthleticsIcon;
            case ClassEnum.Acrobatics:
                return AcrobaticsIcon;
            case ClassEnum.Marksmanship:
                return MarksmanshipIcon;
            case ClassEnum.Subtlety:
                return SubtletyIcon;
            case ClassEnum.Protection:
                return ProtectionIcon;
            case ClassEnum.Sorcery:
                return SorceryIcon;
            case ClassEnum.Elementalism:
                return ElementalismIcon;
            case ClassEnum.Nature:
                return NatureIcon;
            case ClassEnum.Holy:
                return HolyIcon;
        }

        return UnholyIcon;
    }

    public Sprite GetStatusIcon(StatusEffectEnum statusEffect)
    {
        switch (statusEffect)
        {
            case StatusEffectEnum.Bleed:
                return BleedIcon;
            case StatusEffectEnum.Poison:
                return PoisonIcon;
            case StatusEffectEnum.Burn:
                return BurnedIcon;
            case StatusEffectEnum.Hidden:
                return HiddenIcon;
            case StatusEffectEnum.Blinded:
                return BlindedIcon;
            case StatusEffectEnum.Silenced:
                return SilencedIcon;
            case StatusEffectEnum.Frightened:
                return FrightenedIcon;
            case StatusEffectEnum.Incapacitated:
                return IncapacitatedIcon;
            case StatusEffectEnum.Stunned:
                return StunnedIcon;
            case StatusEffectEnum.Rooted:
                return RootedIcon;
            case StatusEffectEnum.Taunt:
                return TauntIcon;
            case StatusEffectEnum.Thorns:
                return ThornsIcon;
        }
        return null;
    }

    public Sprite GetIntentActionIcon(IntentActionEnum action)
    {
        switch (action)
        {
            case IntentActionEnum.PhysicalMeleeAttack:
                return IntentMeleePhysicalIcon;
            case IntentActionEnum.PhysicalRangedAttack:
                return IntentRangedPhysicalIcon;
            case IntentActionEnum.MagicalMeleeAttack:
                return IntentMeleeMagicalIcon;
            case IntentActionEnum.MagicalRangedAttack:
                return IntentRangedMagicalIcon;
            case IntentActionEnum.Debuff:
                return IntentDebuffIcon;
            case IntentActionEnum.Buff:
                return IntentBuffIcon;
            case IntentActionEnum.Heal:
                return IntentHealIcon;
            case IntentActionEnum.AOE:
                return IntentAOEIcon;
        }
        return IntentUnknownIcon;
    }

    public Sprite GetIntentTargetIcon(IntentTargetEnum target)
    {
        switch (target)
        {
            case IntentTargetEnum.Nearest:
                return IntentTargetNearestIcon;
            case IntentTargetEnum.LowestHealth:
                return IntentTargetLowestHealthIcon;
            case IntentTargetEnum.Area:
                return IntentTargetAreaIcon;
            case IntentTargetEnum.Self:
                return IntentTargetSelfIcon;
            case IntentTargetEnum.Random:
                return IntentTargetRandomIcon;
        }
        return IntentTargetUnknownIcon;
    }
}
