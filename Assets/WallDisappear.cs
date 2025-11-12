using UnityEngine;

public class WallDisappear : MonoBehaviour
{
    public float disappearTime = 30f; // الوقت بالثواني (4 دقائق)

    void Start()
    {
        // بعد 4 دقائق، يتم تعطيل الجدار
        Invoke(nameof(DisableWall), disappearTime);
    }

    void DisableWall()
    {
        gameObject.SetActive(false);
        Debug.Log("🧱 الجدار اختفى بعد 4 دقائق.");
    }
}
