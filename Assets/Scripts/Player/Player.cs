using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector2 movement;
    private Animator animator;

    public float moveSpeed = 5f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;              // uasually set gravityScale to 0 for top-down games
        rb.freezeRotation = true;         // freezeRotation avoid physics rotation

        animator = GetComponent<Animator>();
    }

    // FixedUpdate is called at a fixed interval and is independent of frame rate. Put physics code here.
    void FixedUpdate()
    {
        Vector2 newPos = rb.position + movement * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(newPos);
    }

    // move input action
    public void OnMove(InputAction.CallbackContext context)
    {
        animator.SetBool("isWalking", true);
        if (context.canceled)
        {
            animator.SetBool("isWalking", false);
            animator.SetFloat("LastInputX", movement.x);
            animator.SetFloat("LastInputY", movement.y);
        }

        movement = context.ReadValue<Vector2>();
        animator.SetFloat("InputX", movement.x);
        animator.SetFloat("InputY", movement.y);
    }

    // trigger when colliding with collectible items
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Collectible"))
        {
            Debug.Log("Collected an item!");
        }
    }

}
