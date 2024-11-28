
public enum DamageTypeEnum {Physical, Arcane, Fire, Water, Earth, Ice, Electric, Psychic, Holy, Dark, Poison, Healing}
public enum StatusEfectEnum {None, Blinded, Silenced, Charmed, Frightened, Incapacitated, Stunned, DoT, Rooted, Invisible, Unstoppable}
public enum currentUnitAction {Nothing, Moving, Casting }
public enum GameState { StartOfGame, StartOfRound, StartOfTurn, EndOfTurn, EndOfRound, EndOfGame}

// Spell Targeting
public enum OriginTileEnum { None, Caster, LastTargetTile, LastTile };
public enum OriginTargetEnum { None, Caster, LastTarget, TargetOnLastTiles };
public enum TargetTileEnum { MouseOverTile, CasterTile, Caster, PreviousDirection, MouseOverTarget };

// Spell FX
public enum SkillFxType { Projectile, Animation }
public enum SkillFxOriginEnum { None, Caster, Target, Tiles }
public enum SkillFxDestinationEnum { None, Caster, Target, Tiles }
