using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    [Header("References")]
    public Transform player;       // ÇááÇÚÈ
    public Transform cameraPivot;  // äŞØÉ ÇáßÇãíÑÇ (ÚÇÏÉ Êßæä ÇáÑÃÓ)

    [Header("Mouse Settings")]
    public float mouseSensitivity = 120f;
    public float rotationSmooth = 10f;
    public Vector2 pitchLimits = new Vector2(-60, 80);

    [Header("Camera Distance")]
    public float distance = 3f;     // ÈÚÏ ÇáßÇãíÑÇ Úä ÇáÑÃÓ
    public float shoulderOffsetX = 0.5f; // ÅÒÇÍÉ ÇáßÊİ (ÇÎÊíÇÑí)
    public float shoulderOffsetY = 0.2f;

    [Header("Player Rotation")]
    public float playerTurnSmooth = 8f; // ÓÑÚÉ ÇáÊİÇİ ÇááÇÚÈ ÚäÏ ÇáãÔí İŞØ
    public KeyCode recenterKey = KeyCode.Q; // ÒÑ áÅÚÇÏÉ ÇáßÇãíÑÇ Îáİ ÇááÇÚÈ

    private float yaw;   // ÏæÑÇä ÃİŞí (íãíä/íÓÇÑ)
    private float pitch; // ÏæÑÇä ÑÃÓí (İæŞ/ÊÍÊ)
    private Vector3 currentRotation;
    private Vector3 rotationSmoothVelocity;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (cameraPivot == null)
            cameraPivot = player;

        Vector3 rot = transform.eulerAngles;
        yaw = rot.y;
        pitch = rot.x;
    }

    void LateUpdate()
    {
        if (player == null) return;

        // --- 1) ÇáÊŞÇØ ÍÑßÉ ÇáãÇæÓ ---
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        yaw += mouseX * mouseSensitivity * Time.deltaTime;
        pitch -= mouseY * mouseSensitivity * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, pitchLimits.x, pitchLimits.y);

        // --- 2) ÊØÈíŞ ÇáÏæÑÇä Úáì ÇáßÇãíÑÇ (ÈÍÑíÉ ßÇãáÉ) ---
        Vector3 targetRotation = new Vector3(pitch, yaw);
        currentRotation = Vector3.SmoothDamp(currentRotation, targetRotation,
                                             ref rotationSmoothVelocity, 1f / rotationSmooth);
        transform.eulerAngles = currentRotation;

        // --- 3) ÍÓÇÈ ãæŞÚ ÇáßÇãíÑÇ Íæá ÇáÑÃÓ ---
        Vector3 pivotPos = cameraPivot.position +
                           new Vector3(shoulderOffsetX, shoulderOffsetY, 0);
        Vector3 dir = transform.rotation * Vector3.back;
        transform.position = pivotPos - dir * distance;

        // --- 4) ÌÚá ÇááÇÚÈ íÏæÑ İŞØ ÚäÏ ÇáÊÍÑß ááÃãÇã ---
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 moveDir = new Vector3(h, 0, v);

        if (moveDir.magnitude > 0.1f)
        {
            // İŞØ ÚäÏãÇ íÊÍÑß¡ ÇááÇÚÈ íáÊİ äÍæ ÇáßÇãíÑÇ (Yaw İŞØ)
            Quaternion targetRot = Quaternion.Euler(0, yaw, 0);
            player.rotation = Quaternion.Slerp(player.rotation, targetRot, Time.deltaTime * playerTurnSmooth);
        }

        // --- 5) ÅÚÇÏÉ ÇáßÇãíÑÇ Îáİ ÇááÇÚÈ ÈÒÑ (ÇÎÊíÇÑí) ---
        if (Input.GetKeyDown(recenterKey))
        {
            yaw = player.eulerAngles.y;
        }
    }
}
