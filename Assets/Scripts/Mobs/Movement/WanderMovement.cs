using UnityEngine;
using Game.Mobs;

namespace Game.Mobs.Movement
{
    public class WanderMovement : MonoBehaviour, IMovementStrategy
    {
        private MobController mob;

        private Vector2 dir;
        private float nextChangeTime;

        // idle control
        private bool idling;
        private float idleEndTime;

        private void Awake()
        {
            mob = GetComponent<MobController>();
            PickNew();
        }

        public Vector2 GetDesiredVelocity()
        {
            var s = mob.Stats;

            // idle
            if (idling)
            {
                if (Time.time >= idleEndTime)
                {
                    idling = false;
                    PickNew();
                }
                return Vector2.zero;
            }

            if (Time.time >= nextChangeTime)
            {
                // occasionally idle
                if (Random.value < s.idleChanceOnWanderPick)
                {
                    idling = true;
                    idleEndTime = Time.time + Random.Range(s.idleMinTime, s.idleMaxTime);
                    return Vector2.zero;
                }

                PickNew();
            }

            return dir * (s.moveSpeed * s.wanderSpeedMult);
        }

        private void PickNew()
        {
            var s = mob.Stats;
            dir = Random.insideUnitCircle.normalized;
            nextChangeTime = Time.time + Mathf.Max(0.1f, s.wanderChangeInterval);
        }
    }
}