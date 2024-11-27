
public enum DamageTypeEnum {Physical, Arcane, Fire, Water, Earth, Air, Ice, Electric, Psychic, Holy, Dark, Poison, Healing}
public enum StatusEfectEnum {None, Blinded, Silenced, Charmed, Frightened, Incapacitated, Stunned, DoT, Rooted, Invisible, Unstoppable}
public enum currentUnitAction {Nothing, Moving, Casting }
public enum GameState { StartOfGame, StartOfRound, StartOfTurn, EndOfTurn, EndOfRound, EndOfGame}

// Spell Targeting
public enum OriginTileEnum { None, Caster, LastTargetTile, LastTile };
public enum OriginTargetEnum { None, Caster, LastTarget, TargetOnLastTiles };
public enum TargetTileEnum { MouseOverTile, CasterTile, Caster, PreviousDirection, MouseOverTarget };

// Spell FX
public enum SpellFxType { Projectile, AtLocation }
public enum SpellFxOriginEnum { None, Caster, Target, Tiles }
public enum SpellFxDestinationEnum { None, Caster, Target, Tiles }
