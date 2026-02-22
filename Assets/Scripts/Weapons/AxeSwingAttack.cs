using UnityEngine;
using System.Collections.Generic;
using Game.Mobs;

public class AxeSwingAttack : MonoBehaviour
{
    [Header("Swing")]
    public float orbitRadius = 0.6f;
    public float swingAngle = 90f;
    public float swingSpeed = 300f;
    public float restingAngleRight = 90f;
    public float restingAngleLeft = 90f;

    [Header("Damage")]
    public float axeRange = 0.4f;
    public float damage = 25f;

    [Header("Optional Filtering")]
    public bool useMobLayerMask = false;
    public LayerMask mobLayerMask;

    private Transform playerTransform;
    private InputHandler inputHandler;
    private SpriteRenderer sr;

    private bool isSwinging = false;
    private bool isReturning = false;

    private float swingStartAngle;
    private float swingTargetAngle;
    private float swingDirection;
    private float currentAngle;

    public bool IsSwinging => isSwinging;
    public bool IsReturning => isReturning;

    // Track mobs (not colliders) so child colliders don't cause multiple hits
    private readonly HashSet<MobController> hitThisSwing = new HashSet<MobController>();

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();

        // Prefer parent chain (axe is usually under player)
        playerTransform = GetComponentInParent<PlayerController>()?.transform;

        // Fallback: find any PlayerController in scene
        if (playerTransform == null)
            playerTransform = FindFirstObjectByType<PlayerController>()?.transform;

        inputHandler = FindFirstObjectByType<InputHandler>();
        if (inputHandler != null)
            inputHandler.OnAttackPerformed += StartSwing;
    }

    private void Start()
    {
        // Default angle
        currentAngle = restingAngleRight;
    }

    private void OnDestroy()
    {
        if (inputHandler != null)
            inputHandler.OnAttackPerformed -= StartSwing;
    }

    private void Update()
    {
        if (playerTransform == null || inputHandler == null) return;

        if (isSwinging)
            ContinueSwing();
        else if (isReturning)
            ContinueReturn();
        else
            SnapToRestingPosition();
    }

    void SnapToRestingPosition()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(inputHandler.MousePosition);
        bool facingRight = mousePos.x >= playerTransform.position.x;

        currentAngle = facingRight ? restingAngleRight : restingAngleLeft;
        PlaceAxeAtAngle(currentAngle);
    }

    public void StartSwing()
    {
        if (isSwinging || isReturning) return;

        hitThisSwing.Clear();

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(inputHandler.MousePosition);
        bool facingRight = mousePos.x >= playerTransform.position.x;

        swingDirection = facingRight ? 1f : -1f;
        swingStartAngle = facingRight ? restingAngleRight : restingAngleLeft;
        swingTargetAngle = swingStartAngle + swingAngle * swingDirection;

        currentAngle = swingStartAngle;
        isSwinging = true;
    }

    void ContinueSwing()
    {
        currentAngle += swingSpeed * swingDirection * Time.deltaTime;

        PlaceAxeAtAngle(currentAngle);
        CheckDamage(); // damage during swing frames

        bool done = swingDirection > 0
            ? currentAngle >= swingTargetAngle
            : currentAngle <= swingTargetAngle;

        if (done)
        {
            currentAngle = swingTargetAngle;
            isSwinging = false;
            isReturning = true;
        }
    }

    void ContinueReturn()
    {
        currentAngle -= swingSpeed * 1.5f * swingDirection * Time.deltaTime;

        PlaceAxeAtAngle(currentAngle);

        bool done = swingDirection > 0
            ? currentAngle <= swingStartAngle
            : currentAngle >= swingStartAngle;

        if (done)
        {
            currentAngle = swingStartAngle;
            isReturning = false;
        }
    }

    void PlaceAxeAtAngle(float angle)
    {
        float rad = angle * Mathf.Deg2Rad;

        transform.position = playerTransform.position +
            new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f) * orbitRadius;

        transform.rotation = Quaternion.Euler(0f, 0f, angle + 90f);

        sr.flipX = (angle % 360 + 360) % 360 > 180f;
        sr.flipY = false;
    }

    void CheckDamage()
    {
        Collider2D[] hits = useMobLayerMask
            ? Physics2D.OverlapCircleAll(transform.position, axeRange, mobLayerMask)
            : Physics2D.OverlapCircleAll(transform.position, axeRange);

        for (int i = 0; i < hits.Length; i++)
        {
            // Find mob on this collider or its parents
            MobController mob = hits[i].GetComponentInParent<MobController>();
            if (mob == null) continue;

            // Only once per swing
            if (hitThisSwing.Contains(mob)) continue;

            hitThisSwing.Add(mob);

            Debug.Log("Axe hit: " + mob.name);
            mob.TakeDamage(damage);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, axeRange);
    }
}