// States
public enum CurrentActionKind { None, Basic, CastingSkillshot, Animating };
public enum GameState { StartOfGame, StartOfRound, StartOfTurn, EndOfTurn, EndOfRound, EndOfGame}


// Characters and skills
public enum ClassEnum { Athletics, Acrobatics, Marksmanship, Subtlety, Protection, Sorcery, Elementalism, Nature, Holy, Unholy }
public enum StatsEnum { MaxHitpoints, MaxEnergy, MoveSpeed, PhysicalPower, MagicalPower, PhysicalDefense, MagicalDefense }

// Damage etc
public enum DamageTypeEnum {Physical, Arcane, Fire, Ice, Electric, Psychic, Holy, Dark, Poison, Healing, Shield}
public enum StatusEfectEnum { Hidden, Lifedrain, Blinded, Silenced, Frightened, Incapacitated, Stunned, Poison, Burn, Bleed, Manaburn, Rooted, Taunt, Thorns, Dodge, StatChange }
public enum DisplacementEnum { Teleport, Move, Lift }

// Enemy AI
public enum TargetEnum { Self, closestTarget, LowestHealthTarget }


// Spell Targeting
public enum OriginTileEnum { None, Caster, LastTargetTile, LastTile, GetFromSkillPart };
public enum OriginTargetEnum { None, Caster, LastTarget, TargetOnLastTiles, GetFromSkillPart };
public enum TargetTileEnum { None, MouseOverTile, CasterTile, Caster, PreviousDirection, MouseOverTarget, GetFromSkillPart };
public enum TargetKindEnum { Enemies, Allies, All };
public enum DirectionInputEnum { Mouse, Caster, OriginTile }
public enum CursorType { Normal, Melee, Ranged, Spell, Cross }
public enum AutoTargetEnum { Closest, LowestHealth }


// Skill FX
public enum SkillFxType { Projectile, Animation }
public enum SkillFxTargetEnum { None, Caster, Target, Tiles, SkillObject }


// Triggers
public enum TriggerMomentEnum { StartOfGame, StartOfTurn, EndOfTurn, StartOfRound, EndOfRound, OnDealDamage, OnTakeDamage, OnHeal, OnUseAbility }
public enum TriggerEffectEnum { DealDamage, TakeDamage, AddStatusEffect } 
public enum TileTriggerMomentEnum { OnEnterTile, StartOfTurn, EndOfTurn, Aura }
public enum TileTriggerEffectEnum { DealDamage, ApplyEffect }

// Prerequisites
public enum PrerequisiteUnitEnum { None, Target, Caster }
public enum PrerequisiteConditionEnum { None, StatusEffect, Damage, DamagePercentage }
public enum PrerequisiteOperatorsEnum { Equals, MoreThan, LessThen, MoreThanOrEqual, LessThenOrEqual }