using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
    public static GameOverController Instance;

    [Header("Game Over UI")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI finalScoreText;
    public Button mainMenuButton;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(ReturnToMainMenu);
    }

    public void ShowGameOverScreen(float finalScore)
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        if (finalScoreText != null)
            finalScoreText.text = "Final Score: " + Mathf.FloorToInt(finalScore).ToString();

        Time.timeScale = 0f; 
    }

    void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
