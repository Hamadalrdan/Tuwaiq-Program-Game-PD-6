using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterController))]
public class Player_Movment : MonoBehaviour
{
    // ---------------- UI Lose ----------------
    [Header("Lose UI")]
    [SerializeField] private GameObject losePanel;      // اسحب LosePanel هنا أو عطهِ Tag وخلّه فاضي
    [SerializeField] private string enemyTag = "Enemy"; // تأكد العدو بهذا التاق

    // ---------------- Movement ----------------
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float gravity = 9.81f;
    public float jumpForce = 5f;

    private CharacterController cc;
    private float yVel;

    // ---------------- Health/Death ----------------
    [Header("Health")]
    public int maxHealth = 5;
    public int currentHealth;

    [Header("Death")]
    public float deathDelay = 2f;     // ما نستخدمه الآن؛ نخليها احتياط لو تبي تأخير قبل الإيقاف
    private bool isDead = false;

    // ---------------- Hit reaction ----------------
    [Header("Hit Reaction")]
    public float hitStunTime = 0.25f;
    public float knockbackForce = 6f;
    public float knockbackDamping = 6f;

    private float hitStunTimer = 0f;
    private Vector3 externalVelocity = Vector3.zero;

    // ---------------- VFX/SFX ----------------
    [Header("VFX/SFX (Optional)")]
    public AudioSource sfx;
    public AudioClip hitSfx;
    public AudioClip deathSfx;

    // ---------------- Flashlight ----------------
    [Header("Flashlight")]
    public Transform flashlightSocket;
    public GameObject flashlightObject;
    public Vector3 flashlightLocalPosition = new Vector3(0.05f, -0.02f, 0.11f);
    public Vector3 flashlightLocalEuler = new Vector3(0f, 90f, -15f);
    public bool aimWithCamera = true;
    public KeyCode flashlightToggleKey = KeyCode.F;
    public bool flashlightStartsOn = true;

    private Light flashlightLight;   // cached (created if missing)
    private bool flashlightOn;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        currentHealth = maxHealth;

        // حضّر لوحة الخسارة
        if (losePanel == null)
        {
            // جرّب إيجادها بالتاق لو حاط Tag = LosePanelUI
            var byTag = GameObject.FindGameObjectWithTag("LosePanelUI");
            if (byTag) losePanel = byTag;
            else losePanel = GameObject.Find("LosePanel");
        }
        if (losePanel) losePanel.SetActive(false);
        Time.timeScale = 1f;

        // --- Setup flashlight parenting and cache/create light ---
        if (flashlightObject != null && flashlightSocket != null)
        {
            if (flashlightObject.transform.parent != flashlightSocket)
                flashlightObject.transform.SetParent(flashlightSocket, worldPositionStays: false);

            AlignFlashlightTransform(); // apply offsets

            flashlightLight = flashlightObject.GetComponentInChildren<Light>();
            if (flashlightLight == null)
            {
                GameObject spot = new GameObject("AutoSpot");
                spot.transform.SetParent(flashlightObject.transform, false);
                spot.transform.localPosition = new Vector3(0f, 0f, 0.15f);
                spot.transform.localRotation = Quaternion.identity;
                flashlightLight = spot.AddComponent<Light>();
                flashlightLight.type = LightType.Spot;
                flashlightLight.range = 18f;
                flashlightLight.spotAngle = 45f;
                flashlightLight.intensity = 2f;
                flashlightLight.shadows = LightShadows.Soft;
            }

            flashlightOn = flashlightStartsOn;
            SetFlashlightActive(flashlightOn);
        }
        else
        {
            Debug.LogWarning("[Player_Movment] Flashlight references are not assigned in the Inspector.");
        }
    }

    void Update()
    {
        if (hitStunTimer > 0f) hitStunTimer -= Time.deltaTime;

        bool canControl = !isDead && hitStunTimer <= 0f;
        float x = 0f, z = 0f;
        if (canControl)
        {
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) x = 1f;
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))  x = -1f;
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))    z = 1f;
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))  z = -1f;
        }

        Vector3 inputMove = new Vector3(x, 0f, z).normalized * moveSpeed;

        if (cc.isGrounded)
        {
            yVel = -0.5f;
            if (canControl && Input.GetKeyDown(KeyCode.Space)) yVel = jumpForce;
        }
        else
        {
            yVel -= gravity * Time.deltaTime;
        }

        if (externalVelocity.sqrMagnitude > 0.0001f)
            externalVelocity = Vector3.Lerp(externalVelocity, Vector3.zero, knockbackDamping * Time.deltaTime);

        Vector3 move = (canControl ? inputMove : Vector3.zero) + externalVelocity;
        move.y = yVel;
        cc.Move(move * Time.deltaTime);

        HandleFlashlight(canControl);
    }

    // ---------------- Damage/Death ----------------
    public void TakeDamage(int amount) => TakeDamage(amount, transform.position - transform.forward * 0.1f);

    public void TakeDamage(int amount, Vector3 hitFromWorldPos)
    {
        if (isDead) return;

        currentHealth = Mathf.Max(0, currentHealth - amount);
        if (sfx && hitSfx) sfx.PlayOneShot(hitSfx);
        GetComponent<Animator>()?.SetTrigger("Hit");

        // Knockback
        hitStunTimer = hitStunTime;
        Vector3 dir = (transform.position - hitFromWorldPos);
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.0001f) dir = -transform.forward;
        dir.Normalize();
        externalVelocity = dir * knockbackForce;

        if (currentHealth <= 0) Die();
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        externalVelocity = Vector3.zero;
        yVel = 0f;

        GetComponent<Animator>()?.SetTrigger("Die");
        if (sfx && deathSfx) sfx.PlayOneShot(deathSfx);

        // ⛔ بدلاً من إعادة تحميل المشهد: أظهر شاشة الخسارة وأوقف الزمن
        if (losePanel) losePanel.SetActive(true);
        Time.timeScale = 0f;
        Debug.Log("[Lose] YOU LOST");
    }

    // لو تحتاج إعادة المشهد لاحقًا بزر (Restart) استدعِ هذا
    public void RestartScene()
    {
        Time.timeScale = 1f;
        var scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.buildIndex);
    }

    // ---------------- Collision hooks (تأذي اللاعب عند لمس العدو) ----------------
    void OnTriggerEnter(Collider other)
    {
        if (!isDead && other.CompareTag(enemyTag))
            TakeDamage(1, other.transform.position);
    }

    void OnCollisionEnter(Collision other)
    {
        if (!isDead && other.collider.CompareTag(enemyTag))
            TakeDamage(1, other.transform.position);
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (!isDead && hit.collider != null && hit.collider.CompareTag(enemyTag))
            TakeDamage(1, hit.collider.transform.position);
    }

    // ---------------- Flashlight helpers ----------------
    private void HandleFlashlight(bool canControl)
    {
        if (flashlightObject == null || flashlightSocket == null) return;

        if (canControl && Input.GetKeyDown(flashlightToggleKey))
        {
            flashlightOn = !flashlightOn;
            SetFlashlightActive(flashlightOn);
        }

        AlignFlashlightTransform();

        if (aimWithCamera)
        {
            Camera cam = Camera.main;
            if (cam)
            {
                Vector3 flatForward = Vector3.ProjectOnPlane(cam.transform.forward, Vector3.up).normalized;
                if (flatForward.sqrMagnitude > 0.0001f)
                {
                    Quaternion look = Quaternion.LookRotation(flatForward, Vector3.up);
                    flashlightObject.transform.rotation = look;
                    flashlightObject.transform.localRotation *= Quaternion.Euler(flashlightLocalEuler);
                }
            }
        }
    }

    private void AlignFlashlightTransform()
    {
        if (flashlightObject.transform.parent != flashlightSocket)
            flashlightObject.transform.SetParent(flashlightSocket, false);

        flashlightObject.transform.localPosition = flashlightLocalPosition;
        flashlightObject.transform.localRotation = Quaternion.Euler(flashlightLocalEuler);
        flashlightObject.transform.localScale = Vector3.one;
    }

    private void SetFlashlightActive(bool enable)
    {
        // إذا تبي الجسم يظل ظاهر ويطفي الضوء فقط، علّق السطر الأول وخلي الثاني فقط
        flashlightObject.SetActive(enable);
        if (flashlightLight) flashlightLight.enabled = enable;
    }
}
