using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InitiativeInformation : MonoBehaviour
{
    public Image Background;
    public Image Border;
    public Image portrait;
    public TextMeshProUGUI number;

    public Color FriendlyBackground;
    public Color FriendlyNumber;

    public Color EnemyBackground;
    public Color EnemyNumber;

    public Unit thisUnit;

    public int Initiative = 0;

    public void Init(Unit unit, int index)
    {
        thisUnit = unit;
        if (unit.Friendly)
        {
            Background.color = FriendlyBackground;
            if (number != null) number.color = FriendlyNumber;
        }
        else
        {
            Background.color = EnemyBackground;
            if (number != null) number.color = EnemyNumber;
        }

        if (portrait != null && unit.modelSprite != null)
            portrait.sprite = unit.modelSprite.sprite;

        if (number != null) number.text = (index + 1).ToString();
        Initiative = unit.Initiative;
    }

    public void RefreshColor()
    {
        if (thisUnit.Friendly)
        {
            Background.color = FriendlyBackground;
            if (number != null) number.color = FriendlyNumber;
        }
        else
        {
            Background.color = EnemyBackground;
            if (number != null) number.color = EnemyNumber;
        }
    }

    public void SetNumber(int index)
    {
        if (number != null) number.text = (index + 1).ToString();
    }

    public void ToggleActive(bool active)
    {
        if (active)
            Border.color = Color.white;
        else
            Border.color = Color.black;
    }
}
