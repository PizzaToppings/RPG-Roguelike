using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitData
{
    public enum CurrentActionKind {EnemyTurn, Moving, CastingSkillshot};
    
    public static List<Unit> Units = new List<Unit>(); 
    public static List<Character> Characters = new List<Character>(); 
    public static List<Enemy> Enemies = new List<Enemy>(); 
    public static CurrentActionKind CurrentAction;
    public static int? CurrentSkillshot = null;
}
