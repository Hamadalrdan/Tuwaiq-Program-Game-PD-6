using UnityEngine;
using UnityEngine.SceneManagement;   // مهم لإعادة تشغيل اللعبة
using UnityEngine.UI;                // لو بتستخدم Button

public class PlayerLose : MonoBehaviour
{
    [SerializeField] private GameObject losePanel;   // panel الخسارة
    [SerializeField] private GameObject backgroundPanel; // الخلفية السوداء
    [SerializeField] private Button restartButton;   // زر إعادة التشغيل

    [SerializeField] private string enemyTag = "Enemy";
    [SerializeField] private bool pauseOnLose = true;

    void Awake()
    {
        Time.timeScale = 1f;

        // ========== البحث التلقائي عن اللوحات ==========
        if (losePanel == null)
        {
            var byTag = GameObject.FindGameObjectWithTag("LosePanelUI");
            if (byTag != null) losePanel = byTag;
            else losePanel = GameObject.Find("LosePanel");
        }

        if (backgroundPanel == null)
            backgroundPanel = GameObject.Find("LoseBackground");

        if (restartButton == null)
        {
            var btn = GameObject.Find("RestartButton");
            if (btn != null) restartButton = btn.GetComponent<Button>();
        }

        // ========== إخفاء الأشياء عند البداية ==========
        if (losePanel != null) losePanel.SetActive(false);
        if (backgroundPanel != null) backgroundPanel.SetActive(false);

        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);
    }

    void ShowLose(string via)
    {
        Debug.Log("[Lose] YOU LOST via: " + via);

        if (backgroundPanel != null) backgroundPanel.SetActive(true);
        if (losePanel != null) losePanel.SetActive(true);

        if (pauseOnLose) Time.timeScale = 0f;
    }

    // =================== الاصطدام ===================
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(enemyTag)) ShowLose("Trigger");
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag(enemyTag)) ShowLose("Collision");
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider != null && hit.collider.CompareTag(enemyTag))
            ShowLose("ControllerHit");
    }

    // =================== restart function ===================
    public  void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // يعيد نفس المشهد
    }
}
