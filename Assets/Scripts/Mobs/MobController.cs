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

        private Rigidbody2D rb;
        private Animator animator;

        public Rigidbody2D RB => rb;
        public Animator Animator => animator;

        public MobStatsSO Stats => stats;
        public MobType Type => stats != null ? stats.mobType : MobType.Medium;

        public float CurrentHealth => currentHealth;

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
            rb.linearVelocity = velocity;
        }

        public void Stop()
        {
            rb.linearVelocity = Vector2.zero;
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