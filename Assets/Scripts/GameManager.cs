using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Level settings")]
    [SerializeField] private int requiredItems = 5;
    [SerializeField] private string itemName = "Ключи";
    [SerializeField] private int startingHealth = 3;
    [SerializeField] private float levelDuration = 60f;
    [SerializeField] private string nextLevelScene = "Level2";
    [SerializeField] private bool isFinalLevel = false;
    [SerializeField] private string firstLevelScene = "Level1";
    [SerializeField] private string menuScene = "MainMenu";

    [Header("HUD")]
    [SerializeField] private Text healthText;
    [SerializeField] private Text timerText;
    [SerializeField] private Text itemsText;

    [Header("Panels")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject defeatPanel;

    private int health;
    private int collectedItems;
    private float timeRemaining;
    private bool paused;
    private bool gameEnded;
    private bool exitOpen;

    public bool IsPaused => paused;
    public bool IsExitOpen => exitOpen;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        Time.timeScale = 1f;
        health = startingHealth;
        timeRemaining = levelDuration;
    }

    private void Start()
    {
        SetPanelActive(pausePanel, false);
        SetPanelActive(victoryPanel, false);
        SetPanelActive(defeatPanel, false);
        UpdateHUD();
    }

    private void Update()
    {
        if (gameEnded)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePause();

        if (paused)
            return;

        timeRemaining -= Time.deltaTime;

        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            UpdateHUD();
            LoseGame();
            return;
        }

        UpdateTimer();
    }

    public void CollectItem()
    {
        if (gameEnded || collectedItems >= requiredItems)
            return;

        collectedItems++;

        if (collectedItems >= requiredItems)
            exitOpen = true;

        UpdateHUD();
    }

    public void TakeDamage(int amount)
    {
        if (gameEnded || amount <= 0)
            return;

        health = Mathf.Max(0, health - amount);
        UpdateHUD();

        if (health <= 0)
            LoseGame();
    }

    public void LeaveLevel()
    {
        if (gameEnded || !exitOpen)
            return;

        if (isFinalLevel)
        {
            WinGame();
            return;
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene(nextLevelScene);
    }

    public void TogglePause()
    {
        if (gameEnded)
            return;

        SetPaused(!paused);
    }

    public void ResumeGame()
    {
        if (!gameEnded)
            SetPaused(false);
    }

    private void SetPaused(bool value)
    {
        paused = value;
        Time.timeScale = paused ? 0f : 1f;
        SetPanelActive(pausePanel, paused);
    }

    private void WinGame()
    {
        gameEnded = true;
        paused = false;
        Time.timeScale = 0f;
        SetPanelActive(victoryPanel, true);
    }

    private void LoseGame()
    {
        gameEnded = true;
        paused = false;
        Time.timeScale = 0f;
        SetPanelActive(defeatPanel, true);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(firstLevelScene);
    }

    public void ReturnToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(menuScene);
    }

    private void UpdateHUD()
    {
        if (healthText != null)
            healthText.text = "Здоровье: " + health;

        if (itemsText != null)
            itemsText.text = itemName + ": " + collectedItems + " / " + requiredItems;

        UpdateTimer();
    }

    private void UpdateTimer()
    {
        if (timerText == null)
            return;

        int seconds = Mathf.CeilToInt(timeRemaining);
        int minutesPart = seconds / 60;
        int secondsPart = seconds % 60;
        timerText.text = string.Format("Время: {0:00}:{1:00}", minutesPart, secondsPart);
    }

    private void SetPanelActive(GameObject panel, bool value)
    {
        if (panel != null)
            panel.SetActive(value);
    }
}
