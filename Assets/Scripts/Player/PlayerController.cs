using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerStats stats;
    public PlayerStats Stats => stats;

    [Header("Game Over")]
    [SerializeField] private string gameOverSceneName = "GameOverScene";
    [SerializeField] private float gameOverDelaySeconds = 0.75f;

    private bool deathHandled = false;

    private void Awake()
    {
        if (stats == null)
        {
            Debug.LogError("[PlayerController] stats is null in inspector.", this);
            return;
        }

        stats.Initialize();
        stats.OnPlayerDied += OnDied;

        Debug.Log($"[PlayerController] Initialized HP {stats.CurrentHealth}/{stats.MaxHealth}", this);
    }

    private void OnDestroy()
    {
        if (stats != null) stats.OnPlayerDied -= OnDied;
    }

    private void OnDied()
    {
        if (deathHandled) return;
        deathHandled = true;

        Debug.Log("💀 PLAYER DIED");

        // Safety: if anything paused/slowed time, reset before switching scenes
        Time.timeScale = 1f;

        if (gameOverDelaySeconds > 0f)
            Invoke(nameof(LoadGameOver), gameOverDelaySeconds);
        else
            LoadGameOver();
    }

    private void LoadGameOver()
    {
        SceneManager.LoadScene(gameOverSceneName);
    }
}