using UnityEngine.Events;

public class CombatData
{
    public static bool IsReady = false;

    public static int currentRound = 0;

    public static int CurrentUnitTurn = 0;

    public static UnityEvent onTurnStart = new UnityEvent();
    public static UnityEvent onTurnEnd = new UnityEvent();
    public static UnityEvent onRoundStart = new UnityEvent();
    public static UnityEvent onRoundEnd = new UnityEvent();
    public static UnityEvent onCombatEnd = new UnityEvent();

    public static void Reset()
    {
        IsReady = false;
        currentRound = 0;
        CurrentUnitTurn = 0;
        onTurnStart.RemoveAllListeners();
        onTurnEnd.RemoveAllListeners();
        onRoundStart.RemoveAllListeners();
        onRoundEnd.RemoveAllListeners();
        onCombatEnd.RemoveAllListeners();
    }
}
