using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InitiativeInformation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image Background;
    public Image Border;
    public Image portrait;

    public Color FriendlyBackground;
    public Color FriendlyNumber;

    public Color EnemyBackground;
    public Color EnemyNumber;

    public Unit thisUnit;

    public int Initiative = 0;

    [Tooltip("Assign the inner RectTransform that contains the visual content (child of this object). The layout group owns this object's root; we offset the content root instead so the layout is unaffected.")]
    public RectTransform ContentRoot;
    public float ActiveTurnOffset = -10f;

    public void Init(Unit unit)
    {
        thisUnit = unit;
        thisUnit.initiativeInformation = this;
        if (unit.Friendly)
        {
            Background.color = FriendlyBackground;
        }
        else
        {
            Background.color = EnemyBackground;
        }

        if (portrait != null && unit.modelSprite != null)
            portrait.sprite = unit.modelSprite.sprite;

        Initiative = unit.Initiative;
    }

    public void RefreshColor()
    {
        if (thisUnit.Friendly)
        {
            Background.color = FriendlyBackground;
        }
        else
        {
            Background.color = EnemyBackground;
        }
    }

    public void ToggleHover(bool hovered)
    {
        if (Border == null) return;
        Border.color = hovered ? Color.white : Color.black;
    }

    public void SetActiveTurn(bool active)
    {
        if (ContentRoot == null) return;
        var pos = ContentRoot.anchoredPosition;
        pos.y = active ? ActiveTurnOffset : 0f;
        ContentRoot.anchoredPosition = pos;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        thisUnit.Tile.Target();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        thisUnit.Tile.UnTarget();
    }
}
