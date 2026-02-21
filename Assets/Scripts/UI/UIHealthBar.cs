using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    private PlayerController player;
    [SerializeField] private Image healthBarFill;
    [SerializeField] private TextMeshProUGUI healthText;

    private float currentHealth;

    void Start()
    {
        // UNCOMMENT ONCE PLAYER IS IN SCENE
        // player = GetComponent<PlayerController>();
        // currentHealth = player.Stats.Health(?);
    }

    void Update()
    {
        // UNCOMMENT ONCE PLAYER IS IN SCENE
        // float healthPercent = currentHealth / player.Stats.MaxHealth;
        // healthBarFill.fillAmount = healthPercent;
        // healthText.text = string.Format("{0}/{1}", currentHealth, player.Stats.MaxHealth);

        healthBarFill.fillAmount = .5f;
        healthText.text = string.Format("{0}/{1}", 50, 100);
    }
}