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
        var damageNumber = GetOrCreateFloatingNumber<FloatingDamageNumber>(DamageNumber);
        var color = ui_Singletons.GetDamageTypeColor(data.DamageType);
        damageNumber.Init(data, color);
    }

    public void ShowHealNumber(DamagaDataResolved data)
    {
        var healNumber = GetOrCreateFloatingNumber<FloatingHealNumber>(HealNumber);
        var color = ui_Singletons.GetDamageTypeColor(data.DamageType);
        healNumber.Init(data, color);
    }

    public void ShowStatusEffect(string displayText, Unit target, bool isBuff)
    {
        var statusText = GetOrCreateFloatingNumber<FloatingStatusEffectText>(HealNumber); // Assuming HealNumber is correct here
        statusText.Init(displayText, target, isBuff);
    }

    private T GetOrCreateFloatingNumber<T>(GameObject prefab) where T : Component
    {
        T floatingNumber = null;

        if (DamageNumbersCanvas.childCount > 0)
		{
            foreach (Transform childTransform in DamageNumbersCanvas.transform)
            {
                var child = childTransform.gameObject;

                if (child == null)
                    continue;

                if (child.activeSelf)
                    continue;

                if (child.TryGetComponent<T>(out floatingNumber))
                    break;
            }
		}

        if (floatingNumber == null)
        {
            var instance = Instantiate(prefab, DamageNumbersCanvas);
            floatingNumber = instance.GetComponent<T>();
        }

        return floatingNumber;
    }
}
