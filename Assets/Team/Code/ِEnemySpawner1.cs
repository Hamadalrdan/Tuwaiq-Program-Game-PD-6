using JetBrains.Annotations;
using System.Collections;
using UnityEngine;

public class EnemySpawner1 : MonoBehaviour
{
    public GameObject EnemyPrefap;

    void Start()
    {
        StartCoroutine(SpawnEnemy());
    }

    public IEnumerator SpawnEnemy()
    {
        yield return new WaitForSeconds(.8f);

        // 👇 نأخذ موقع السباونر
        Vector3 spawnPos = transform.position;

        // 👈 نخلي الـ X عشوائي بين -3 و +3
        spawnPos.x += Random.Range(-5f, 5f);

        // 👈 لو تبي العشوائية على Z بدل X، استخدم:
        // spawnPos.z += Random.Range(-3f, 3f);

        Instantiate(EnemyPrefap, spawnPos, EnemyPrefap.transform.rotation, null);

        StartCoroutine(SpawnEnemy());
    }
}
