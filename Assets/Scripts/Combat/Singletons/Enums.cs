// States
public enum CurrentActionKind { None, Basic, CastingSkillshot, Animating };
public enum GameState { StartOfGame, StartOfRound, StartOfTurn, EndOfTurn, EndOfRound, EndOfGame}


// Characters and skills
public enum ClassEnum { Athletics, Acrobatics, Marksmanship, Subtlety, Protection, Sorcery, Elementalism, Nature, Holy, Unholy }


// Damage etc
public enum DamageTypeEnum {Physical, Arcane, Fire, Water, Earth, Ice, Electric, Psychic, Holy, Dark, Poison, Healing, Shield}
public enum StatusEfectEnum {None, Blinded, Silenced, Charmed, Frightened, Incapacitated, Stunned, DoT, Rooted, Invisible, Unstoppable }
public enum DisplacementEnum { Teleport, Move }


// Spell Targeting
public enum OriginTileEnum { None, Caster, LastTargetTile, LastTile, GetFromSkillPart };
public enum OriginTargetEnum { None, Caster, LastTarget, TargetOnLastTiles, GetFromSkillPart };
public enum TargetTileEnum { None, MouseOverTile, CasterTile, Caster, PreviousDirection, MouseOverTarget, GetFromSkillPart };
public enum TargetKindEnum { Enemies, Allies, All };
public enum CursorType { Normal, Melee, Ranged, Spell, Cross }


// Skill FX
public enum SkillFxType { Projectile, Animation }
public enum SkillFxOriginEnum { None, Caster, Target, Tiles }
public enum SkillFxDestinationEnum { None, Caster, Target, Tiles }


// Triggers. Need improvement
public enum TriggerMomentEnum { StartOfGame, StartOfTurn, EndOfTurn, StartOfRound, EndOfRound, OnDealDamage, OnTakeDamage, OnHeal }
public enum TriggerEffectEnum { DealDamage, TakeDamage } 
public enum TileTriggerMomentEnum { OnEnterTile, EndOfTurn, Aura }
public enum TileTriggerEffectEnum { DealDamage, TakeDamage }