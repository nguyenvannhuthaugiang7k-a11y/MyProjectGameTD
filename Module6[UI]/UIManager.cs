using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UI Panels (CanvasGroup)")]
    [SerializeField] private CanvasGroup mainMenuPanel;
    [SerializeField] private CanvasGroup gameplayPanel;
    [SerializeField] private CanvasGroup victoryPanel;
    [SerializeField] private CanvasGroup defeatPanel;

    private void Awake()
    {
        if (Instance == null) 
        {
            Instance = this;
        }
        else 
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        GameStateManager.OnGameStateChanged += HandleGameStateChanged;
        GameManager.OnGameOver += ShowDefeatPanel;
        GameManager.OnGameWin += ShowVictoryPanel;
    }

    private void OnDisable()
    {
        GameStateManager.OnGameStateChanged -= HandleGameStateChanged;
        GameManager.OnGameOver -= ShowDefeatPanel;
        GameManager.OnGameWin -= ShowVictoryPanel;
    }

    private void Start()
    {
        Time.timeScale = 1f;

        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == "MainMenu")
        {
            ShowMainMenuUI();
        }
        else
        {
            ShowGameplayUI();
        }
    }

    // --- HIỂN THỊ PANEL ---

    public void ShowMainMenuUI()
    {
        HideAllPanels();
        ShowPanel(mainMenuPanel);
    }

    public void ShowGameplayUI()
    {
        HideAllPanels();
        ShowPanel(gameplayPanel);
    }

    private void HandleGameStateChanged(GameState state)
    {
        switch (state)
        {
            case GameState.MainMenu:
                ShowMainMenuUI();
                break;
            case GameState.Gameplay:
                ShowGameplayUI();
                break;
            case GameState.GameWin:
                ShowVictoryPanel();
                break;
            case GameState.GameOver:
                ShowDefeatPanel();
                break;
        }
    }

    public void ShowDefeatPanel()
    {
        Debug.Log("[UIManager] Kích hoạt DEFEAT PANEL!");

        HideAllPanels();

        if (defeatPanel != null)
        {
            ShowPanel(defeatPanel);
            defeatPanel.transform.SetAsLastSibling();
        }

        // Tạm dừng thời gian SAU KHI đã kích hoạt UI thành công
        Time.timeScale = 0f;
    }

    public void ShowVictoryPanel()
    {
        Debug.Log("[UIManager] Kích hoạt VICTORY PANEL!");

        HideAllPanels();

        if (victoryPanel != null)
        {
            ShowPanel(victoryPanel);
            victoryPanel.transform.SetAsLastSibling();
        }

        Time.timeScale = 0f;
    }

    private void HideAllPanels()
    {
        SetPanelState(mainMenuPanel, false);
        SetPanelState(gameplayPanel, false);
        SetPanelState(victoryPanel, false);
        SetPanelState(defeatPanel, false);
    }

    private void ShowPanel(CanvasGroup group)
    {
        SetPanelState(group, true);
    }

    private void SetPanelState(CanvasGroup group, bool isVisible)
    {
        if (group == null) return;

        // Bật GameObject để đảm bảo Button hoạt động
        group.gameObject.SetActive(isVisible);

        group.alpha = isVisible ? 1f : 0f;
        group.blocksRaycasts = isVisible;
        group.interactable = true; // Luôn giữ interactable = true
    }

    // --- BUTTON EVENTS ---

    public void OnClick_StartGame()
    {
        Time.timeScale = 1f;
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.LoadNextLevel();
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    public void OnClick_PlayAgain()
    {
        Debug.Log("[UIManager] Đã bấm Play Again!");
        Time.timeScale = 1f;

        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.RestartCurrentLevel();
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void OnClick_Home()
    {
        Debug.Log("[UIManager] Đã bấm Home!");
        Time.timeScale = 1f;

        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.LoadMainMenu();
        }
        else
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}