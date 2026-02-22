using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDeathHandler : MonoBehaviour
{
    [SerializeField] private string gameOverSceneName = "GameOverScene";
    [SerializeField] private bool addSmallDelay = false;
    [SerializeField] private float delaySeconds = 1.0f;

    private PlayerController player;
    private bool triggered = false;

    private void Awake()
    {
        player = GetComponent<PlayerController>();
        if (player == null)
        {
            Debug.LogError("[PlayerDeathHandler] PlayerController not found on Player.", this);
            enabled = false;
            return;
        }
    }

    private void OnEnable()
    {
        // Subscribe
        player.Stats.OnPlayerDied += HandleDied;
    }

    private void OnDisable()
    {
        // Unsubscribe
        if (player != null)
            player.Stats.OnPlayerDied -= HandleDied;
    }

    private void HandleDied()
    {
        if (triggered) return;
        triggered = true;

        Time.timeScale = 1f; // safety reset

        if (!addSmallDelay)
        {
            SceneManager.LoadScene(gameOverSceneName);
            return;
        }

        StartCoroutine(LoadAfterDelay());
    }

    private System.Collections.IEnumerator LoadAfterDelay()
    {
        yield return new WaitForSeconds(delaySeconds);
        SceneManager.LoadScene(gameOverSceneName);
    }
}