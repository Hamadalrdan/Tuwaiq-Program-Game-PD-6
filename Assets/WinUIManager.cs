using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // ãåãÉ ÚÔÇä TextMeshProUGUI

public class WinUIManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject winPanel;         // äÓÍÈ Úáíå WinPanel
    public TextMeshProUGUI winText;     // äÓÍÈ Úáíå WinText (ÇÎÊíÇÑí)

    void Start()
    {
        // äÎİí ÔÇÔÉ ÇáİæÒ İí ÇáÈÏÇíÉ
        if (winPanel != null)
            winPanel.SetActive(false);

        // äÊÃßÏ Åä ÇáæŞÊ ÔÛÇá ØÈíÚí
        Time.timeScale = 1f;
    }

    // ÏÇáÉ äÓÊÎÏãåÇ ÚäÏ ÇáİæÒ
    public void ShowWinScreen(string message = "YOU WIN!")
    {
        if (winPanel != null)
            winPanel.SetActive(true);

        if (winText != null)
            winText.text = message;

        // äæŞİ ÇááÚÈ ÚÔÇä íÍÓ ÇááÇÚÈ ÃäåÇ äåÇíÉ ÇáãÑÍáÉ
        Time.timeScale = 0f;
    }

    // ÏÇáÉ Restart ááÒÑ
    public void RestartLevel()
    {
        Time.timeScale = 1f;
        Scene current = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current.buildIndex);
    }

    // (ÇÎÊíÇÑí) áæ ÚäÏß ãÔåÏ ÑÆíÓí
    public void GoToMainMenu(string sceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }
}
