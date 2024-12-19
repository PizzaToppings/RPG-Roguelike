using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    [HideInInspector] public Unit thisUnit;
    [SerializeField] Image healthbar;
    [SerializeField] Image shieldbar;

    int MaxHitpoints => thisUnit.MaxHitpoints;
    int Hitpoints => thisUnit.Hitpoints;
    int ShieldPoints => thisUnit.ShieldPoints;

    public void UpdateHealthbar()
    {
        var maxHealth = MaxHitpoints;
        if (Hitpoints + ShieldPoints > maxHealth)
            maxHealth = Hitpoints + ShieldPoints;

        healthbar.fillAmount = (float)Hitpoints / maxHealth;
        shieldbar.fillAmount = (float)ShieldPoints + Hitpoints / MaxHitpoints;
    }
}
