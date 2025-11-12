using System.Collections;
using UnityEngine;

public class EnemySpawner1 : MonoBehaviour
{
    public GameObject enemyPrefab;     // اسحب prefab العدو هنا
    public int maxEnemies = 6;
    public string enemyTag = "Enemy";
    public float firstDelay = 1.5f;
    public float every = 2f;

    void Start() => StartCoroutine(Loop());

    IEnumerator Loop()
    {
        yield return new WaitForSeconds(firstDelay);

        while (true)
        {
            int alive = GameObject.FindGameObjectsWithTag(enemyTag).Length;
            if (alive < maxEnemies)
            {
                Vector3 p = transform.position;
                p.x += Random.Range(-5f, 5f);
                Instantiate(enemyPrefab, p, transform.rotation);
            }

            yield return new WaitForSeconds(every);
        }
    }
}