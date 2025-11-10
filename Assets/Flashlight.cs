using UnityEngine;

public class FlashlightToggle : MonoBehaviour
{
    [Header("Light")]
    [Tooltip("اسحب مكوّن Light الخاص بالفلاش هنا (Spot/Point).")]
    public Light flashLight;

    [Header("Controls")]
    public KeyCode toggleKey = KeyCode.F;

    [Header("Optional Fade")]
    [Tooltip("فعّل التدرّج بدل التشغيل الفوري.")]
    public bool useFade = true;
    [Tooltip("قوة الإضاءة عند التشغيل.")]
    public float onIntensity = 1.8f;
    [Tooltip("مدة التدرّج بالثواني.")]
    public float fadeDuration = 0.15f;

    [Header("Optional Sound")]
    public AudioSource clickSfx;

    bool isOn;
    float offIntensity = 0f;
    Coroutine fadeCo;

    void Reset()
    {
        // محاولة إيجاد ضوء تلقائياً لو نسيته
        if (!flashLight) flashLight = GetComponentInChildren<Light>();
    }

    void Awake()
    {
        if (!flashLight)
        {
            Debug.LogWarning("[FlashlightToggle] ما لقيت Light، اسحبه في المتغيّر flashLight.");
            return;
        }

        // يبدأ مطفّي
        isOn = false;
        flashLight.enabled = false;
        flashLight.intensity = offIntensity;
    }

    void Update()
    {
        if (!flashLight) return;

        if (Input.GetKeyDown(toggleKey))
        {
            isOn = !isOn;
            if (clickSfx) clickSfx.Play();

            if (useFade)
            {
                if (fadeCo != null) StopCoroutine(fadeCo);
                fadeCo = StartCoroutine(FadeLight(isOn));
            }
            else
            {
                flashLight.enabled = isOn;
                flashLight.intensity = isOn ? onIntensity : offIntensity;
            }
        }
    }

    System.Collections.IEnumerator FadeLight(bool turnOn)
    {
        float start = flashLight.intensity;
        float target = turnOn ? onIntensity : offIntensity;

        // فعّل اللمبة قبل الرفع، واطفِها بعد النزول
        if (turnOn) flashLight.enabled = true;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / Mathf.Max(0.0001f, fadeDuration);
            flashLight.intensity = Mathf.Lerp(start, target, t);
            yield return null;
        }

        flashLight.intensity = target;
        if (!turnOn) flashLight.enabled = false;
    }
}
