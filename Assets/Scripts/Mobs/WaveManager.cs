using System.Collections;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }

    private enum WaveState { Intermission, Spawning, Fighting }
    [SerializeField] private WaveState state = WaveState.Intermission;

    [Header("Wave")]
    [Tooltip("Starts at 0. Wave 1 begins after the initial intermission.")]
    public int currentRound = 0;

    [Header("Intermission")]
    [Tooltip("Wait this long AFTER the wave is fully cleared (all spawned + all dead).")]
    public float intermissionAfterClear = 15f;

    [Header("Spawning")]
    public Transform[] spawnPoints;
    public GameObject[] mobPrefabs;      // Light/Medium/Heavy prefabs
    public float spawnInterval = 0.5f;   // time between spawn attempts (zombies trickle)
    public MobWaveStatsSO waveScaling;

    [Header("Count Scaling")]
    public int baseEnemyCount = 10;      // zombies-like: more per wave
    public int enemyCountGrowth = 3;

    [Header("Zombies Style")]
    [Tooltip("Max number of enemies allowed alive at once. When you kill one, another can spawn.")]
    public int maxAliveAtOnce = 6;

    // Tracking
    private int enemiesAlive = 0;
    private int remainingToSpawn = 0;
    private bool isSpawning = false;
    private Coroutine waveLoopRoutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        waveLoopRoutine = StartCoroutine(WaveLoop());
    }

    private IEnumerator WaveLoop()
    {
        while (true)
        {
            // Intermission before starting the next wave (also happens before Wave 1).
            state = WaveState.Intermission;
            if (intermissionAfterClear > 0f)
                yield return new WaitForSeconds(intermissionAfterClear);

            // Start next wave (sets remainingToSpawn, then trickles spawns under cap).
            StartNextWave();

            // Fight until the wave is fully complete:
            // - nothing left to spawn
            // - no enemies alive
            state = WaveState.Fighting;
            while (remainingToSpawn > 0 || enemiesAlive > 0 || isSpawning)
                yield return null;

            // loop repeats -> intermission -> next wave
        }
    }

    private void StartNextWave()
    {
        // Validation
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("[WaveManager] No spawnPoints assigned.");
            return;
        }
        if (mobPrefabs == null || mobPrefabs.Length == 0)
        {
            Debug.LogError("[WaveManager] No mobPrefabs assigned.");
            return;
        }
        if (waveScaling == null)
        {
            Debug.LogError("[WaveManager] No waveScaling assigned.");
            return;
        }

        currentRound++;

        int totalToSpawn = baseEnemyCount + (currentRound - 1) * enemyCountGrowth;
        if (totalToSpawn < 0) totalToSpawn = 0;

        remainingToSpawn = totalToSpawn;

        if (waveLoopRoutine != null) { /* just keeping reference; loop already running */ }

        // Start the trickle spawner
        if (!isSpawning)
            StartCoroutine(TrickleSpawnRoutine(currentRound));
    }

    private IEnumerator TrickleSpawnRoutine(int roundForThisWave)
    {
        state = WaveState.Spawning;
        isSpawning = true;

        // Keep trying to spawn until we've spawned the whole wave quota
        while (remainingToSpawn > 0)
        {
            // Only spawn if we're under the alive cap
            if (enemiesAlive < maxAliveAtOnce)
            {
                SpawnMob(roundForThisWave);
                remainingToSpawn--;
            }

            if (spawnInterval > 0f)
                yield return new WaitForSeconds(spawnInterval);
            else
                yield return null;
        }

        isSpawning = false;
        state = WaveState.Fighting;
    }

    private void SpawnMob(int round)
    {
        Transform sp = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject prefab = ChooseMobPrefab(round);

        GameObject obj = Instantiate(prefab, sp.position, sp.rotation);

        // ✅ IMPORTANT: look on children too (common prefab hierarchy)
        global::Mob mob = obj.GetComponentInChildren<global::Mob>();

        // If the prefab isn't configured right, destroy it and DO NOT count it
        if (mob == null)
        {
            Debug.LogError($"[WaveManager] Prefab '{prefab.name}' has NO Mob component (on root or children). Fix prefab.");
            Destroy(obj);
            return;
        }

        if (mob.baseStats == null)
        {
            Debug.LogError($"[WaveManager] Mob on prefab '{prefab.name}' has NO baseStats assigned. Fix prefab.");
            Destroy(obj);
            return;
        }

        // ✅ Only now do we count it as alive
        enemiesAlive++;

        var runtime = MobRuntimeStatBuilder.Build(mob.baseStats, waveScaling, round);
        mob.Init(runtime);
    }

    private GameObject ChooseMobPrefab(int round)
    {
        if (mobPrefabs.Length == 1) return mobPrefabs[0];

        float r = Random.value;

        // Simple weights (tweak later)
        if (round < 4)
        {
            return mobPrefabs[0];
        }
        else if (round < 8)
        {
            if (mobPrefabs.Length >= 2 && r < 0.30f) return mobPrefabs[1];
            return mobPrefabs[0];
        }
        else
        {
            if (mobPrefabs.Length >= 3 && r < 0.20f) return mobPrefabs[2];
            if (mobPrefabs.Length >= 2 && r < 0.50f) return mobPrefabs[1];
            return mobPrefabs[0];
        }
    }

    // Call exactly once when a mob dies
    public void NotifyMobDied()
    {
        enemiesAlive--;
        if (enemiesAlive < 0) enemiesAlive = 0;
    }

    public int GetEnemiesAlive() => enemiesAlive;
    public int GetRemainingToSpawn() => remainingToSpawn;
    public int GetCurrentRound() => currentRound;
    public bool IsSpawning() => isSpawning;
}