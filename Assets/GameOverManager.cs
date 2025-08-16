using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance;

    [Header("UI")]
    public GameObject gameOverPanel;
    public Text placeText;
    public Text scoreText;
    public Text totalScoreText;
    public Button restartButton;
    public Button mainMenuButton;
    public Text TotalLivePlayersText;

    [Header("Score")]
    public int totalScore = 0;

    private int totalEnemies = 4;
    private int placeCounter = 5;
    private bool playerDead = false;
    private bool gameEnded = false;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        gameOverPanel.SetActive(false);
    }

    public void Initialize(int enemyCount)
    {
        totalEnemies = enemyCount;
        placeCounter = 5;
        playerDead = false;
        gameEnded = false;
        gameOverPanel.SetActive(false);
        SwowLivePlayers();
    }

    private void Update()
    {
        
    }

    public void OnPlayerDied(string deadTag)
    {
        if (gameEnded) return;

        Debug.Log("Уничтожен объект с тегом: " + deadTag);

        if (deadTag == "Player")
        {
            playerDead = true;
            ShowGameOver(placeCounter); // игрок умер
            placeCounter--;
            SwowLivePlayers();
        }
        else if (deadTag == "Enemy")
        {
            totalEnemies--;
            Debug.Log("Враг уничтожен. Осталось врагов: " + totalEnemies);

            placeCounter--;
            SwowLivePlayers();

            if (!playerDead && totalEnemies == 0)
            {
                // Победа — игрок остался один
                ShowGameOver(1);
            }
        }
       

    }

    public void SwowLivePlayers()
    {
        TotalLivePlayersText.text = $"{placeCounter}";

    }

    private void ShowGameOver(int place)
    {
        gameEnded = true;
        gameOverPanel.SetActive(true);

        placeText.text = $"{place} место";
        placeText.color = GetColorByPlace(place);

        int earnedScore = GetScoreByPlace(place);
        scoreText.text = $"+{earnedScore} очков";

        totalScore += earnedScore;
        totalScoreText.text = $"Общие очки: {totalScore}";

        PlayerPrefs.SetInt("TotalScore", totalScore);

        restartButton.onClick.RemoveAllListeners();
        restartButton.onClick.AddListener(() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex));

        mainMenuButton.onClick.RemoveAllListeners();
        mainMenuButton.onClick.AddListener(() => SceneManager.LoadScene("Meny"));
    }

    private Color GetColorByPlace(int place)
    {
        switch (place)
        {
            case 1: return Color.yellow;
            case 2: return Color.gray;
            case 3: return new Color(1f, 0.5f, 0f); // оранжевый
            default: return Color.white;
        }
    }

    private int GetScoreByPlace(int place)
    {
        switch (place)
        {
            case 1: return 20;
            case 2: return 10;
            case 3: return 5;
            default: return 0;
        }
    }

    private void Start()
    {
        totalScore = PlayerPrefs.GetInt("TotalScore", 0);
    }
}
