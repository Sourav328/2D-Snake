using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Snake : MonoBehaviour
{
    [SerializeField] private Vector2 direction = Vector2.right;
    [SerializeField] private float moveDelay = 0.2f;
    [SerializeField] private GameObject bodySegmentPrefab;
    [SerializeField] private int initialSize = 3;
    [SerializeField] private float powerUpDuration = 3f;

    private List<Transform> bodyParts = new List<Transform>();
    private Vector3 lastHeadPos;
    private float startTime;
    private float powerUpTimer = 0f;

    private bool isShielded = false;
    private bool doubleScore = false;
    private bool speedBoost = false;

    private void Start()
    {
        startTime = Time.time;

        for (int i = 0; i < initialSize; i++)
        {
            Grow();
        }

        InvokeRepeating(nameof(Move), moveDelay, moveDelay);
    }

    private void Update()
    {
        HandleInput();

        if (powerUpTimer > 0f)
        {
            powerUpTimer -= Time.deltaTime;

            if (powerUpTimer <= 0f)
            {
                isShielded = false;
                doubleScore = false;
                speedBoost = false;
                CancelSpeedBoost();
            }
        }
    }

    private void Move()
    {
        lastHeadPos = transform.position;
        Vector3 newPos = transform.position + (Vector3)direction;

        float camHeight = Camera.main.orthographicSize;
        float camWidth = camHeight * Camera.main.aspect;

        newPos.x = Wrap(newPos.x, -camWidth, camWidth);
        newPos.y = Wrap(newPos.y, -camHeight, camHeight);

        transform.position = newPos;

        for (int i = bodyParts.Count - 1; i > 0; i--)
        {
            bodyParts[i].position = bodyParts[i - 1].position;
        }

        if (bodyParts.Count > 0)
            bodyParts[0].position = lastHeadPos;
    }

    private float Wrap(float value, float min, float max)
    {
        float range = max - min;
        return value < min ? value + range : value > max ? value - range : value;
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) && direction != Vector2.down)
            direction = Vector2.up;
        else if (Input.GetKeyDown(KeyCode.DownArrow) && direction != Vector2.up)
            direction = Vector2.down;
        else if (Input.GetKeyDown(KeyCode.LeftArrow) && direction != Vector2.right)
            direction = Vector2.left;
        else if (Input.GetKeyDown(KeyCode.RightArrow) && direction != Vector2.left)
            direction = Vector2.right;
    }

    public void Grow()
    {
        GameObject segment = Instantiate(bodySegmentPrefab);
        Vector3 spawnPos = bodyParts.Count == 0 ? transform.position : bodyParts[bodyParts.Count - 1].position;
        segment.transform.position = spawnPos;
        segment.tag = "Body";
        bodyParts.Add(segment.transform);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (Time.time - startTime < 1f) return;

        if (other.CompareTag("Food"))
        {
            Grow();
            Destroy(other.gameObject);
            FindObjectOfType<GameManager>().SpawnFood();

            int scoreToAdd = doubleScore ? 2 : 1;
            ScoreManager.Instance.AddScore(scoreToAdd);
        }
        else if (other.CompareTag("Body"))
        {
            if (!isShielded)
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else if (other.CompareTag("PowerUp"))
        {
            PowerUp powerUp = other.GetComponent<PowerUp>();
            ActivatePowerUp(powerUp.powerType);
            Destroy(other.gameObject);
        }
    }

    private void ActivatePowerUp(PowerUpType type)
    {
        powerUpTimer = powerUpDuration;

        switch (type)
        {
            case PowerUpType.Shield:
                isShielded = true;
                break;
            case PowerUpType.ScoreBoost:
                doubleScore = true;
                break;
            case PowerUpType.SpeedUp:
                speedBoost = true;
                CancelInvoke(nameof(Move));
                InvokeRepeating(nameof(Move), moveDelay / 2f, moveDelay / 2f);
                break;
            default:
                print("wrong");
                break;
        }
    }

    private void CancelSpeedBoost()
    {
        CancelInvoke(nameof(Move));
        InvokeRepeating(nameof(Move), moveDelay, moveDelay);
    }
}
