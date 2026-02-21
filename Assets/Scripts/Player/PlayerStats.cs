using UnityEngine;

[System.Serializable]
public class PlayerStats
{
    public float baseHealth = 100f;
    public float baseMoveSpeed = 5f;
    public float baseAttSpeed = 1f;
    public float baseStrength = 10f;

    private float healthMod = 1f;
    private float speedMod = 1f;
    private float attackSpeedMod = 1f;
    private float strengthMod = 1f;

    public float MaxHealth => baseHealth * healthMod;
    public float MoveSpeed => baseMoveSpeed * speedMod;
    public float AttackSpeed => baseAttSpeed * attackSpeedMod;
    public float Strength => baseStrength * strengthMod;

    public void AddHealthPercent(float amount) => healthMod += amount;
    public void AddSpeedPercent(float amount) => speedMod += amount;
    public void AddAttackSpeedPercent(float amount) => attackSpeedMod += amount;
    public void AddStrengthPercent(float amount) => strengthMod += amount;
}
