using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    public GameState CurrentState { get; private set; }

    public static event Action<GameState> OnGameStateChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Giữ Manager tồn tại xuyên suốt giữa các Scene
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Tự động nhận diện Trạng thái theo tên Scene đang chạy
        string activeSceneName = SceneManager.GetActiveScene().name;
        if (activeSceneName == "MainMenu")
        {
            SetState(GameState.MainMenu);
        }
        else
        {
            SetState(GameState.Gameplay);
        }
    }

    public void SetState(GameState newState)
    {
        CurrentState = newState;

        switch (newState)
        {
            case GameState.MainMenu:
            case GameState.Gameplay:
                Time.timeScale = 1f;
                break;
            case GameState.GameWin:
            case GameState.GameOver:
                Time.timeScale = 0f; // Dừng game khi Thắng/Thua
                break;
        }

        OnGameStateChanged?.Invoke(newState);
    }

    // --- CÁC HÀM TẢI SCENE ---

    // Chơi lại Level hiện tại
    public void RestartCurrentLevel()
    {
        Time.timeScale = 1f;
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }

    // Sang Level tiếp theo
    public void LoadNextLevel()
    {
        Time.timeScale = 1f;
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            LoadMainMenu(); // Hết level thì quay về Main Menu
        }
    }

    // Quay về Scene MainMenu
    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}