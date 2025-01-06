using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    UI_Singletons ui_Singletons = UI_Singletons.Instance;

    [HideInInspector] public Unit thisUnit;
    [SerializeField] Image healthbar;
    [SerializeField] Image shieldbar;

    [SerializeField] List<Image> statusEffectImages;

    List<StatusEfectEnum> statusEffects = new List<StatusEfectEnum>();

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

    public void AddStatusEffect(StatusEfectEnum statusEffect)
	{
        statusEffects.Add(statusEffect);

        ShowStatusEffects();
    }

    public void RemoveStatusEffect(StatusEfectEnum statusEffect)
    {
        if (statusEffects.Contains(statusEffect))
            statusEffects.Remove(statusEffect);

        ShowStatusEffects();
    }

    void ShowStatusEffects()
	{
        statusEffectImages.ForEach(x => x.enabled = false);

        int imageIndex = 0;
        for (int i = 0; i < statusEffects.Count; i++)
        {
            var image = ui_Singletons.GetStatusIcon(statusEffects[i]);
            if (image == null)
                continue;

            statusEffectImages[i].sprite = image;
            statusEffectImages[i].enabled = true;

            imageIndex++;
            if (imageIndex >= statusEffectImages.Count)
                return;
        }
    }
}
