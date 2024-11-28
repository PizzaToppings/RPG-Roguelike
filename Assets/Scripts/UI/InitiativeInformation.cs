using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InitiativeInformation : MonoBehaviour
{
    public Image Background;
    public Image Border;
    public TextMeshProUGUI number;

    public Color FriendlyBackground;
    public Color FriendlyNumber;

    public Color EnemyBackground;
    public Color EnemyNumber;

    public int Initiative = 0;

    public void Init(Unit unit, int index)
    {
        if (unit.Friendly)
        {
            Background.color = FriendlyBackground;
            number.color = FriendlyNumber;
        }
        else
        {
            Background.color = EnemyBackground;
            number.color = EnemyNumber;
        }

        number.text = (index + 1).ToString();
        Initiative = unit.Initiative;
    }

    public void ToggleActive(bool active)
    {
        if (active)
            Border.color = Color.white;
        else
            Border.color = Color.black;
    }
}
