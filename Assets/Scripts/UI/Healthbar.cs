using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    [HideInInspector] public Unit thisUnit;
    [SerializeField] Image healthbar;
    [SerializeField] Image shieldbar;

    int maxHitpoints => thisUnit.MaxHitpoints;
    int hitpoints => thisUnit.Hitpoints;
    int shieldPoints => thisUnit.ShieldPoints;

    public virtual void UpdateHealthbar()
    {
        var maxHealth = maxHitpoints;
        if (hitpoints + shieldPoints > maxHealth)
            maxHealth = hitpoints + shieldPoints;

        healthbar.fillAmount = (float)hitpoints / maxHealth;
        shieldbar.fillAmount = (float)(shieldPoints + hitpoints) / maxHealth;
    }
}
