using UnityEngine;
using System;

[System.Serializable]
public class PlayerStats
{
    [Header("Base Stats")]
    public float baseHealth = 100f;
    public float baseMoveSpeed = 5f;
    public float baseAttSpeed = 1f;
    public float baseStrength = 10f;

    private float healthMod = 1f;
    private float speedMod = 1f;
    private float attackSpeedMod = 1f;
    private float strengthMod = 1f;

    // --- Runtime Health ---
    private float currentHealth;

    // Optional: event for UI or death handling
    public event Action OnPlayerDied;

    // Properties
    public float MaxHealth => baseHealth * healthMod;
    public float CurrentHealth => currentHealth;
    public float MoveSpeed => baseMoveSpeed * speedMod;
    public float AttackSpeed => baseAttSpeed * attackSpeedMod;
    public float Strength => baseStrength * strengthMod;

    // Call this once at start
    public void Initialize()
    {
        currentHealth = MaxHealth;
        Debug.Log($"Player initialized with HP: {currentHealth}");
    }

    public void TakeDamage(float amount)
    {
        if (amount <= 0f) return;

        // If already dead, ignore further damage
        if (currentHealth <= 0f) return;

        currentHealth -= amount;

        if (currentHealth <= 0f)
        {
            currentHealth = 0f;

            Debug.Log("💀 PLAYER DIED");
            OnPlayerDied?.Invoke();
        }
    }

    public void Heal(float amount)
    {
        if (amount <= 0f) return;
        currentHealth = Mathf.Min(MaxHealth, currentHealth + amount);
    }

    // Entanglement buffs (unchanged)
    public void AddHealthPercent(float amount)
    {
        healthMod += amount;

        // When max health changes, clamp current health
        currentHealth = Mathf.Min(currentHealth, MaxHealth);
    }

    public void AddSpeedPercent(float amount) => speedMod += amount;
    public void AddAttackSpeedPercent(float amount) => attackSpeedMod += amount;
    public void AddStrengthPercent(float amount) => strengthMod += amount;
}