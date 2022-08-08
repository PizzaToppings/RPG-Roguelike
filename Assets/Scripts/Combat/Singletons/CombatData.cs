using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CombatData : MonoBehaviour
{
    public static int currentRound = 0;

    public static int currentCharacterTurn = 0;

    public static UnityEvent onTurnStart;
    public static UnityEvent onTurnEnd;
    public static UnityEvent onRoundStart;
    public static UnityEvent onRoundEnd;
}
