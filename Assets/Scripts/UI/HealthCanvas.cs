using UnityEngine;

public class HealthCanvas : MonoBehaviour
{
    public static HealthCanvas Instance;

    UI_Singletons ui_Singletons;

    public GameObject HealthBar;
    public Transform HealthBarCanvas;

    [Space]
    public GameObject DamageNumber;
    public GameObject HealNumber;
    public GameObject StatusEfectText;
    public Transform DamageNumbersCanvas;


    public void Init()
    {
        Instance = this;
        ui_Singletons = UI_Singletons.Instance;

        foreach (var unit in UnitData.Enemies)
        {
            CreateHealthbar(unit);
        }
    }

    public void CreateHealthbar(Enemy unit)
	{
        var hb = Instantiate(HealthBar, HealthBarCanvas);
        var healthbar = hb.GetComponent<FloatingHealthbar>();
        healthbar.Init(unit);
        unit.ThisHealthbar = healthbar;
    }

    public void ShowDamageNumber(DamagaDataResolved data)
    {
        var color = ui_Singletons.GetDamageTypeColor(data.DamageType);
        var dn = Instantiate(DamageNumber, DamageNumbersCanvas);
        var damageNumber = dn.GetComponent<FloatingDamageNumber>();
        damageNumber.Init(data, color);
    }

    public void ShowHealNumber(DamagaDataResolved data)
    {
        var color = ui_Singletons.GetDamageTypeColor(data.DamageType);
        var hn = Instantiate(HealNumber, DamageNumbersCanvas);
        var healNumber = hn.GetComponent<FloatingHealNumber>();
        healNumber.Init(data, color);
    }

    public void ShowStatusEffect(string displayText, Unit target, bool isBuff)
    {
        var se = Instantiate(StatusEfectText, DamageNumbersCanvas);
        var statusText = se.GetComponent<FloatingStatusEffectText>();
        statusText.Init(displayText, target, isBuff);
    }
}
