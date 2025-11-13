using UnityEngine;

public class ZombieSound : MonoBehaviour
{
    AudioSource audioS;

    void Start()
    {
        audioS = GetComponent<AudioSource>();
        audioS.Play(); // يشغل الصوت أول ما يظهر الزومبي
    }
}