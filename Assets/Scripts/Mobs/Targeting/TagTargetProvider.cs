using UnityEngine;

namespace Game.Mobs.Targeting
{
    public class TagTargetProvider : MonoBehaviour, ITargetProvider
    {
        public string targetTag = "Player";
        public float refreshInterval = 0.5f;

        private Transform cached;
        private float nextRefresh;

        public Transform GetTarget()
        {
            if (Time.time >= nextRefresh || cached == null)
            {
                nextRefresh = Time.time + Mathf.Max(0.05f, refreshInterval);
                var go = GameObject.FindGameObjectWithTag(targetTag);
                cached = go != null ? go.transform : null;
            }

            return cached;
        }
    }
}