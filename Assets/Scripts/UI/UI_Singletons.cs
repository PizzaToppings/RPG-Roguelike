using UnityEngine;

public class UI_Singletons : MonoBehaviour
{
    public static UI_Singletons Instance;

    CursorMode cursorMode = CursorMode.ForceSoftware;

    [Space]
    [SerializeField] Texture2D DefaultCursorTexture;
    [SerializeField] Texture2D MeleeAttackCursorTexture;
    [SerializeField] Texture2D RangedAttackCursorTexture;
    [SerializeField] Texture2D SpellCursorTexture;
    [SerializeField] Texture2D CrossCursorTexture;

    [Space]
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
    [SerializeField] Color PhysicalDamageColor;
    [SerializeField] Color ArcaneDamageColor;
    [SerializeField] Color FireDamageColor;
    [SerializeField] Color WaterDamageColor;
    [SerializeField] Color EarthDamageColor;
    [SerializeField] Color IceDamageColor;
    [SerializeField] Color ElectricDamageColor;
    [SerializeField] Color PsychicDamageColor;
    [SerializeField] Color HolyDamageColor;
    [SerializeField] Color DarkDamageColor;
    [SerializeField] Color PoisonDamageColor;
    [SerializeField] Color HealingColor;
    [SerializeField] Color ShieldColor;



    public void CreateInstance()
    {
        Instance = this;

        Cursor.SetCursor(DefaultCursorTexture, Vector2.zero, cursorMode);
    }

    public Color GetDamageTypeColor(DamageTypeEnum damageType)
    {
        switch (damageType)
        {
            case DamageTypeEnum.Arcane:
                return ArcaneDamageColor;
            case DamageTypeEnum.Fire:
                return FireDamageColor;
            case DamageTypeEnum.Water:
                return WaterDamageColor;
            case DamageTypeEnum.Earth:
                return EarthDamageColor;
            case DamageTypeEnum.Ice:
                return IceDamageColor;
            case DamageTypeEnum.Electric:
                return ElectricDamageColor;
            case DamageTypeEnum.Holy:
                return HolyDamageColor;
            case DamageTypeEnum.Dark:
                return DarkDamageColor;
            case DamageTypeEnum.Poison:
                return PoisonDamageColor;
            case DamageTypeEnum.Healing:
                return HealingColor;
            case DamageTypeEnum.Shield:
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

        Cursor.SetCursor(texture, Vector2.zero, cursorMode);
    }
}
