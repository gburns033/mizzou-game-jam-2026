using System.Collections;
using UnityEngine;
using Game.Mobs;

namespace Game.Entanglement
{
    [DisallowMultipleComponent]
    public class EntangleAbility : MonoBehaviour
    {
        [Header("Timing")]
        [SerializeField] private float entanglePauseSeconds = 0.5f;

        [Header("Debug")]
        [SerializeField] private bool debugLogs = true;

        private PlayerController player;
        private bool isEntangling;

        private void Awake()
        {
            player = GetComponentInParent<PlayerController>();
            if (player == null) player = GetComponentInChildren<PlayerController>(true);
            
            if (debugLogs)
                Debug.Log($"[EntangleAbility] Awake on {name}. PlayerController found? {player != null}", this);
        }

        public void TryEntangle(GameObject mobGO)
        {
            if (isEntangling) return;
            if (player == null) return;
            if (mobGO == null) return;

            // Support child colliders
            MobController mobController = mobGO.GetComponentInParent<MobController>();
            MobEntangle mobEntangle = mobGO.GetComponentInParent<MobEntangle>();

            if (mobEntangle == null)
            {
                if (debugLogs) Debug.LogWarning("[EntangleAbility] Target has no MobEntangle.", this);
                return;
            }

            StartCoroutine(EntangleRoutine(mobController, mobEntangle));
        }

        private IEnumerator EntangleRoutine(MobController mobController, MobEntangle mobEntangle)
        {
            isEntangling = true;

            if (debugLogs)
                Debug.Log($"[EntangleAbility] Entangle started -> {mobEntangle.name}", this);

            // 1) Freeze mob
            if (mobController != null)
                mobController.FreezeFor(entanglePauseSeconds);

            // 2) Pause
            yield return new WaitForSeconds(entanglePauseSeconds);

            // 3) Roll effect after pause
            EntanglementEffect effect = mobEntangle.GetEntanglementEffect();

            // 4) Apply to player stats
            player.Stats.AddHealthPercent(effect.healthPct);
            player.Stats.AddSpeedPercent(effect.speedPct);
            player.Stats.AddAttackSpeedPercent(effect.attackSpeedPct);
            player.Stats.AddStrengthPercent(effect.strengthPct);

            if (debugLogs)
            {
                Debug.Log($"[EntangleAbility] SUCCESS with {mobEntangle.Type} -> " +
                          $"HP {effect.healthPct:+0.###;-0.###;0}, " +
                          $"SPD {effect.speedPct:+0.###;-0.###;0}, " +
                          $"AS {effect.attackSpeedPct:+0.###;-0.###;0}, " +
                          $"STR {effect.strengthPct:+0.###;-0.###;0}", this);
            }

            isEntangling = false;
        }
    }
}