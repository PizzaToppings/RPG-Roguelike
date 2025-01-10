using UnityEngine;
using TMPro;

public class FloatingStatusEffectText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] AnimationCurve bounceCurve;

    Unit target;

    float duration = 0.8f;
    float fadeDuration = 0.3f;
    float shakeAmount = 10f;

    private Vector3 startPosition;
    private float timer;

    public void Init(string displayText, Unit target, bool isBuff)
    {
        text.color = Color.grey; // change?
        text.text = displayText;

        this.target = target;

        startPosition = Camera.main.WorldToScreenPoint(target.transform.position);
        transform.position = startPosition;
    }

    void Update()
    {
        timer += Time.deltaTime;
        float progress = timer / duration;
        var upMovement = 10 * progress;

        if (progress < 1f)
        {
            float offset = Mathf.Sin(Time.time * 20f) * shakeAmount;
            transform.position = Camera.main.WorldToScreenPoint(target.transform.position) + new Vector3(offset, upMovement, 0) + Vector3.up * 2f;
        }
        else if (progress < 1f + fadeDuration)
        {
            transform.position = Camera.main.WorldToScreenPoint(target.transform.position) + new Vector3(0, upMovement, 0) + Vector3.up * 2f;
            float fadeProgress = (progress - 1f) / fadeDuration;
            text.alpha = Mathf.Lerp(1f, 0f, fadeProgress);
        }
        else
        {
            Destroy(gameObject); // TODO cache
        }
    }
}