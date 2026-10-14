using UnityEngine;

public class PauseButton : MonoBehaviour
{
    public void TogglePause()
    {
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.TogglePause();
        }
    }
}