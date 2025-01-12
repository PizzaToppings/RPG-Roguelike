using UnityEngine;
using TMPro;
using System.Collections;

public class FloatingHealNumber : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;

    DamagaDataResolved damageDataResolved;

    float duration = 0.8f;
    float fadeDuration = 0.3f;

    private Vector3 startPosition;

    public void Init(DamagaDataResolved data, Color textColor)
    {
        damageDataResolved = data;
        text.color = textColor;
        
        if (data.DamageDone != 0)
            text.text = data.DamageDone.ToString();
        else if (data.ShieldDamage < 0)
		{
            var amount = data.ShieldDamage * -1;
            text.text = amount.ToString();
		}

        startPosition = Camera.main.WorldToScreenPoint(data.Target.transform.position);
        transform.position = startPosition;

        StartCoroutine(AnimateHealText());
    }

    private IEnumerator AnimateHealText()
    {
        if (damageDataResolved == null)
            yield break;

        float timer = 0f;

        while (timer < duration + fadeDuration)
        {
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

            yield return null;
        }

        Destroy(gameObject); // TODO cache
    }
}


