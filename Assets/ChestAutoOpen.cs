using UnityEngine;

public class ChestAutoOpen : MonoBehaviour
{
    public Animator chestAnimator; 
    public GameObject snake;
    bool opened = false;

    private void OnTriggerEnter(Collider other)
    {
        if (opened) return;
        if (!other.CompareTag("Player")) return;

        opened = true;

        // يشغل أنيميشن فتح الصندوق
        if (chestAnimator != null)
            chestAnimator.SetTrigger("Open");

        // يطلع الثعبان
        if (snake != null)
            snake.SetActive(true);
    }
}