using UnityEngine;
using TMPro;

public class FloatingHealNumber : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] AnimationCurve bounceCurve;

    DamagaDataResolved damageDataResolved;

    float duration = 0.8f;
    float fadeDuration = 0.3f;

    private Vector3 startPosition;
    private float timer;

    public void Init(DamagaDataResolved data, Color textColor)
    {
        damageDataResolved = data;

        text.color = textColor;
        text.text = data.DamageDone.ToString();

        startPosition = Camera.main.WorldToScreenPoint(data.Target.transform.position);
        transform.position = startPosition;
    }

    void Update()
    {
        if (damageDataResolved == null)
            return;

        timer += Time.deltaTime;
        float progress = timer / duration;
        var upMovement = 30 * progress;

        if (progress < 1f)
        {
            transform.position = Camera.main.WorldToScreenPoint(damageDataResolved.Target.transform.position) + new Vector3(0, upMovement, 0);
        }
        else if (progress < 1f + fadeDuration)
        {
            transform.position = Camera.main.WorldToScreenPoint(damageDataResolved.Target.transform.position) + new Vector3(0, upMovement, 0);
            float fadeProgress = (progress - 1f) / fadeDuration;
            text.alpha = Mathf.Lerp(1f, 0f, fadeProgress);
        }
        else
        {
            Destroy(gameObject); // TODO cache
        }
    }
}


