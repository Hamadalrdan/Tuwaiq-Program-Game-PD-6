using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterController))]
public class Player_Movment : MonoBehaviour
{
    // ==================== UI Lose ====================
    [Header("Lose UI")]
    [SerializeField] private GameObject losePanel;
    [SerializeField] private string enemyTag = "Enemy";

    // ==================== Movement ====================
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float gravity = 9.81f;
    public float jumpForce = 5f;

    private CharacterController cc;
    private float yVel;

    // ==================== Health/Death ====================
    [Header("Health")]
    public int maxHealth = 5;
    public int currentHealth;

    [Header("Death")]
    public float deathDelay = 2f;
    private bool isDead = false;

    // ==================== Hit reaction ====================
    [Header("Hit Reaction")]
    public float hitStunTime = 0.25f;
    public float knockbackForce = 6f;
    public float knockbackDamping = 6f;

    // يمنع تعدد الضربات في نفس اللحظة
    [Tooltip("وقت عدم القابلية للضرر بعد الضربة (ثانية)")]
    public float invulnerableAfterHit = 0.25f;
    private float lastHitTime = -999f;

    private float hitStunTimer = 0f;
    private Vector3 externalVelocity = Vector3.zero;

    // ==================== VFX/SFX ====================
    [Header("VFX/SFX (Optional)")]
    public AudioSource sfx;
    public AudioClip hitSfx;
    public AudioClip deathSfx;

    // ==================== Flashlight ====================
    [Header("Flashlight")]
    public Transform flashlightSocket;
    public GameObject flashlightObject;
    public Vector3 flashlightLocalPosition = new Vector3(0.05f, -0.02f, 0.11f);
    public Vector3 flashlightLocalEuler = new Vector3(0f, 90f, -15f);
    public bool aimWithCamera = true;
    public KeyCode flashlightToggleKey = KeyCode.F;
    public bool flashlightStartsOn = true;

    private Light flashlightLight;
    private bool flashlightOn;

    // ==================== Animator ====================
    [Header("Animator")]
    public Animator anim;

    private static readonly int SpeedHash    = Animator.StringToHash("Speed");
    private static readonly int IsMovingHash = Animator.StringToHash("IsMoving");
    private static readonly int HitHash      = Animator.StringToHash("Hit");
    private static readonly int DieHash      = Animator.StringToHash("Die");

    private bool hasSpeed, hasIsMoving, hasHit, hasDie;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        currentHealth = maxHealth;

        // حضّر لوحة الخسارة
        if (losePanel == null)
        {
            var byTag = GameObject.FindGameObjectWithTag("LosePanelUI");
            if (byTag) losePanel = byTag;
            else losePanel = GameObject.Find("LosePanel");
        }
        if (losePanel) losePanel.SetActive(false);
        Time.timeScale = 1f;

        // Animator
        if (anim == null) anim = GetComponent<Animator>();
        if (anim) anim.applyRootMotion = false;
        CacheAnimatorParams();

        // إعداد المصباح
        if (flashlightObject != null && flashlightSocket != null)
        {
            if (flashlightObject.transform.parent != flashlightSocket)
                flashlightObject.transform.SetParent(flashlightSocket, false);

            AlignFlashlightTransform();

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
            if (canControl && Input.GetKeyDown(KeyCode.Space))
                yVel = jumpForce;
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

        if (anim != null)
        {
            float groundSpeed = (canControl ? new Vector2(inputMove.x, inputMove.z).magnitude : 0f);
            float speed01 = Mathf.Clamp01(groundSpeed / Mathf.Max(0.01f, moveSpeed));

            if (hasSpeed)    anim.SetFloat(SpeedHash, speed01);
            if (hasIsMoving) anim.SetBool(IsMovingHash, speed01 > 0.05f);
        }

        HandleFlashlight(canControl);
    }

    // ==================== Damage/Death ====================
    public void TakeDamage(int amount) => TakeDamage(amount, transform.position - transform.forward * 0.1f);

    public void TakeDamage(int amount, Vector3 hitFromWorldPos)
    {
        if (isDead) return;
        if (Time.time - lastHitTime < invulnerableAfterHit) return; // حماية من الضرب المكرر
        lastHitTime = Time.time;

        currentHealth = Mathf.Max(0, currentHealth - amount);

        if (sfx && hitSfx) sfx.PlayOneShot(hitSfx);
        if (anim && hasHit) anim.SetTrigger(HitHash);

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

        if (anim && hasDie) anim.SetTrigger(DieHash);
        if (sfx && deathSfx) sfx.PlayOneShot(deathSfx);

        if (losePanel) losePanel.SetActive(true);
        Time.timeScale = 0f;
        Debug.Log("[Lose] YOU LOST");
    }

    public void RestartScene()
    {
        Time.timeScale = 1f;
        var scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.buildIndex);
    }

    // ==================== Enemy Hit (Player-side) ====================
    void OnTriggerEnter(Collider other)
    {
        if (isDead) return;
        if (!other || !other.gameObject.activeInHierarchy) return;

        if (other.CompareTag(enemyTag))
        {
            TakeDamage(1, other.transform.position);
            Destroy(GetEnemyRoot(other.transform));
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isDead) return;
        if (collision == null || collision.collider == null) return;

        if (collision.collider.CompareTag(enemyTag))
        {
            TakeDamage(1, collision.collider.transform.position);
            Destroy(GetEnemyRoot(collision.collider.transform));
        }
    }

    // يبحث عن الجذر الحقيقي للعدو (لو الكوليدر كان ابن داخل البريفاب)
    private GameObject GetEnemyRoot(Transform t)
    {
        Transform cur = t;
        while (cur.parent != null)
        {
            if (cur.CompareTag(enemyTag)) return cur.gameObject;
            // لو ما فيه تاق، نوقف عند أول كائن عليه سكربت العدو
            if (cur.GetComponent<MonoBehaviour>() && cur.name.ToLower().Contains("enemy"))
                return cur.gameObject;
            cur = cur.parent;
        }
        // آخر محاولة: ارجع لأعلى جذر
        return t.root.gameObject;
    }

    // ==================== Flashlight helpers ====================
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
        flashlightObject.SetActive(enable);
        if (flashlightLight) flashlightLight.enabled = enable;
    }

    private void CacheAnimatorParams()
    {
        hasSpeed = hasIsMoving = hasHit = hasDie = false;
        if (anim == null) return;

        foreach (var p in anim.parameters)
        {
            if (p.type == AnimatorControllerParameterType.Float   && p.nameHash == SpeedHash)    hasSpeed = true;
            if (p.type == AnimatorControllerParameterType.Bool    && p.nameHash == IsMovingHash) hasIsMoving = true;
            if (p.type == AnimatorControllerParameterType.Trigger && p.nameHash == HitHash)      hasHit = true;
            if (p.type == AnimatorControllerParameterType.Trigger && p.nameHash == DieHash)      hasDie = true;
        }

        if (!hasSpeed)
            Debug.LogWarning("[Animator] Missing parameter Float 'Speed' (مهم لانتقالات Idle/Walking).");
    }
}
