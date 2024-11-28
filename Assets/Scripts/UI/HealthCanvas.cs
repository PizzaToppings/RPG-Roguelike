using UnityEngine;

public class HealthCanvas : MonoBehaviour
{
    public GameObject HealthBar;
    public Transform HealthBarCanvas;

    public void Init()
    {
        foreach (var unit in UnitData.Enemies)
        {
            var hb = Instantiate(HealthBar, HealthBarCanvas);
            var healthbar = hb.GetComponent<Healthbar>();
            healthbar.Init(unit.transform);
        }
    }
}
