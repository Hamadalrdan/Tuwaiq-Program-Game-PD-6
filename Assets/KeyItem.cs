using UnityEngine;

public class KeyItem : MonoBehaviour
{
    public bool taken = false; // عشان نمنع أخذ المفتاح مرتين

    private void OnTriggerStay(Collider other)
    {
        // إذا دخل اللاعب في نطاق المفتاح
        if (taken) return; // لو تم التقاطه مسبقاً
        if (other.CompareTag("Player"))
        {
            // إذا ضغط Q وهو داخل نطاق المفتاح
            if (Input.GetKeyDown(KeyCode.Q))
            {
                PlayerKeyController playerKey = other.GetComponent<PlayerKeyController>();
                if (playerKey != null)
                {
                    playerKey.hasKey = true;  // اللاعب صار معه المفتاح
                    taken = true;
                    Debug.Log("🔑 Player picked up the key!");
                    Destroy(gameObject);       // نخفي المفتاح من اللعبة
                }
            }
        }
    }
}
