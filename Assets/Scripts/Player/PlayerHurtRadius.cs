using UnityEngine;
using Game.Mobs;

[DisallowMultipleComponent]
public class PlayerHurtRadius : MonoBehaviour
{
    [Header("Detection")]
    public float hurtRadius = 1.25f;  // temporarily larger for debugging
    public LayerMask mobLayerMask;    // should be "Mob"

    [Header("Damage")]
    public float damageCooldown = 0.5f;
    public bool useMobStatsDamage = true;
    public float fallbackDamage = 10f;

    [Header("Center (optional)")]
    public Transform center;

    [Header("Debug")]
    public bool debugLogs = true;
    public bool debugDumpOnce = true;

    private PlayerController player;
    private float nextDamageTime;

    private void Awake()
    {
        player = GetComponent<PlayerController>() ?? GetComponentInParent<PlayerController>();
        if (player == null)
        {
            Debug.LogError("[PlayerHurtRadius] No PlayerController found.", this);
            enabled = false;
            return;
        }

        if (center == null) center = transform;

        // Safety: if you forgot to set mask, auto-set if layer exists
        if (mobLayerMask.value == 0)
        {
            int mobLayer = LayerMask.NameToLayer("Mob");
            if (mobLayer != -1)
                mobLayerMask = 1 << mobLayer;
        }

        if (debugLogs)
            Debug.Log($"[PlayerHurtRadius] Awake. mobLayerMask={mobLayerMask.value} (0 means NOTHING). center={center.name}", this);
    }

    private void Update()
    {
        Vector2 pos = center.position;

        // 1) Dump ALL colliders in radius (ignores layer masks) — tells us what's actually nearby.
        if (debugLogs && debugDumpOnce)
        {
            var all = Physics2D.OverlapCircleAll(pos, hurtRadius);
            Debug.Log($"[PlayerHurtRadius] OverlapCircleAll: found {all.Length} colliders within r={hurtRadius} at {pos}", this);
            for (int i = 0; i < all.Length; i++)
            {
                var c = all[i];
                Debug.Log($"  -> {c.name} | layer={LayerMask.LayerToName(c.gameObject.layer)} | enabled={c.enabled}", c);
            }
            debugDumpOnce = false;
        }

        // Cooldown
        if (Time.time < nextDamageTime) return;

        // 2) Now check ONLY mobs by mask
        Collider2D hit = Physics2D.OverlapCircle(pos, hurtRadius, mobLayerMask);
        if (hit == null)
        {
            if (debugLogs) Debug.Log($"[PlayerHurtRadius] No MOB found in radius using mask={mobLayerMask.value}.", this);
            return;
        }

        float dmg = ResolveDamageFrom(hit);
        player.Stats.TakeDamage(dmg);
        nextDamageTime = Time.time + damageCooldown;

        if (debugLogs)
            Debug.Log($"[PlayerHurtRadius] Took {dmg} from {hit.name} (layer={LayerMask.LayerToName(hit.gameObject.layer)}). HP={player.Stats.CurrentHealth}/{player.Stats.MaxHealth}", this);
    }

    private float ResolveDamageFrom(Collider2D hit)
    {
        if (!useMobStatsDamage) return fallbackDamage;

        var mobController = hit.GetComponent<MobController>() ?? hit.GetComponentInParent<MobController>();
        if (mobController != null && mobController.Stats != null)
            return mobController.Stats.attackDamage;

        return fallbackDamage;
    }

    private void OnDrawGizmosSelected()
    {
        Transform c = center != null ? center : transform;
        Gizmos.DrawWireSphere(c.position, hurtRadius);
    }
}