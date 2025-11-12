using UnityEngine;

namespace DoorScript
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

        void Start()
        {
            asource = GetComponent<AudioSource>();
            player = GameObject.FindGameObjectWithTag("Player").transform; // نبحث عن اللاعب عبر التاغ
        }

        void Update()
        {
            float targetAngle = open ? openAngle : closeAngle;
            Quaternion targetRotation = Quaternion.Euler(0, targetAngle, 0);

            // دوران الباب بسلاسة
            transform.localRotation = Quaternion.Slerp(
                transform.localRotation,
                targetRotation,
                Time.deltaTime * smooth
            );

            // 👇 إذا اللاعب قريب وضغط E → يفتح الباب
            if (Vector3.Distance(player.position, transform.position) <= interactDistance)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    OpenDoor();
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
