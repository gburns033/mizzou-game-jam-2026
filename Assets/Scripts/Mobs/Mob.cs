using UnityEngine;
using Game.Mobs; // ✅ lets this script see MobStatsSO

public class Mob : MonoBehaviour
{
    [Header("Do NOT modify this at runtime")]
    public MobStatsSO baseStats;

    [Header("Runtime (scaled)")]
    public float currentHP;
    public float maxHP;
    public float damage;
    public float moveSpeed;

    private bool isDead = false;

    public void Init(MobRuntimeStats stats)
    {
        maxHP = stats.maxHP;
        currentHP = maxHP;
        damage = stats.damage;
        moveSpeed = stats.moveSpeed;

        // If you have a movement script, plug it in here:
        // var mover = GetComponent<MobMovement>();
        // if (mover != null) mover.SetSpeed(moveSpeed);
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHP -= amount;
        if (currentHP <= 0f) Die();
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        if (WaveManager.Instance != null)
            WaveManager.Instance.NotifyMobDied();

        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        // Safety net: if the mob is destroyed without calling Die()
        if (!isDead && WaveManager.Instance != null)
            WaveManager.Instance.NotifyMobDied();
    }
}

