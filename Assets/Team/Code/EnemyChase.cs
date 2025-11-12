using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyChase : MonoBehaviour
{
    public float speed = 3f;
    public float stopDistance = 1.5f;
    public string playerTag = "Player";
    public int damage = 1; // كم ينقص من اللاعب

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

        if (toPlayer.sqrMagnitude <= stopDistance * stopDistance)
        {
            rb.linearVelocity = Vector3.zero;
            return;
        }

        transform.rotation = Quaternion.LookRotation(toPlayer);
        rb.linearVelocity = toPlayer.normalized * speed;
    }

    // ✅ يضرب اللاعب وينقص قلبه ويختفي
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            var player = other.GetComponent<Player_Movment>();
            if (player != null)
            {
                player.TakeDamage(damage, transform.position);
            }

            Destroy(gameObject); // حذف العدو بعد الضربة
        }
    }
}