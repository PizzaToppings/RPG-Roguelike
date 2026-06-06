// Run map nodes
public enum NodeTypeEnum { Combat, EliteCombat, Boss, RestZone, Shop, Event, TreasureRoom }
public enum EncounterTierEnum { Normal, Elite, Boss }

// Run progression steps
public enum ProgressionStep { SelectCharacter, SelectSkill, SelectTrait, Combat, EliteCombat, Boss, RestZone, Shop, Event, TreasureRoom }

// States
public enum CurrentActionKind { None, Basic, CastingSkillshot, Animating, EnemyTurn, CharacterPlacement };
public enum GameState { StartOfGame, StartOfRound, StartOfTurn, EndOfTurn, EndOfRound, EndOfGame}


// Characters and skills
public enum ClassEnum { Athletics, Acrobatics, Marksmanship, Subtlety, Protection, Sorcery, Elementalism, Nature, Holy, Unholy }
public enum StatsEnum { MaxHitpoints, /*MaxEnergy,*/ MoveSpeed, Power, Armor, Shield, Range }

// Damage etc
public enum HitTypeEnum { Damage, Healing, Shield }
public enum StatusEffectEnum { Hidden, Lifedrain, Blinded, Silenced, Frightened, Incapacitated, Stunned, Poison, Burn, Bleed, Fatique, Rooted, Taunt, Thorns, Dodge, StatChange, Unique }
public enum DisplacementEnum { Teleport, Move, Lift }

// Enemy AI
public enum TargetEnum { Self, closestTarget, LowestHealthTarget, AllAllies, AllEnemies, AllUnits }
public enum TraitTargetFactionEnum { Friendly, Enemy, All }
public enum TraitTargetSelectionEnum { Self, Closest, LowestHealth, Area }

// Enemy Intent (displayed above healthbar)
public enum IntentActionEnum { Unknown, MeleeAttack, RangedAttack, Debuff, Buff, Heal, AOE }
public enum IntentTargetEnum { Unknown, Nearest, LowestHealth, Area, Self, Random }


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
public enum TriggerMomentEnum { Instant, StartOfCombat, StartOfTurn, EndOfTurn, StartOfRound, EndOfRound, OnDealDamage, OnTakeDamage, OnHeal, OnUseAbility, OnStanceChange, EndOfCombat, OnKillEnemy }
public enum TriggerEffectEnum { DealDamage, AddStatusEffect, AddEnergy, ModifyStat }
// Which unit's turn should be used for duration ticks
public enum DurationOwnerEnum { Target, Caster }

// Skill Augments
public enum SkillAugmentTriggerEnum { OnInit, OnCast, OnCastPerTarget, OnStanceChange }
public enum SkillAugmentEffectEnum { AddStatusEffectToTargets, AddStatusEffectToCaster, AddEnergy, ModifyEnergyCost, ModifyRange, ModifyDamage, ResetSkill }

// Anomalies
public enum AnomalyTriggerEnum { OnCombatStart, StartOfRound, EndOfRound, OnStanceChange, OnCombatEnd }
public enum AnomalyTargetEnum { AllCharacters, AllEnemies, AllUnits, BoardState }
public enum AnomalyEffectEnum { AddStatusEffect, ModifyStat, DealDamage }
public enum EnemyTriggerMomentEnum { StartOfCombat, StartOfTurn, EndOfTurn, StartOfRound, EndOfRound, OnDealDamage, OnTakeDamage, OnDeath }
public enum TileTriggerMomentEnum { OnEnterTile, StartOfTurn, EndOfTurn, Aura }
public enum TileTriggerEffectEnum { DealDamage, ApplyEffect }

// Prerequisites
public enum PrerequisiteUnitEnum { None, Target, Caster }
public enum PrerequisiteConditionEnum { None, StatusEffect, Damage, DamagePercentage, AdjacentUnits, CombatStyle }
public enum PrerequisiteOperatorsEnum { Equals, MoreThan, LessThen, MoreThanOrEqual, LessThenOrEqual, NotEquals }
public enum PrerequisiteAdjacentFactionEnum { Any, Friendly, Enemy }