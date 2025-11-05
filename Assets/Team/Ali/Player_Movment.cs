using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Player_Movement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float gravity = 9.81f;
    public float rotationSpeed = 5f; // <-- هنا نتحكم بسرعة اللف

    private CharacterController cc;
    private float yVel;

    void Awake() => cc = GetComponent<CharacterController>();

    void Update()
    {
        float x = 0f;
        float z = 0f;

        // الأسهم + WASD
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) x = 1f;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))  x = -1f;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))    z = 1f;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))  z = -1f;

        Vector3 input = new Vector3(x, 0f, z).normalized;
        Vector3 move = input * moveSpeed;

        // جاذبية
        yVel = cc.isGrounded ? -1f : yVel - gravity * Time.deltaTime;
        move.y = yVel;

        cc.Move(move * Time.deltaTime);

        // تخفيف سرعة الدوران
        Vector3 flat = new Vector3(move.x, 0, move.z);
        if (flat.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(flat),
                Time.deltaTime * rotationSpeed  // <-- هنا تم تعديل السرعة
            );
    }
}