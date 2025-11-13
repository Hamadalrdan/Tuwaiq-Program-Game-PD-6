using UnityEngine;

public class CameraLook : MonoBehaviour
{
    public Transform playerBody;   // اسحب Player هنا
    public float sensitivity = 2f; // قلّل لو الحركة سريعة
    public float smooth = 10f;     // سلاسة الدوران

    float xRot;                    // دوران الكاميرا عمودياً

    void Start()
    {

    }

    void LateUpdate()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        // لفّ اللاعب حول محور Y (أفقي)
        playerBody.Rotate(Vector3.up * mouseX);

        // لفّ الكاميرا حول محور X (عمودي) مع تقييد
        xRot -= mouseY;
        xRot = Mathf.Clamp(xRot, -55f, 55f);

        // سلاسة في تدوير الكاميرا
        Quaternion target = Quaternion.Euler(xRot, 0f, 0f);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, target, smooth * Time.deltaTime);
    }
}