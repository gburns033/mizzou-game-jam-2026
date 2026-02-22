using System.Collections.Generic;
using UnityEngine;
using Game.Mobs;

namespace Game.Combat
{
    [DisallowMultipleComponent]
    public class AxeHitbox : MonoBehaviour
    {
        [Header("Damage")]
        public float damage = 25f;

        [Header("Targeting")]
        public LayerMask mobLayer; // optional; can be Everything if you want
        public bool useLayerMask = false;

        [Header("Timing")]
        public float hitCooldownSeconds = 0.2f; // prevents multi-hits per frame/swing

        [Header("Debug")]
        public bool debugLogs = false;

        private bool isActive;
        private float nextAllowedHitTime;

        // Prevent hitting same mob multiple times in one swing
        private readonly HashSet<MobController> hitThisSwing = new HashSet<MobController>();

        /// <summary>
        /// Call this at the start of the swing (or via animation event).
        /// </summary>
        public void BeginSwing()
        {
            isActive = true;
            hitThisSwing.Clear();
            if (debugLogs) Debug.Log("[Axe] BeginSwing", this);
        }

        /// <summary>
        /// Call this at the end of the swing (or via animation event).
        /// </summary>
        public void EndSwing()
        {
            isActive = false;
            if (debugLogs) Debug.Log("[Axe] EndSwing", this);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (!isActive) return;
            if (Time.time < nextAllowedHitTime) return;

            if (useLayerMask)
            {
                int otherLayerBit = 1 << other.gameObject.layer;
                if ((mobLayer.value & otherLayerBit) == 0) return;
            }

            MobController mob = other.GetComponentInParent<MobController>();
            if (mob == null) return;

            // only once per swing
            if (hitThisSwing.Contains(mob)) return;

            hitThisSwing.Add(mob);
            nextAllowedHitTime = Time.time + hitCooldownSeconds;

            mob.TakeDamage(damage);

            if (debugLogs)
                Debug.Log($"[Axe] Hit {mob.name} for {damage}. Mob HP now {mob.CurrentHealth}", this);
        }
    }
}