// ==== SnakePlayer2.cs ====
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Snake_2 : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private Vector2 direction = Vector2.right;
    [SerializeField] private float moveDelay = 0.2f;
    [SerializeField] private GameObject bodySegmentPrefab;
    [SerializeField] private int initialSize = 3;

    [Header("Power-Up Settings")]
    [SerializeField] private float powerUpDuration = 3f;
    [SerializeField] private float speedBoostMultiplier = 2f;
    [SerializeField] private float shieldBlinkSpeed = 5f;

    [Header("Visual")]
    [SerializeField] private SpriteRenderer headRenderer;

    private List<Transform> bodyParts = new List<Transform>();
    private Vector3 lastHeadPos;
    private float startTime;
    private float powerUpTimer = 0f;

    private bool isShielded = false;
    private bool scoreBoost = false;
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
                scoreBoost = false;
                speedBoost = false;
                CancelSpeedBoost();
                StopCoroutine(BlinkShield());
                if (headRenderer != null) headRenderer.enabled = true;
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
        if (Input.GetKeyDown(KeyCode.W) && direction != Vector2.down)
            direction = Vector2.up;
        else if (Input.GetKeyDown(KeyCode.S) && direction != Vector2.up)
            direction = Vector2.down;
        else if (Input.GetKeyDown(KeyCode.A) && direction != Vector2.right)
            direction = Vector2.left;
        else if (Input.GetKeyDown(KeyCode.D) && direction != Vector2.left)
            direction = Vector2.right;
    }

    public void Grow(int amount = 1)
    {
        for (int i = 0; i < amount; i++)
        {
            GameObject segment = Instantiate(bodySegmentPrefab);
            Vector3 spawnPos = bodyParts.Count == 0 ? transform.position : bodyParts[bodyParts.Count - 1].position;
            segment.transform.position = spawnPos;
            segment.tag = "Body";
            bodyParts.Add(segment.transform);
        }
    }

    private void Shrink(int amount = 1)
    {
        for (int i = 0; i < amount; i++)
        {
            if (bodyParts.Count > 0)
            {
                Destroy(bodyParts[bodyParts.Count - 1].gameObject);
                bodyParts.RemoveAt(bodyParts.Count - 1);
            }
        }
    }

    public int BodySize()
    {
        return bodyParts.Count;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (Time.time - startTime < 1f) return;

        if (other.TryGetComponent(out Food food))
        {
            switch (food.foodType)
            {
                case FoodType.Food:
                    Grow();
                    ScoreManager.Instance.AddScore(1, 2); // Player 2
                    FindObjectOfType<GameManager>().SpawnFood();
                    break;
                case FoodType.MassGainer:
                    Grow(3);
                    ScoreManager.Instance.AddScore(1, 2);
                    break;
                case FoodType.MassBurner:
                    if (BodySize() > 3)
                        Shrink(2);
                    break;
            }
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("Body") && !isShielded)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else if (other.TryGetComponent(out PowerUp power))
        {
            ActivatePowerUp(power.powerType);
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
                StartCoroutine(BlinkShield());
                break;
            case PowerUpType.ScoreBoost:
                scoreBoost = true;
                break;
            case PowerUpType.SpeedBoost:
                speedBoost = true;
                CancelInvoke(nameof(Move));
                InvokeRepeating(nameof(Move), moveDelay / speedBoostMultiplier, moveDelay / speedBoostMultiplier);
                break;
        }
    }

    private void CancelSpeedBoost()
    {
        CancelInvoke(nameof(Move));
        InvokeRepeating(nameof(Move), moveDelay, moveDelay);
    }

    private IEnumerator BlinkShield()
    {
        float elapsed = 0f;
        while (elapsed < powerUpDuration)
        {
            if (headRenderer != null)
                headRenderer.enabled = !headRenderer.enabled;

            yield return new WaitForSeconds(1f / shieldBlinkSpeed);
            elapsed += 1f / shieldBlinkSpeed;
        }
        if (headRenderer != null)
            headRenderer.enabled = true;
    }

    public bool HasScoreBoost() => scoreBoost;
}
