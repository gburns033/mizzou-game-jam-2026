using UnityEngine;

[CreateAssetMenu(menuName = "Mobs/Mob Wave Scaling")]
public class MobWaveStatsSO : ScriptableObject
{
    [Header("Per-Round Growth (starting at round 1 = 1.0x)")]
    public float hpPerRound = 0.15f;
    public float damagePerRound = 0.10f;
    public float speedPerRound = 0.03f;

    [Header("Caps (optional)")]
    public float maxHpMult = 10f;
    public float maxDamageMult = 10f;
    public float maxSpeedMult = 3f;

    public float HpMult(int round) =>
        Mathf.Min(1f + Mathf.Max(0, round - 1) * hpPerRound, maxHpMult);

    public float DamageMult(int round) =>
        Mathf.Min(1f + Mathf.Max(0, round - 1) * damagePerRound, maxDamageMult);

    public float SpeedMult(int round) =>
        Mathf.Min(1f + Mathf.Max(0, round - 1) * speedPerRound, maxSpeedMult);
}