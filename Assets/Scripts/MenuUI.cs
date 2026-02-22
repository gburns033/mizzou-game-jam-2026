using UnityEngine;

public class MenuUI : MonoBehaviour
{
    public void Play()
    {
        if (SceneHandler.Instance == null)
        {
            Debug.LogError("[MenuUI] SceneHandler.Instance is null. Add SceneHandler to StartMenu scene.");
            return;
        }

        SceneHandler.Instance.LoadGame();
    }

    public void QuitGame()
    {
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    }
}