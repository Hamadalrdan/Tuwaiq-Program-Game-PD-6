using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyChase : MonoBehaviour
{
    public float speed = 3f;
    public float stopDistance = 1.5f;
    public string playerTag = "Player";
    public int damage = 1;

    // 🔊 الصوت
    public AudioSource zombieSound;
    public float soundDistance = 8f; 
    public float soundDelay = 2f;    
    private float nextSoundTime = 0f;

    private Rigidbody rb;
    private Transform target;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag(playerTag);
        if (p != null) target = p.transform;
    }

    void FixedUpdate()
    {
        if (target == null) 
        { 
            rb.linearVelocity = Vector3.zero; 
            return; 
        }

        Vector3 toPlayer = target.position - transform.position;
        toPlayer.y = 0f;

        // 🔊 يشغل الصوت إذا الزومبي قريب
        float dist = toPlayer.magnitude;
        if (dist <= soundDistance && Time.time >= nextSoundTime)
        {
            if (zombieSound != null && !zombieSound.isPlaying)
            {
                zombieSound.Play();
            }
            nextSoundTime = Time.time + soundDelay;
        }

        if (toPlayer.sqrMagnitude <= stopDistance * stopDistance)
        {
            rb.linearVelocity = Vector3.zero;
            return;
        }

        transform.rotation = Quaternion.LookRotation(toPlayer);
        rb.linearVelocity = toPlayer.normalized * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            var player = other.GetComponent<Player_Movment>();
            if (player != null)
            {
                player.TakeDamage(damage, transform.position);
            }

            Destroy(gameObject);
        }
    }
}
