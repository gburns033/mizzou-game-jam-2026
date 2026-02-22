using UnityEngine;
using TMPro;

namespace Game.UI
{
    public class DamagePopup : MonoBehaviour
    {
        public float floatSpeed = 1.5f;
        public float lifetime = 0.6f;

        private TMP_Text text;
        private float timer;

        private void Awake()
        {
            text = GetComponent<TMP_Text>();
        }

        public void SetText(string value)
        {
            if (text != null)
                text.text = value;
        }

        private void Update()
        {
            transform.position += Vector3.up * floatSpeed * Time.deltaTime;

            timer += Time.deltaTime;

            float alpha = Mathf.Clamp01(1f - timer / lifetime);
            if (text != null)
            {
                Color c = text.color;
                c.a = alpha;
                text.color = c;
            }

            if (timer >= lifetime)
                Destroy(gameObject);
        }
    }
}