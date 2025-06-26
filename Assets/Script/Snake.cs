using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Snake : MonoBehaviour
{
   
    public Vector2 direction = Vector2.right;
    public float moveDelay = 0.2f;
    public GameObject bodySegmentPrefab;
    public int initialSize = 3;

    
    private List<Transform> bodyParts = new List<Transform>();
    private Vector3 lastHeadPos;
    private float startTime;

    private void Start()
    {
        startTime = Time.time;

        // Spawn initial body segments
        for (int i = 0; i < initialSize; i++)
        {
            Grow();
        }

        // Start repeating movement
        InvokeRepeating(nameof(Move), moveDelay, moveDelay);
    }

    private void Update()
    {
        HandleInput();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Skip collision check during the first second (to avoid spawning collision)
        if (Time.time - startTime < 1f) return;

        if (other.CompareTag("Food"))
        {
            Grow();
            Destroy(other.gameObject);
            FindObjectOfType<GameManager>().SpawnFood();
        }
        else if (other.CompareTag("Body"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

 
    private void Move()
    {
        lastHeadPos = transform.position;

        Vector3 newPos = transform.position + (Vector3)direction;

        // Calculate screen bounds based on camera
        float camHeight = Camera.main.orthographicSize;
        float camWidth = camHeight * Camera.main.aspect;

        // Apply screen wrapping
        newPos.x = Wrap(newPos.x, -camWidth, camWidth);
        newPos.y = Wrap(newPos.y, -camHeight, camHeight);

        transform.position = newPos;

        // Move body segments to follow the head
        for (int i = bodyParts.Count - 1; i > 0; i--)
        {
            bodyParts[i].position = bodyParts[i - 1].position;
        }

        if (bodyParts.Count > 0)
            bodyParts[0].position = lastHeadPos;
    }

   
    public void Grow()
    {
        GameObject segment = Instantiate(bodySegmentPrefab);
        Vector3 spawnPos = bodyParts.Count == 0 ? transform.position : bodyParts[bodyParts.Count - 1].position;
        segment.transform.position = spawnPos;
        segment.tag = "Body";
        bodyParts.Add(segment.transform);
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

  
    private float Wrap(float value, float min, float max)
    {
        float range = max - min;
        return value < min ? value + range : value > max ? value - range : value;
    }
}
