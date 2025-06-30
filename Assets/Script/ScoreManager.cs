using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    private int score1 = 0;
    private int score2 = 0;

    [SerializeField] private TextMeshProUGUI scoreText1;
    [SerializeField] private TextMeshProUGUI scoreText2;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        UpdateScoreText();
    }

    public void AddScore(int amount, int playerNumber = 1)
    {
        if (playerNumber == 1)
        {
            if (FindObjectOfType<Snake>().HasScoreBoost())
                score1 += amount * 2;
            else
                score1 += amount;
        }
        else if (playerNumber == 2)
        {
            if (FindObjectOfType<Snake_2>().HasScoreBoost())
                score2 += amount * 2;
            else
                score2 += amount;
        }

        UpdateScoreText();
    }

    public int GetScore(int playerNumber = 1)
    {
        return playerNumber == 1 ? score1 : score2;
    }

    private void UpdateScoreText()
    {
        if (scoreText1 != null)
            scoreText1.text = "Score: " + score1.ToString();

        if (scoreText2 != null)
            scoreText2.text = "Score: " + score2.ToString();
    }
}
