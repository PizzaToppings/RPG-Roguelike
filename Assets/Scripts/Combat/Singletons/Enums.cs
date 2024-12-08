// states
public enum CurrentActionKind { None, Basic, CastingSkillshot, Animating };
public enum GameState { StartOfGame, StartOfRound, StartOfTurn, EndOfTurn, EndOfRound, EndOfGame}

// Damage
public enum DamageTypeEnum {Physical, Arcane, Fire, Water, Earth, Ice, Electric, Psychic, Holy, Dark, Poison, Healing}
public enum StatusEfectEnum {None, Blinded, Silenced, Charmed, Frightened, Incapacitated, Stunned, DoT, Rooted, Invisible, Unstoppable}

// Spell Targeting
public enum OriginTileEnum { None, Caster, LastTargetTile, LastTile };
public enum OriginTargetEnum { None, Caster, LastTarget, TargetOnLastTiles };
public enum TargetTileEnum { None, MouseOverTile, CasterTile, Caster, PreviousDirection, MouseOverTarget };
public enum CursorType { Normal, Melee, Ranged, Spell }

// Skill FX
public enum SkillFxType { Projectile, Animation }
public enum SkillFxOriginEnum { None, Caster, Target, Tiles }
public enum SkillFxDestinationEnum { None, Caster, Target, Tiles }
