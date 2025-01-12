using UnityEngine;
using TMPro;
using System.Collections;

public class FloatingStatusEffectText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;

    Unit target;

    float duration = 0.8f;
    float fadeDuration = 0.3f;
    float shakeAmount = 10f;

    bool isBuff;

    private Vector3 startPosition;

    public void Init(string displayText, Unit target, bool isBuff)
    {
        text.color = Color.grey; // change?
        text.text = displayText;

        this.isBuff = isBuff;

        this.target = target;

        startPosition = Camera.main.WorldToScreenPoint(target.transform.position);
        transform.position = startPosition;

        StartCoroutine(AnimateText());
    }

    private IEnumerator AnimateText()
    {
        float timer = 0f;

        while (timer < duration + fadeDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / duration;
            var upMovement = 10 * progress;
            float offset = 0;

            if (progress < 1f)
            {
                if (isBuff == false)
                    offset = Mathf.Sin(Time.time * 30f) * shakeAmount;

                transform.position = Camera.main.WorldToScreenPoint(target.transform.position) + new Vector3(offset, upMovement, 0) + Vector3.up * 80f;
            }
            else if (progress < 1f + fadeDuration)
            {
                transform.position = Camera.main.WorldToScreenPoint(target.transform.position) + new Vector3(0, upMovement, 0) + Vector3.up * 80f;
                float fadeProgress = (progress - 1f) / fadeDuration;
                text.alpha = Mathf.Lerp(1f, 0f, fadeProgress);
            }

            yield return null;
        }

        Destroy(gameObject); // TODO cache
    }
}