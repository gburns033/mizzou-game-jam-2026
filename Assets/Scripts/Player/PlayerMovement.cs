using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private PlayerController player;
    
    [SerializeField] private Animator animator;
    private Rigidbody2D rb;
    private InputHandler input;

    void Start()
    {
        player = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
        input = GetComponent<InputHandler>();
    }

    void Update()
    {
        // Use the MoveInput from our InputHandler
        float currentSpeed = player.Stats.MoveSpeed;
        rb.linearVelocity = input.MoveInput * currentSpeed;

        if (input.MoveInput.magnitude > 0)
        {
            animator.SetBool("isRunning", true);
        } 
        else
        {
            animator.SetBool("isRunning", false);
        }
        
        FlipTowardMouse();
    }

    void FlipTowardMouse()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(input.MousePosition);
    
        if (mousePos.x < transform.position.x)
        {
            transform.localScale = new Vector3(-1, 1, 1); 
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1); 
        }
    }
}