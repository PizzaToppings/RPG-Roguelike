using UnityEngine;
using TMPro;
using System.Collections;

public class FloatingDamageNumber : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] AnimationCurve bounceCurve;

    DamagaDataResolved damageDataResolved;

    float duration = 0.8f;
    float fadeDuration = 0.3f;

    private Vector3 startPosition;

    float sideMovementValue = 0;
    float bounceValue = 0;

    public void Init(DamagaDataResolved data, Color textColor)
    {
        damageDataResolved = data;

        bool isNegative = Random.Range(0, 2) == 1;
        sideMovementValue = Random.Range(10, 50);
        if (isNegative)
            sideMovementValue *= -1;

        bounceValue = Random.Range(30, 60);

        text.color = textColor;
        text.text = data.DamageDone.ToString();

        if (data.ShieldDamage != 0)
            text.text += $"\n <color=#D7D7D7> ({data.ShieldDamage} blocked)";

        startPosition = Camera.main.WorldToScreenPoint(data.Target.transform.position);
        transform.position = startPosition;

        StartCoroutine(AnimateDamageText());
    }

    private IEnumerator AnimateDamageText()
    {
        if (damageDataResolved == null)
            yield break;

        float timer = 0f;

        while (timer < duration + fadeDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / duration;
            var sideMovement = sideMovementValue * progress;
            var bounceCurveValue = bounceCurve.Evaluate(progress) * bounceValue;

            if (progress < 1f)
            {
                transform.position = Camera.main.WorldToScreenPoint(damageDataResolved.Target.transform.position)
                                     + new Vector3(sideMovement, bounceCurveValue, 0);
            }
            else if (progress < 1f + fadeDuration)
            {
                float fadeProgress = (progress - 1f) / fadeDuration;
                text.alpha = Mathf.Lerp(1f, 0f, fadeProgress);
            }

            yield return null; // Wait for the next frame
        }

        Destroy(gameObject); // TODO cache
    }
}
