using DoorScript;
using UnityEngine;

public class PlayerKeyController : MonoBehaviour
{
    public bool hasKey = false;           // هل اللاعب معه المفتاح؟
    public float interactDistance = 3f;   // مدى التفاعل

    void Update()
    {
        // الضغط على E لفتح الباب
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryOpenDoor();
        }
    }

    void TryOpenDoor()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
        {
            if (hit.collider.CompareTag("Door"))
            {
                Door door = hit.collider.GetComponent<Door>();
                if (door != null)
                {
                    if (hasKey)
                    {
                        door.OpenDoor();
                        Debug.Log("🚪 Door opened successfully!");
                    }
                    else
                    {
                        Debug.Log("❌ You need a key to open this door!");
                    }
                }
            }
        }
    }
}
