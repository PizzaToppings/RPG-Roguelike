using UnityEngine;
using TMPro;

public class FloatingDamageNumber : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] AnimationCurve bounceCurve;

    DamageDataCalculated damageData;

    float duration = 0.8f;
    float fadeDuration = 0.3f;

    private Vector3 startPosition;
    private float timer;

    float sideMovementValue = 0;
    float bounceValue = 0;

    public void Init(DamageDataCalculated data, Color textColor)
    {
        damageData = data;

        sideMovementValue = Random.Range(10, 50);
        bounceValue = Random.Range(30, 60);

        text.color = textColor;
        text.text = data.Damage.ToString();

        startPosition = Camera.main.WorldToScreenPoint(data.Target.transform.position);
        transform.position = startPosition;
    }

    void Update()
    {
        if (damageData == null)
            return;

        timer += Time.deltaTime;
        float progress = timer / duration;
        var sideMovement = sideMovementValue * progress;
        var bounceCurveValue = bounceCurve.Evaluate(progress) * bounceValue;

        if (progress < 1f)
        {
            transform.position = Camera.main.WorldToScreenPoint(damageData.Target.transform.position) + new Vector3(sideMovement, bounceCurveValue, 0);
        }
        else if (progress < 1f + fadeDuration)
        {
            transform.position = Camera.main.WorldToScreenPoint(damageData.Target.transform.position) + new Vector3(sideMovement, bounceCurveValue, 0);
            float fadeProgress = (progress - 1f) / fadeDuration;
            text.alpha = Mathf.Lerp(1f, 0f, fadeProgress);
        }
        else
        {
            Destroy(gameObject); // TODO cache
        }
    }
}
