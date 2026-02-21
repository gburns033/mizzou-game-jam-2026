using UnityEngine;

namespace Game.Mobs
{
    [CreateAssetMenu(menuName = "Game/Mobs/Mob Stats", fileName = "MobStatsSO")]
    public class MobStatsSO : ScriptableObject
    {
        public MobType mobType;

        [Header("Core Stats")]
        public float maxHealth = 100f;
        public float moveSpeed = 3f;
        public float attackDamage = 10f;

        [Header("AI")]
        public float detectionRadius = 5f;
        public float wanderSpeedMult = 0.6f;

        [Header("Wander")]
        public float wanderChangeInterval = 1.25f;

        [Header("Idle")]
        public float idleMinTime = 0.6f;
        public float idleMaxTime = 1.6f;
        [Range(0f, 1f)] public float idleChanceOnWanderPick = 0.25f;
    }
}