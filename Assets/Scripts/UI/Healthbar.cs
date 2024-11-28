using UnityEngine;

public class Healthbar : MonoBehaviour
{
    Transform unit;
    Vector3 offset = Vector3.up * 2f;

    Camera camera;

    public void Init(Transform myUnit)
    {
        camera = Camera.main;
        unit = myUnit.transform;
    }

    void Update()
    {
        if (unit == null || camera == null)
            return;

        Vector3 screenPosition = camera.WorldToScreenPoint(unit.position + offset);

        transform.position = screenPosition;
    }
}
