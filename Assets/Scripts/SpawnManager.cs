using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    GameObject enemyPrefab;

    [SerializeField]
    GameObject enemyContainer;

    [SerializeField]
    GameObject[] powerUpPrefabs;

    bool shouldSpawn = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void StopSpawning()
    {
        shouldSpawn = false;
    }

    public void StartSpawning()
    {
        shouldSpawn = true;
        StartCoroutine(EnemySpawner());
        StartCoroutine(PowerUpSpawner());
    }

    IEnumerator EnemySpawner()
    {
        yield return new WaitForSeconds(3f);
        while(shouldSpawn)
        {
            GameObject enemy = Instantiate(enemyPrefab);
            enemy.transform.parent = enemyContainer.transform;
            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator PowerUpSpawner()
    {
        yield return new WaitForSeconds(3f);

        while (shouldSpawn)
        {
            GameObject toSpawn = powerUpPrefabs[Random.Range(0, powerUpPrefabs.Length)];
            float randomX = Random.Range(-8f, 8f);
            float randomTime = Random.Range(3f, 7f);
            Instantiate(toSpawn, new Vector3(randomX, 8f, 0), Quaternion.identity);
            yield return new WaitForSeconds(randomTime);
        }
    }
}
