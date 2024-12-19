// Editor
public enum ComparisonType
{
    Equals = 1,
    NotEqual = 2,
    GreaterThan = 3,
    SmallerThan = 4,
    SmallerOrEqual = 5,
    GreaterOrEqual = 6
}

public enum DisablingType
{
    ReadOnly = 2,
    DontDraw = 3
}

// states
public enum CurrentActionKind { None, Basic, CastingSkillshot, Animating };
public enum GameState { StartOfGame, StartOfRound, StartOfTurn, EndOfTurn, EndOfRound, EndOfGame}

// Damage
public enum DamageTypeEnum {Physical, Arcane, Fire, Water, Earth, Ice, Electric, Psychic, Holy, Dark, Poison, Healing, Shield}
public enum StatusEfectEnum {None, Blinded, Silenced, Charmed, Frightened, Incapacitated, Stunned, DoT, Rooted, Invisible, Unstoppable}

// Spell Targeting
public enum OriginTileEnum { None, Caster, LastTargetTile, LastTile, GetFromSkillPart };
public enum OriginTargetEnum { None, Caster, LastTarget, TargetOnLastTiles, GetFromSkillPart };
public enum TargetTileEnum { None, MouseOverTile, CasterTile, Caster, PreviousDirection, MouseOverTarget, GetFromSkillPart };
public enum CursorType { Normal, Melee, Ranged, Spell }
public enum TargetKindEnum { Enemies, Allies, All };


// Skill FX
public enum SkillFxType { Projectile, Animation }
public enum SkillFxOriginEnum { None, Caster, Target, Tiles }
public enum SkillFxDestinationEnum { None, Caster, Target, Tiles }
