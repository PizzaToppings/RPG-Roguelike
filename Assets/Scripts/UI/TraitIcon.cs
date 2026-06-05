using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TraitIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    SO_Trait traitSO;
    public Image image;
    Coroutine animCoroutine;

    void Awake()
    {
        if (image == null) image = GetComponent<Image>();
    }

    public void Set(SO_Trait so)
    {
        traitSO = so;
        if (image != null && traitSO != null && traitSO.Image != null)
            image.sprite = traitSO.Image;
        gameObject.SetActive(true);
        // prepare for animated appearance
        transform.localScale = Vector3.zero;
    }

    public void Clear()
    {
        traitSO = null;
        if (image != null) image.sprite = null;
        // stop any running animation and reset
        if (animCoroutine != null)
            StopCoroutine(animCoroutine);
        transform.localScale = Vector3.one;
        gameObject.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (traitSO == null) return;
        UIManager.Instance?.StartShowTraitInformation(traitSO, GetComponent<RectTransform>());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (traitSO == null) return;
        UIManager.Instance?.EndShowTraitInformation(traitSO);
    }

    // Convenience for non-event calls
    public void OnPointerEnter() => OnPointerEnter(null);
    public void OnPointerExit() => OnPointerExit(null);

    public void AnimateIn(float delay = 0f, float duration = 0.12f)
    {
        if (animCoroutine != null)
            StopCoroutine(animCoroutine);
        animCoroutine = StartCoroutine(AnimateInCoroutine(delay, duration));
    }

    private System.Collections.IEnumerator AnimateInCoroutine(float delay, float duration)
    {
        if (delay > 0f)
            yield return new WaitForSeconds(delay);

        float t = 0f;
        transform.localScale = Vector3.zero;
        while (t < duration)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / duration);
            // ease out
            float eased = 1f - Mathf.Pow(1f - p, 3f);
            transform.localScale = Vector3.LerpUnclamped(Vector3.zero, Vector3.one, eased);
            yield return null;
        }
        transform.localScale = Vector3.one;
        animCoroutine = null;
    }
}
