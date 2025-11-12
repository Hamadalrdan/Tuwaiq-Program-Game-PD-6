using UnityEngine;

namespace DooorScript
{
    [RequireComponent(typeof(AudioSource))]
    public class Door : MonoBehaviour
    {
        public bool open = false;
        public float smooth = 2.0f;
        public float openAngle = 90.0f; // زاوية الفتح
        private float closeAngle = 0.0f;

        private AudioSource asource;
        public AudioClip openDoor, closeDoor;

        public float interactDistance = 5f; // المسافة التي يمكن التفاعل فيها
        private Transform player; // مرجع اللاعب
        private PlayerKeyController playerKeyController; // مرجع سكربت المفتاح عند اللاعب

        void Start()
        {
            asource = GetComponent<AudioSource>();

            // نحصل على اللاعب وسكربت المفتاح الخاص به
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
                playerKeyController = playerObj.GetComponent<PlayerKeyController>();
            }
        }

        void Update()
        {
            // حركة الباب (دوران سلس)
            float targetAngle = open ? openAngle : closeAngle;
            Quaternion targetRotation = Quaternion.Euler(0, targetAngle, 0);

            transform.localRotation = Quaternion.Slerp(
                transform.localRotation,
                targetRotation,
                Time.deltaTime * smooth
            );

            // إذا اللاعب قريب وضغط E
            if (player != null && Vector3.Distance(player.position, transform.position) <= interactDistance)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    // يتحقق إذا كان اللاعب معه المفتاح
                    if (playerKeyController != null && playerKeyController.hasKey)
                    {
                        OpenDoor();
                    }
                    else
                    {
                        Debug.Log("🚪 الباب مقفل! تحتاج إلى مفتاح لفتحه.");
                    }
                }
            }
        }

        public void OpenDoor()
        {
            open = !open;
            if (asource != null)
            {
                asource.clip = open ? openDoor : closeDoor;
                asource.Play();
            }
        }
    }
}
