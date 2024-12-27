using System.Collections.Generic;

public class UnitData
{
    public static List<Unit> Units = new List<Unit>(); 
    public static List<Character> Characters = new List<Character>(); 
    public static List<Enemy> Enemies = new List<Enemy>();

    public static Unit ActiveUnit {get; set; } 
    public static CurrentActionKind CurrentAction;
}
