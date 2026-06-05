using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CharacterSelectTraitIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    SO_Trait trait;

    [SerializeField] Image image;

    public void Set(SO_Trait _trait)
    {
        trait = _trait;
        if (image == null) image = GetComponent<Image>();
        if (image != null && trait != null && trait.Image != null)
            image.sprite = trait.Image;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (trait == null) return;
        CharacterSelectUI.Instance?.StartShowTraitInformation(trait, GetComponent<RectTransform>());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (trait == null) return;
        CharacterSelectUI.Instance?.EndShowTraitInformation(trait);
    }

    public void OnPointerEnter() => OnPointerEnter(null);
    public void OnPointerExit() => OnPointerExit(null);
}
