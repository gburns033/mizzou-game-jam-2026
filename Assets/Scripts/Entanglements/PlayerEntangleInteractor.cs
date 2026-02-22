using UnityEngine;
using UnityEngine.InputSystem;
using Game.Mobs;

namespace Game.Entanglement
{
    [DisallowMultipleComponent]
    public class PlayerEntangleInteractor : MonoBehaviour
    {
        [Header("Debug")]
        public bool debugLogs = true;

        [Header("Input")]
        public Key entangleKey = Key.E;

        [Header("Detection")]
        public float entangleRange = 2.0f;
        public LayerMask mobLayer;

        [Header("Cooldown")]
        public float cooldownSeconds = 0.25f;

        [Header("Refs")]
        [SerializeField] private EntangleAbility entangleAbility;

        private PlayerController player;
        private float nextAllowedTime;

        private void Start()
        {
            if (debugLogs) Debug.Log("[Entangle] Start fired", this);
        }

        private void Awake()
        {
            player = GetComponentInParent<PlayerController>();
            if (player == null) player = GetComponentInChildren<PlayerController>(true);

            if (entangleAbility == null)
                entangleAbility = GetComponent<EntangleAbility>();

            // Auto-add if missing (so you can’t forget in Inspector)
            if (entangleAbility == null)
                entangleAbility = gameObject.AddComponent<EntangleAbility>();

            if (debugLogs)
                Debug.Log($"[Entangle] Awake on {name}. PlayerController found? {player != null} | EntangleAbility found? {entangleAbility != null}", this);

            }

        private void Update()
        {
            if (player == null || entangleAbility == null) return;

            if (Keyboard.current == null)
            {
                if (debugLogs) Debug.LogWarning("[Entangle] Keyboard.current is null (no keyboard?)");
                return;
            }

            if (!Keyboard.current[entangleKey].wasPressedThisFrame) return;

            if (debugLogs) Debug.Log("[Entangle] Key pressed", this);

            if (Time.time < nextAllowedTime)
            {
                if (debugLogs) Debug.Log("[Entangle] On cooldown", this);
                return;
            }

            var entangle = FindClosestEntangleable();
            if (entangle == null)
            {
                if (debugLogs) Debug.Log("[Entangle] No MobEntangle found in range/layer", this);
                return;
            }

            // Call the ability (this triggers freeze + pause + apply)
            entangleAbility.TryEntangle(entangle.gameObject);

            nextAllowedTime = Time.time + cooldownSeconds;

            Debug.Log($"[Entangle] Key press handled by: {gameObject.name}", this);
        }

        private MobEntangle FindClosestEntangleable()
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, entangleRange);

            if (debugLogs && hits != null)
            {
                for (int i = 0; i < hits.Length; i++)
                    Debug.Log($"[Entangle] Hit {hits[i].name} layer={hits[i].gameObject.layer}", hits[i]);
            }

            if (debugLogs) Debug.Log($"[Entangle] Overlap hits: {(hits == null ? 0 : hits.Length)}", this);

            if (hits == null || hits.Length == 0) return null;

            MobEntangle best = null;
            float bestDist = float.PositiveInfinity;

            foreach (var h in hits)
            {
                var ent = h.GetComponentInParent<MobEntangle>();
                if (ent == null) continue;

                float d = Vector2.Distance(transform.position, ent.transform.position);
                if (d < bestDist)
                {
                    bestDist = d;
                    best = ent;
                }
            }

            return best;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(transform.position, entangleRange);
        }
    }
}