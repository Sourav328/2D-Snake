using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject[] foodPrefabs;
    [SerializeField] private float minSpawnTime = 3f;
    [SerializeField] private float maxSpawnTime = 8f;

    private void Start()
    {
        SpawnFood(); // Spawn one at the beginning
        StartCoroutine(SpawnFoodRoutine());
    }

    public void SpawnFood()
    {
        Snake snake = FindObjectOfType<Snake>();
        GameObject selectedPrefab;

        while (true)
        {
            selectedPrefab = foodPrefabs[Random.Range(0, foodPrefabs.Length)];
            Food food = selectedPrefab.GetComponent<Food>();

            if (food != null && food.foodType == FoodType.MassBurner && snake.BodySize() <= 3)
                continue;

            break;
        }

        float camHeight = Camera.main.orthographicSize;
        float camWidth = camHeight * Camera.main.aspect;

        float x = Mathf.Round(Random.Range(-camWidth + 1, camWidth - 1));
        float y = Mathf.Round(Random.Range(-camHeight + 1, camHeight - 1));
        Vector3 spawnPos = new Vector3(x, y, 0f);

        Instantiate(selectedPrefab, spawnPos, Quaternion.identity);
    }

    private IEnumerator SpawnFoodRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minSpawnTime, maxSpawnTime));
            SpawnFood();
        }
    }
}