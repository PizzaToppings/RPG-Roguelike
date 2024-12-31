using UnityEngine;

public class HealthCanvas : MonoBehaviour
{
    UI_Singletons ui_Singletons;

    public GameObject HealthBar;
    public Transform HealthBarCanvas;

    [Space]
    public GameObject DamageNumber;
    public Transform DamageNumbersCanvas;


    public void Init()
    {
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

    public void ShowDamageNumber(DamageData data)
    {
        var color = ui_Singletons.GetDamageTypeColor(data.DamageType);
        var dn = Instantiate(DamageNumber, DamageNumbersCanvas);
        var damageNumber = dn.GetComponent<FloatingDamageNumber>();
        damageNumber.Init(data, color);
    }
}
