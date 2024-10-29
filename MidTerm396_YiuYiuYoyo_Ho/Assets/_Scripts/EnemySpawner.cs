using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private int enemySize;
    [SerializeField] private float spawnInterval = 10.0f;
    void Start()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, -enemySize);
        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        while (true)
        {
            for (int i = 0; i < enemySize; i++)
            {
                SpawnEnemy(i);
                yield return new WaitForSeconds(0.5f);
            }
            Vector3 newPosition = transform.position;
            newPosition.x += 2;
            transform.position = newPosition;
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnEnemy(int i)
    {
        Vector3 instantiatePosiiton = new Vector3(transform.position.x, transform.position.y, transform.position.z + i * 2);
        Instantiate(enemyPrefab, instantiatePosiiton, transform.rotation);
    }
}
