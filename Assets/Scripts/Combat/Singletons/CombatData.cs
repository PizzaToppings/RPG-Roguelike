using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CombatData : MonoBehaviour
{
    public static int currentRound = 0;

    public static int currentCharacterTurn = 0;

    public static UnityEvent onTurnStart = new UnityEvent();
    public static UnityEvent onTurnEnd = new UnityEvent();
    public static UnityEvent onRoundStart = new UnityEvent();
    public static UnityEvent onRoundEnd = new UnityEvent();
}
