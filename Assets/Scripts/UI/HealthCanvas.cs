using UnityEngine;

public class HealthCanvas : MonoBehaviour
{
    [SerializeField] DamageTypeColor damageTypeColor;

    public GameObject HealthBar;
    public Transform HealthBarCanvas;

    [Space]
    public GameObject DamageNumber;
    public Transform DamageNumbersCanvas;


    public void Init()
    {
        foreach (var unit in UnitData.Enemies)
        {
            var hb = Instantiate(HealthBar, HealthBarCanvas);
            var healthbar = hb.GetComponent<Healthbar>();
            healthbar.Init(unit.transform);
        }
    }

    public void ShowDamageNumber(DamageData data)
    {
        var color = damageTypeColor.GetDamageTypeColor(data.DamageType);
        var dn = Instantiate(DamageNumber, DamageNumbersCanvas);
        var damageNumber = dn.GetComponent<FloatingDamageNumber>();
        damageNumber.Init(data, color);
    }
}
