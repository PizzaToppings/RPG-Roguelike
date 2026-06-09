using UnityEngine.Events;

using System.Collections.Generic;

public class CombatData
{
    public static bool IsReady = false;

    public static int currentRound = 0;

    // Index into the current round's turn sequence.
    public static int CurrentUnitTurn = 0;

    // Sequence of units for the current round. May contain the same Unit multiple times
    // to represent multiple turn slots for a single unit.
    public static List<Unit> TurnSequence = new List<Unit>();

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
        TurnSequence.Clear();
        onTurnStart.RemoveAllListeners();
        onTurnEnd.RemoveAllListeners();
        onRoundStart.RemoveAllListeners();
        onRoundEnd.RemoveAllListeners();
        onCombatEnd.RemoveAllListeners();
    }
}
