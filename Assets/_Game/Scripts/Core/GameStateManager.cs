using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    public GameState CurrentState { get; private set; }

    public static event Action<GameState> OnGameStateChanged;

    private bool isPaused = false;

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

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {   
        Debug.Log(
            $"[GameStateManager] Scene Loaded: {scene.name}"
        );

        if (scene.name == "MainMenu")
        {
            SetState(GameState.MainMenu);
        }
        else
        {
            SetState(GameState.Gameplay);
            Debug.Log(
                $"[GameStateManager] State sau khi vào scene = {CurrentState}"
            );
        }
    }

    /*private void Start()
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
    }*/

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

    public void TogglePause()
    {
        Debug.Log(
            $"[GameStateManager] TogglePause trước khi xử lý: " +
            $"State = {CurrentState}, Paused = {isPaused}"
        );

        if (CurrentState != GameState.Gameplay)
            return;

        isPaused = !isPaused;

        Time.timeScale = isPaused ? 0f : 1f;

        Debug.Log(
            $"[GameStateManager] TogglePause sau khi xử lý: " +
            $"State = {CurrentState}, Paused = {isPaused}, TimeScale = {Time.timeScale}"
        );
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