using System.Collections;
using UnityEngine;
using Game.UI;

namespace Game.Mobs
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody2D))]
    public class MobController : MonoBehaviour
    {
        [Header("Config")]
        public MobStatsSO stats;

        [Header("Damage UI")]
        public DamagePopup damagePopupPrefab;
        public Vector3 damagePopupOffset = new Vector3(0f, 0.5f, 0f);

        [Header("Runtime")]
        [SerializeField] private float currentHealth;

        [Header("Status")]
        [SerializeField] private bool isFrozen;

        [Header("Player Damage (Hurt Radius Query)")]
        [Tooltip("Small radius around the mob where the player takes damage (tight hitbox).")]
        public float hurtRadius = 1.25f;

        [Tooltip("Which layers count as player for taking damage. Set this to ONLY the Player layer.")]
        public LayerMask playerHurtMask;

        [Tooltip("Seconds between damage ticks while the player is within hurtRadius.")]
        public float hurtDamageCooldown = 0.5f;

        [Tooltip("If true, uses stats.attackDamage. If false, uses hurtDamageOverride.")]
        public bool useStatsAttackDamage = true;

        [Tooltip("Used only if useStatsAttackDamage is false.")]
        public float hurtDamageOverride = 10f;

        [Tooltip("Where the hurt radius is centered (defaults to this transform). Optional.")]
        public Transform hurtCenter;

        [Header("Debug")]
        public bool debugHurt = false;

        private float nextHurtTime;

        private Rigidbody2D rb;
        private Animator animator;

        private Coroutine freezeRoutine;
        private RigidbodyConstraints2D savedConstraints;

        private bool isDying;

        public Rigidbody2D RB => rb;
        public Animator Animator => animator;

        public MobStatsSO Stats => stats;
        public MobType Type => stats != null ? stats.mobType : MobType.Medium;

        public float CurrentHealth => currentHealth;
        public bool IsFrozen => isFrozen;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();

            rb.gravityScale = 0f;
            rb.freezeRotation = true;

            if (stats == null)
            {
                Debug.LogError($"{name}: MobController missing MobStatsSO.", this);
                enabled = false;
                return;
            }

            currentHealth = stats.maxHealth;

            if (hurtCenter == null)
                hurtCenter = transform;
        }

        private void Update()
        {
            // Damage player if within a tight radius (does NOT rely on triggers/collisions)
            TryHurtPlayerInRadius();
        }

        public void SetVelocity(Vector2 velocity)
        {
            if (isFrozen || isDying)
            {
                rb.linearVelocity = Vector2.zero;
                return;
            }

            rb.linearVelocity = velocity;
        }

        public void Stop()
        {
            rb.linearVelocity = Vector2.zero;
        }

        /// <summary>
        /// Hard-freezes the mob for duration seconds (stops any Rigidbody2D movement).
        /// Calling FreezeFor again restarts the timer.
        /// </summary>
        public void FreezeFor(float duration)
        {
            if (duration <= 0f) return;

            if (freezeRoutine != null)
                StopCoroutine(freezeRoutine);

            freezeRoutine = StartCoroutine(FreezeRoutine(duration));
        }

        private IEnumerator FreezeRoutine(float duration)
        {
            isFrozen = true;

            // Save constraints, then hard-freeze everything
            savedConstraints = rb.constraints;

            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;

            yield return new WaitForSeconds(duration);

            // Restore constraints
            rb.constraints = savedConstraints;

            isFrozen = false;
            freezeRoutine = null;
        }

        public void TakeDamage(float amount)
        {
            if (isDying) return;

            currentHealth -= amount;
            if (currentHealth <= 0f)
            {
                currentHealth = 0f;
                Die();
            }

            if (damagePopupPrefab != null)
            {
                var popup = Instantiate(
                    damagePopupPrefab,
                    transform.position + damagePopupOffset,
                    Quaternion.identity
                );

                popup.SetText(Mathf.RoundToInt(amount).ToString());
            }
        }

        private void Die()
        {
            if (isDying) return;
            isDying = true;

            if (animator != null) animator.SetTrigger("Die");
            Destroy(gameObject, 0.5f);
        }

        // ----------------------------
        // Hurt Radius Logic (Physics Query)
        // ----------------------------

        private float GetHurtDamage()
        {
            if (!useStatsAttackDamage) return hurtDamageOverride;
            if (stats == null) return hurtDamageOverride;
            return stats.attackDamage;
        }

        private void TryHurtPlayerInRadius()
        {
            if (isDying || isFrozen) return;
            if (Time.time < nextHurtTime) return;
            if (hurtRadius <= 0f) return;

            Vector2 center = (hurtCenter != null) ? 
                (Vector2)hurtCenter.position : 
                (Vector2)transform.position;

            Collider2D hit = Physics2D.OverlapCircle(center, hurtRadius, playerHurtMask);
            if (hit == null) return;

            var player = hit.GetComponentInParent<PlayerController>();
            if (player == null) return;

            float dmg = GetHurtDamage();
            player.Stats.TakeDamage(dmg);

            nextHurtTime = Time.time + hurtDamageCooldown;

            // 🔥 Only log actual damage
            Debug.Log($"[DAMAGE] Player took {dmg} from {name}. HP: {player.Stats.CurrentHealth}/{player.Stats.MaxHealth}");
        }

        private void OnDrawGizmosSelected()
        {
            if (hurtRadius <= 0f) return;

            Vector3 center = (hurtCenter != null) ? hurtCenter.position : transform.position;
            Gizmos.DrawWireSphere(center, hurtRadius);
        }
    }
}