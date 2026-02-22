using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler : MonoBehaviour
{
    public static SceneHandler Instance { get; private set; }

    [Header("Default targets")]
    [SerializeField] private string startMenuSceneName = "StartMenu";
    [SerializeField] private string defaultGameSceneName = "Gabe"; // <-- change to your gameplay scene name

    [Header("Options")]
    [SerializeField] private float defaultLoadDelay = 0f;

    private bool isLoading;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LoadStartMenu() => LoadScene(startMenuSceneName);
    public void LoadGame() => LoadScene(defaultGameSceneName);

    public void LoadScene(string sceneName) => LoadScene(sceneName, defaultLoadDelay);

    public void LoadScene(string sceneName, float delaySeconds)
    {
        if (isLoading) return;
        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogError("[SceneHandler] sceneName is null/empty.");
            return;
        }

        StartCoroutine(LoadRoutine(sceneName, delaySeconds));
    }

    public void ReloadCurrentScene()
    {
        if (isLoading) return;
        LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private IEnumerator LoadRoutine(string sceneName, float delaySeconds)
    {
        isLoading = true;

        if (delaySeconds > 0f)
            yield return new WaitForSeconds(delaySeconds);

        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        if (op == null)
        {
            Debug.LogError($"[SceneHandler] Failed to load '{sceneName}'. Is it added to Build Settings?");
            isLoading = false;
            yield break;
        }

        while (!op.isDone)
            yield return null;

        isLoading = false;
    }
}