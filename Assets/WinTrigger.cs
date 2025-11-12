using UnityEngine;

public class WinTrigger : MonoBehaviour
{
    public WinUIManager winUIManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Â‰« Ì” œ⁄Ì Ê«ÃÂ… «·›Ê“
            winUIManager.ShowWinScreen("YOU WIN");
        }
    }
}
