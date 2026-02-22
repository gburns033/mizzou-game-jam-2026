using UnityEngine;
using Game.Entanglement;

namespace Game.Mobs
{
    [DisallowMultipleComponent]
    public class MobEntangle : MonoBehaviour
    {
        [Header("Type (auto if MobController exists)")]
        public MobType fallbackType = MobType.Medium;

        [Header("Buff/debuff range (fractions)")]
        [Range(0.05f, 0.08f)] public float minPct = 0.05f;
        [Range(0.05f, 0.08f)] public float maxPct = 0.08f;

        public bool lockEffectAfterFirstRoll = true;

        private MobController mobController;
        private bool rolled;
        private EntanglementEffect cached;

        public MobType Type => mobController != null ? mobController.Type : fallbackType;

        private void Awake()
        {
            mobController = GetComponent<MobController>();
        }

        public EntanglementEffect GetEntanglementEffect()
        {
            if (lockEffectAfterFirstRoll && rolled)
                return cached;

            float buff = Random.Range(minPct, maxPct);     // +0.05..+0.08
            float debuff = -Random.Range(minPct, maxPct);  // -0.05..-0.08

            EntanglementEffect e;

            switch (Type)
            {
                case MobType.Heavy:
                    // Heavy: +Strength, -Speed
                    e = new EntanglementEffect(0f, debuff, 0f, buff);
                    break;

                case MobType.Light:
                    // Light: +Speed, -Strength
                    e = new EntanglementEffect(0f, buff, 0f, debuff);
                    break;

                case MobType.Medium:
                default:
                    // Medium: +AttackSpeed, -Speed (milder)
                    e = new EntanglementEffect(0f, debuff * 0.5f, buff * 0.75f, 0f);
                    break;
            }

            if (lockEffectAfterFirstRoll)
            {
                cached = e;
                rolled = true;
            }

            return e;
        }
    }
}