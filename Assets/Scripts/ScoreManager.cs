using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public TextMeshProUGUI scoreText;
    public float score = 0f;
    public bool isGameOver = false;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Update()
    {
        if (!isGameOver)
        {
            score += Time.deltaTime;
            UpdateScoreUI();
        }
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + Mathf.FloorToInt(score).ToString();
    }

    public void GameOver()
    {
        isGameOver = true;
        GameOverController.Instance.ShowGameOverScreen(score);
    }
}
