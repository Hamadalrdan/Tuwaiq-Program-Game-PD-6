using UnityEngine;

public class PlayerLose : MonoBehaviour
{
    [SerializeField] private GameObject losePanel;   // اختياري: سيجده تلقائيًا لو فاضي
    [SerializeField] private string enemyTag = "Enemy";
    [SerializeField] private bool pauseOnLose = true;

    void Awake()
    {
        Time.timeScale = 1f;

        if (losePanel == null)
        {
            var byTag = GameObject.FindGameObjectWithTag("LosePanelUI");
            if (byTag != null) losePanel = byTag;
            else losePanel = GameObject.Find("LosePanel");
        }

        if (losePanel != null)
        {
            losePanel.SetActive(false);
            Debug.Log("[Lose] Found panel: " + losePanel.name + " (hidden at start)");
        }
        else
        {
            Debug.LogWarning("[Lose] No LosePanel assigned/found!");
        }
    }

    void ShowLose(string via)
    {
        Debug.Log("[Lose] YOU LOST via: " + via);
        if (losePanel != null) losePanel.SetActive(true);
        if (pauseOnLose) Time.timeScale = 0f;
    }

    // 1) لو العدو Trigger
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(enemyTag)) ShowLose("OnTriggerEnter");
    }

    // 2) لو تصادم فيزيائي عادي (Rigidbodies/Colliders)
    void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag(enemyTag)) ShowLose("OnCollisionEnter");
    }

    // 3) خاص بالـ CharacterController
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider != null && hit.collider.CompareTag(enemyTag))
            ShowLose("OnControllerColliderHit");
    }
}