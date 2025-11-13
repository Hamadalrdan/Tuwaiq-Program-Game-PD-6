using UnityEngine;
using System.Collections;

public class BullIntro : MonoBehaviour
{
    [Header("Player")]
    public Transform player;

    [Header("Charge Settings")]
    public float chargeSpeed = 15f;
    public float delayBeforeCharge = 2f;
    public float disappearDistance = 1.8f;

    [Header("FX")]
    public AudioSource roarSound;
    public AudioClip roarClip;
    public ParticleSystem disappearEffect;

    private bool charging = false;

    IEnumerator Start()
    {
        // ننتظر قبل الهجمة
        yield return new WaitForSeconds(delayBeforeCharge);

        // صوت الزئير
        if (roarSound != null)
        {
            if (roarClip != null)
                roarSound.PlayOneShot(roarClip);
            else
                roarSound.Play();
        }

        charging = true;
    }

    void Update()
    {
        if (!charging || player == null)
            return;

        // خله دايم يطالع اللاعب
        transform.LookAt(player);

        // يمشي باتجاه اللاعب
        transform.position = Vector3.MoveTowards(
            transform.position,
            player.position,
            chargeSpeed * Time.deltaTime
        );

        // لما يقرب مسافة معينة يختفي
        float dist = Vector3.Distance(transform.position, player.position);
        if (dist <= disappearDistance)
        {
            // نشغّل دخان الاختفاء
            if (disappearEffect != null)
            {
                disappearEffect.transform.position = transform.position;
                disappearEffect.Play();
            }

            // نخفي كل رسومات الثور + التصادم
            foreach (var r in GetComponentsInChildren<Renderer>())
                r.enabled = false;

            foreach (var c in GetComponentsInChildren<Collider>())
                c.enabled = false;

            // نحذفه بعد ثانية
            Destroy(gameObject, 1f);
        }
    }
}