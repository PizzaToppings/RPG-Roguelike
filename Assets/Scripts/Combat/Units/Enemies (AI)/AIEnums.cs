// AI Role — drives movement profile preferences
public enum AIRoleEnum { Melee, Ranged, Support }

// How a skill selects its best target
public enum TargetPriorityKindEnum { Closest, LowestHealth, HighestHealth, Random, Sequenced }

// Logic operator for condition groups
public enum ConditionLogicEnum { And, Or }

// Which unit(s) a condition evaluates against
public enum ConditionTargetEnum { Self, AnyAlly, AnyEnemy, AnyCharacter }

// Comparison operators used by numeric conditions
public enum AIComparisonEnum { Equal, NotEqual, LessThan, LessThanOrEqual, GreaterThan, GreaterThanOrEqual, Modulo }
