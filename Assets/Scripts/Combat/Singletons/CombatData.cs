using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CombatData : MonoBehaviour
{
    public static int currentRound = 0;

    public static int currentCharacterTurn = 0;

    public static Unit CurrentActiveUnit {get; set; } 

    public static UnityAction onTurnStart;
    public static UnityAction onTurnEnd;
    public static UnityAction onRoundStart;
    public static UnityAction onRoundEnd;
}
