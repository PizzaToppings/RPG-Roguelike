using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TraitInfoScreen : MonoBehaviour
{
    UI_Singletons ui_Singletons;

    [SerializeField] TextMeshProUGUI traitName;
    [SerializeField] TextMeshProUGUI traitDescription;

    public void Activate(SO_Trait trait, RectTransform anchor = null)
    {
        if (ui_Singletons == null)
            ui_Singletons = UI_Singletons.Instance;

        if (trait == null)
            return;

        traitName.text = trait.TraitName;

        traitDescription.text = trait.Description;

        gameObject.SetActive(true);

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());

        if (anchor != null)
            PositionAt(anchor);
    }

    public void PositionAt(RectTransform anchor)
    {
        if (anchor == null)
            return;

        var thisRT = GetComponent<RectTransform>();
        if (thisRT == null)
            return;

        var parentRT = thisRT.parent as RectTransform;
        if (parentRT == null)
            return;

        // Camera to use for conversions (null for ScreenSpaceOverlay)
        var canvas = thisRT.GetComponentInParent<Canvas>();
        var cam = canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay ? canvas.worldCamera : null;

        // anchor top-center in world space
        Vector3[] corners = new Vector3[4];
        anchor.GetWorldCorners(corners); // 0=bl,1=tl,2=tr,3=br
        Vector3 anchorTopCenterWorld = (corners[1] + corners[2]) * 0.5f;

        // convert to parent local coordinates
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(cam, anchorTopCenterWorld);
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRT, screenPoint, cam, out localPoint);

        // Only compute and set X; keep current Y
        float w = thisRT.rect.width;
        float pX = thisRT.pivot.x;

        float anchoredX = localPoint.x + w * (0.5f - pX);

        // clamp horizontally
        float pw = parentRT.rect.width * 0.5f;
        float minAnchoredX = -pw + w * pX;
        float maxAnchoredX = pw - w * (1f - pX);
        if (minAnchoredX > maxAnchoredX) anchoredX = 0f; else anchoredX = Mathf.Clamp(anchoredX, minAnchoredX, maxAnchoredX);

        float anchoredY = thisRT.anchoredPosition.y;
        thisRT.anchoredPosition = new Vector2(anchoredX, anchoredY);
    }

    public void Deactivate() => gameObject.SetActive(false);
}
