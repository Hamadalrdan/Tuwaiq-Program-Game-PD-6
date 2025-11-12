using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
public class PlayerAnimation : MonoBehaviour
{
    [Header("Refs")]
    public Animator anim;
    public CharacterController cc;

    [Header("Tuning")]
    public float moveSpeedMax = 10f;     // نفس Move Speed في سكربت الحركة (حطها نفس القيمة)
    public float dampTime = 0.1f;        // تنعيم الأنيميتر
    public string speedParam = "Speed";  // اسم باراميتر السرعة

    void Awake()
    {
        if (!anim) anim = GetComponent<Animator>();
        if (!cc)   cc   = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (!anim || !cc) return;

        // 1) سرعة من CharacterController (أفقي فقط)
        Vector3 v = cc.velocity; 
        float velSpeed = new Vector2(v.x, v.z).magnitude;

        // 2) سرعة احتياط من الإدخال (لو الحركة تتم بطرق ثانية)
        float h = (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))  ? -1f :
            (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) ?  1f : 0f;
        float z = (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))  ? -1f :
            (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))    ?  1f : 0f;
        float inputSpeed = new Vector2(h, z).magnitude * moveSpeedMax;

        // نأخذ الأكبر (الأضمن)
        float rawSpeed = Mathf.Max(velSpeed, inputSpeed);

        // نطبّعها إلى 0..1 حسب السرعة القصوى
        float speed01 = Mathf.Clamp01(rawSpeed / Mathf.Max(0.01f, moveSpeedMax));

        // نرسل للأنيميتر بتنعيم
        anim.SetFloat(speedParam, speed01, dampTime, Time.deltaTime);
    }
}