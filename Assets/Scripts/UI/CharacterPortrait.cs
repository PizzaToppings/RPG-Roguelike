using UnityEngine;
using UnityEngine.UI;

public class CharacterPortrait : Healthbar
{
    [SerializeField] Image energybar;

    int maxEnergy => (thisUnit as Character).MaxEnergy;
    int energy => (thisUnit as Character).Energy;

    public override void UpdateHealthbar()
    {
        base.UpdateHealthbar();
        energybar.fillAmount = (float)energy / maxEnergy;
    }
}
