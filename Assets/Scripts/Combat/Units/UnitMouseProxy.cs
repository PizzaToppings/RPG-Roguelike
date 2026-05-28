using UnityEngine;

/// <summary>
/// Attach this to the child GameObject that contains the Collider/Collider2D
/// when the collider has been moved off the root Unit GameObject. This proxy
/// forwards Unity mouse events to the parent Unit component so existing
/// OnMouseXXX handlers continue to work.
/// </summary>
[RequireComponent(typeof(PolygonCollider2D))]
public class UnitMouseProxy : MonoBehaviour
{
    Unit cachedUnit;

    Unit GetUnit()
    {
        if (cachedUnit == null)
            cachedUnit = GetComponentInParent<Unit>();
        return cachedUnit;
    }

    void OnMouseEnter()
    {
        var unit = GetUnit();
        if (unit != null)
            unit.OnMouseEnter();
    }

    void OnMouseExit()
    {
        var unit = GetUnit();
        if (unit != null)
            unit.OnMouseExit();
    }

    void OnMouseDown()
    {
        var unit = GetUnit();
        if (unit != null)
            unit.OnMouseDown();
    }

    void OnMouseUp()
    {
        var unit = GetUnit();
        if (unit == null) return;

        // Forward click-like behaviour when left mouse button is released
        if (Input.GetMouseButtonUp(0))
            unit.OnClick();
    }
}
