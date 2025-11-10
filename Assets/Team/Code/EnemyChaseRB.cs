using UnityEngine;

public class EnemyChaseRB : MonoBehaviour
{
    public Transform target;
    public float moveSpeed = 3f;
    public float turnSpeed = 720f;

    public float stopDistance = 1.3f;   // كم يقف قبل اللاعب
    public int damage = 1;              // كم ينقص من صحة اللاعب
    public float attackCooldown = 1f;   // ثانية بين الضربات

    float nextAttackTime;

    void Start()
    {
        if (!target)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p) target = p.transform;
        }
    }

    void Update()
    {
        if (!target) return;

        // اتجاه اللاعب
        Vector3 to = target.position - transform.position;
        to.y = 0f;

        // لو قريب مرة → توقف + هجوم
        if (to.magnitude <= stopDistance)
        {
            AttackPlayer();
            return;
        }

        // لو بعيد → تحرك له
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            Quaternion.LookRotation(to),
            turnSpeed * Time.deltaTime
        );

        transform.position += transform.forward * moveSpeed * Time.deltaTime;
    }

    void AttackPlayer()
    {
        // تأكد إن الوقت يسمح بالهجوم
        if (Time.time < nextAttackTime) return;

        // أنقص صحة اللاعب
        target.GetComponent<Player_Movment>()?.TakeDamage(damage);
        Debug.Log("💥 Enemy hit player!");

        // وقت الضربة الجاية
        nextAttackTime = Time.time + attackCooldown;
    }
}