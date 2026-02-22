using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Damage")]
    public float damage = 10f;

    [Header("Lifetime")]
    public float lifetime = 2f;

    [Header("Ignore Layers (names must match your Layer names)")]
    [SerializeField] private string playerLayerName = "Player";
    [SerializeField] private string handLayerName   = "Hand";
    [SerializeField] private string bulletLayerName = "Bullet";

    [Header("Debug")]
    public bool logHits = false;

    private int _playerLayer;
    private int _handLayer;
    private int _bulletLayer;

    private void Awake()
    {
        // Cache layer ids once (faster + avoids repeated NameToLayer calls)
        _playerLayer = LayerMask.NameToLayer(playerLayerName);
        _handLayer   = LayerMask.NameToLayer(handLayerName);
        _bulletLayer = LayerMask.NameToLayer(bulletLayerName);

        // Helpful warnings if a layer name is wrong
        if (_playerLayer == -1) Debug.LogWarning($"[Bullet] Layer '{playerLayerName}' does not exist.");
        if (_handLayer == -1)   Debug.LogWarning($"[Bullet] Layer '{handLayerName}' does not exist.");
        if (_bulletLayer == -1) Debug.LogWarning($"[Bullet] Layer '{bulletLayerName}' does not exist.");
    }

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        int hitLayer = collision.collider.gameObject.layer;

        // Force-ignore Player / Hand / Bullet collisions (pass through)
        // (Even if your Physics2D matrix is wrong, this prevents bullet popping.)
        if (hitLayer == _playerLayer || hitLayer == _handLayer || hitLayer == _bulletLayer)
        {
            if (logHits)
                Debug.Log($"[Bullet] Ignored hit: {collision.collider.name} | layer: {LayerMask.LayerToName(hitLayer)}");

            // Do NOT destroy bullet; let it pass through
            return;
        }

        if (logHits)
            Debug.Log($"[Bullet] Hit: {collision.collider.name} | layer: {LayerMask.LayerToName(hitLayer)}");

        // Deal damage if it's a mob (collider could be on a child)
        Mob mob = collision.collider.GetComponent<Mob>() ?? collision.collider.GetComponentInParent<Mob>();
        if (mob != null)
            mob.TakeDamage(damage);

        // Bullet disappears on any non-ignored collision (mob, wall, etc.)
        Destroy(gameObject);
    }
}