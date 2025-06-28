using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject foodPrefab;
    [SerializeField] private GameObject[] powerUpPrefabs;
    [SerializeField] private float minSpawnTime = 5f;
    [SerializeField] private float maxSpawnTime = 15f;

    private void Start()
    {
        SpawnFood();
        StartCoroutine(SpawnPowerUps());
    }

    public void SpawnFood()
    {
        float camHeight = Camera.main.orthographicSize;
        float camWidth = camHeight * Camera.main.aspect;

        float x = Mathf.Round(Random.Range(-camWidth + 1, camWidth - 1));
        float y = Mathf.Round(Random.Range(-camHeight + 1, camHeight - 1));
        Vector3 spawnPosition = new Vector3(x, y, 0f);

        Instantiate(foodPrefab, spawnPosition, Quaternion.identity);
    }

    private IEnumerator SpawnPowerUps()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minSpawnTime, maxSpawnTime));

            float camHeight = Camera.main.orthographicSize;
            float camWidth = camHeight * Camera.main.aspect;

            float x = Mathf.Round(Random.Range(-camWidth + 1, camWidth - 1));
            float y = Mathf.Round(Random.Range(-camHeight + 1, camHeight - 1));
            Vector3 spawnPosition = new Vector3(x, y, 0f);

            int index = Random.Range(0, powerUpPrefabs.Length);
            Instantiate(powerUpPrefabs[index], spawnPosition, Quaternion.identity);
        }
    }
}
