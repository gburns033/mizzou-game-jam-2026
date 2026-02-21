using UnityEngine;
using Game.Mobs.Targeting;

namespace Game.Mobs.Movement
{
    public class ChaseMovement : MonoBehaviour, IMovementStrategy
    {
        private MobController mob;
        private ITargetProvider targetProvider;

        private void Awake()
        {
            mob = GetComponent<MobController>();
            targetProvider = GetComponent<ITargetProvider>(); // e.g., TagTargetProvider
        }

        public bool IsTargetInRange(out Transform target)
        {
            target = targetProvider != null ? targetProvider.GetTarget() : null;
            if (target == null) return false;

            float dist = Vector2.Distance(mob.transform.position, target.position);
            return dist <= mob.Stats.detectionRadius;
        }

        public Vector2 GetDesiredVelocity()
        {
            if (!IsTargetInRange(out Transform target) || target == null)
                return Vector2.zero;

            Vector2 dir = ((Vector2)target.position - mob.RB.position).normalized;
            return dir * mob.Stats.moveSpeed;
        }
    }
}