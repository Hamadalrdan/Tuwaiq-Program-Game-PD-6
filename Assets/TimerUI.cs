using UnityEngine;
using TMPro;

public class TimerUI : MonoBehaviour
{
    [Header("Timer Settings")]
    public float startTime = 180f;      // الوقت الكلي بالثواني (مثلاً 3 دقائق)
    private float remainingTime;

    [Header("UI")]
    public TMP_Text timerText;          // رابط نص التايمر
    public Color normalColor = Color.white;
    public Color dangerColor = Color.red;

    [Header("Warning Sound")]
    public AudioSource warningSound;    // صوت التحذير
    public float warningTime = 10f;     // عند كم ثانية يشغل الصوت
    private bool warningPlayed = false;

    void Start()
    {
        remainingTime = startTime;

        if (timerText != null)
            timerText.color = normalColor;
    }

    void Update()
    {
        if (remainingTime <= 0f)
        {
            remainingTime = 0f;

            // هنا تقدر تضيف كود الخسارة مثلاً:
            // losePanel.SetActive(true);

            return;
        }

        // نقلل الوقت
        remainingTime -= Time.deltaTime;

        // نحسب الدقايق والثواني
        int minutes = Mathf.FloorToInt(remainingTime / 60f);
        int seconds = Mathf.FloorToInt(remainingTime % 60f);

        if (timerText != null)
            timerText.text = $"{minutes:00}:{seconds:00}";

        // لو باقي أقل من دقيقة يخلي اللون أحمر
        if (remainingTime <= 60f && timerText != null)
        {
            timerText.color = dangerColor;
        }

        // لو قرب يخلص الوقت يشغل صوت التحذير مرة وحدة
        if (!warningPlayed && remainingTime <= warningTime)
        {
            if (warningSound != null)
                warningSound.Play();

            warningPlayed = true;
        }
    }
}