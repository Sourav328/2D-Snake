using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject foodPrefab;

    public void SpawnFood()
    {
        float camHeight = Camera.main.orthographicSize;
        float camWidth = camHeight * Camera.main.aspect;

        float x = Mathf.Round(Random.Range(-camWidth + 1, camWidth - 1));
        float y = Mathf.Round(Random.Range(-camHeight + 1, camHeight - 1));
        Vector3 spawnPosition = new Vector3(x, y, 0f);

        Instantiate(foodPrefab, spawnPosition, Quaternion.identity);
    }

    private void Start()
    {
        SpawnFood();
    }
}
