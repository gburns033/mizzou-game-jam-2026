using UnityEngine;

public class Axe : Weapon
{
    public float axeRange = 1.5f;
    public float orbitRadius = 1.5f;

    private SpriteRenderer sr;
    private Transform playerTransform;

    protected override void Start()
    {
        base.Start();
        // Find player by tag instead of using parent
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        sr = GetComponent<SpriteRenderer>();
        sr.flipX = true; // Flip the axe sprite to point left by defaults
    }

    protected override void Update()
    {
        OrbitAroundPlayer();
        HandleSortingOrder();
    }

    void OrbitAroundPlayer()
    {
        if (inputHandler == null || playerTransform == null) return;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(inputHandler.MousePosition);
        mousePos.z = 0f;

        if (mousePos.x < transform.position.x)
        {
            // Look Left
            sr.flipX = true;
            sr.flipY = false;
        }
        else if (mousePos.x > transform.position.x)
        {
            sr.flipX = false;
            sr.flipY = true;
        }

        // Rotate toward mouse
        transform.rotation = Quaternion.Euler(0, 0,
        Mathf.Atan2(mousePos.y - playerTransform.position.y,
                    mousePos.x - playerTransform.position.x)
        * Mathf.Rad2Deg - 90); // changed -90 to +90

        // Move axe position to orbit around player
        Vector3 direction = (mousePos - playerTransform.position).normalized;
        transform.position = playerTransform.position + direction * orbitRadius;
    }

    void HandleSortingOrder()
    {
        if (sr == null || playerTransform == null || inputHandler == null) return;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(inputHandler.MousePosition);
        sr.sortingOrder = mousePos.x >= playerTransform.position.x ? 2 : 0;
    }


    protected override void Attack()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, axeRange);
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Enemy"))
                Debug.Log("Hit an enemy!");
        }
    }
}