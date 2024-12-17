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
        healthbar.fillAmount = (float)Hitpoints / MaxHitpoints;
        shieldbar.fillAmount = (float)ShieldPoints / MaxHitpoints;
    }
}
