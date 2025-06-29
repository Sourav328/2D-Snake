using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Snake : MonoBehaviour
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
    [SerializeField] private SpriteRenderer headRenderer; // Assign Head sprite here

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
                if (headRenderer != null) headRenderer.enabled = true; // Ensure visible after blinking
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

    private void Shrink()
    {
        if (bodyParts.Count > 0)
        {
            Destroy(bodyParts[bodyParts.Count - 1].gameObject);
            bodyParts.RemoveAt(bodyParts.Count - 1);
        }
    }

    public int BodySize()
    {
        return bodyParts.Count;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (Time.time - startTime < 1f) return;

        string tag = other.tag;

        if (tag == "Food" || tag == "MassGainer" || tag == "MassBurner")
        {
            Food food = other.GetComponent<Food>();
            if (food != null)
            {
                if (tag == "Food")
                {
                    Grow();
                    ScoreManager.Instance.AddScore(1);
                    Destroy(other.gameObject);
                    FindObjectOfType<GameManager>().SpawnFood(); // Respawn only normal food
                }
                else if ((tag == "MassGainer" || tag == "Mass Gainer") && ScoreManager.Instance.GetScore() > 10)
                {
                    Grow();
                    Grow();
                    Destroy(other.gameObject);
                }
                else if ((tag == "MassBurner" || tag == "Mass Burner") && ScoreManager.Instance.GetScore() > 10)
                {
                    Shrink();
                    Shrink();
                    Destroy(other.gameObject);
                }
                else
                {
                    Destroy(other.gameObject); // Invalid tag or not enough score
                }
            }
        }
        else if (tag == "Body")
        {
            if (!isShielded)
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else if (tag == "ScoreBoost" || tag == "SpeedBoost" || tag == "Shield")
        {
            PowerUp power = other.GetComponent<PowerUp>();
            if (power != null)
            {
                ActivatePowerUp(power.powerType);
                Destroy(other.gameObject);
            }
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
    public bool HasScoreBoost()
    {
        return scoreBoost;
    }
}