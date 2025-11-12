using UnityEngine;

public class ChestAutoOpen : MonoBehaviour
{
    public GameObject chestClosed;   // اسحب chest_close
    public GameObject chestOpen;     // اسحب chest_open (غير مفعّل)
    bool opened = false;

    private void OnTriggerEnter(Collider other)
    {
        if (opened) return;
        if (!other.CompareTag("Player")) return;

        opened = true;
        if (chestClosed) chestClosed.SetActive(false);
        if (chestOpen)   chestOpen.SetActive(true);
    }
}