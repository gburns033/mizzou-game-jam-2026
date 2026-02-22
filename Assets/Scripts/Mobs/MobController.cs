using System.Collections;
using UnityEngine;

namespace Game.Mobs
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody2D))]
    public class MobController : MonoBehaviour
    {
        [Header("Config")]
        public MobStatsSO stats;

        [Header("Runtime")]
        [SerializeField] private float currentHealth;

        [Header("Status")]
        [SerializeField] private bool isFrozen;

        private Rigidbody2D rb;
        private Animator animator;

        private Coroutine freezeRoutine;
        private RigidbodyConstraints2D savedConstraints;

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
        }

        public void SetVelocity(Vector2 velocity)
        {
            if (isFrozen)
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
            currentHealth -= amount;
            if (currentHealth <= 0f)
            {
                currentHealth = 0f;
                Die();
            }
        }

        private void Die()
        {
            if (animator != null) animator.SetTrigger("Die");
            Destroy(gameObject, 0.5f);
        }
    }
}