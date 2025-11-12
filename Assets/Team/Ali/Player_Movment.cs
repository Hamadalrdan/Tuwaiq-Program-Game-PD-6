using DoorScript;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Player_Movement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float gravity = 9.81f;
    public float jumpForce = 5f;

    [Header("Mouse Look Settings")]
    public Transform cameraTransform;   // اسحب Main Camera هنا
    public float mouseSensitivity = 200f;
    public float minPitch = -60f;
    public float maxPitch = 80f;

    [Header("Interaction")]
    public float interactDistance = 3f;

    [Header("Inventory")]
    public bool hasKey = false; // ✅ اللاعب عنده المفتاح أو لا

    private CharacterController cc;
    private float yVel;
    private float yaw;   // دوران أفقي
    private float pitch; // دوران رأسي

    void Awake()
    {
        cc = GetComponent<CharacterController>();

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        yaw = transform.eulerAngles.y;
        if (cameraTransform != null)
            pitch = cameraTransform.localEulerAngles.x;
    }

    void Update()
    {
        // ========== 1) دوران الماوس ==========
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        transform.rotation = Quaternion.Euler(0f, yaw, 0f);
        if (cameraTransform != null)
            cameraTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);

        // ========== 2) حركة اللاعب ==========
        float x = 0f, z = 0f;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) x = 1f;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) x = -1f;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) z = 1f;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) z = -1f;

        Vector3 input = new Vector3(x, 0f, z).normalized;
        Vector3 move = transform.TransformDirection(input) * moveSpeed;

        // ========== 3) الجاذبية + القفز ==========
        if (cc.isGrounded)
        {
            if (yVel < 0f)
                yVel = -1f;

            if (Input.GetKeyDown(KeyCode.Space))
                yVel = jumpForce;
        }
        else
        {
            yVel -= gravity * Time.deltaTime;
        }

        move.y = yVel;
        cc.Move(move * Time.deltaTime);

        // ========== 4) التفاعل (Q للمفتاح / E للباب) ==========
        if (Input.GetKeyDown(KeyCode.Q))
        {
            TryPickUpKey();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            TryOpenDoor();
        }
    }

    // ✅ أخذ المفتاح بالزر Q
    void TryPickUpKey()
    {
        Transform rayOrigin = cameraTransform != null ? cameraTransform : transform;
        Ray ray = new Ray(rayOrigin.position, rayOrigin.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
        {
            if (hit.collider.CompareTag("Key"))
            {
                hasKey = true;
                Destroy(hit.collider.gameObject);
                Debug.Log("🔑 You picked up a key!");
            }
        }
    }

    // ✅ فتح الباب بالزر E
    void TryOpenDoor()
    {
        Transform rayOrigin = cameraTransform != null ? cameraTransform : transform;
        Ray ray = new Ray(rayOrigin.position, rayOrigin.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
        {
            if (hit.collider.CompareTag("Door"))
            {
                Door door = hit.collider.GetComponent<Door>();
                if (door != null)
                {
                    if (hasKey)
                    {
                        door.OpenDoor();
                        Debug.Log("🚪 Door opened!");
                        // لو تبي المفتاح يُستخدم مرة واحدة:
                        // hasKey = false;
                    }
                    else
                    {
                        Debug.Log("❌ You need a key!");
                    }
                }
            }
        }
    }
}
