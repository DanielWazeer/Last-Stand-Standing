
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D rb;
    public BoxCollider2D playerCollider;
    private float moveInput;
    public float moveSpeed;
    public float jumpForce;
    public bool facingRight = false;
    [SerializeField] private LayerMask groundMask;

    public static int unDefeated;
    public AudioSource jump;

    public AudioSource walk;
    private Animator anim;
    void Start()
    {
        playerCollider = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        HandleGravity();
        if (PlayerShoot.CanMove)
        {
            Move();
            HandleJump();
        }
        else
        {
            moveInput = 0;
        }
    }
    void Move()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
        if(moveInput != 0 )
        {
            anim.SetBool("Walk", true);
            if(!walk.isPlaying && isGrounded())
                walk.Play();
        }
        else
        {
            anim.SetBool("Walk", false);
            walk.Stop();
        }
        Flip();
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
    }
    void HandleGravity()
    {
        if (rb.velocity.y < 1f)
            rb.gravityScale = 10f;
        else rb.gravityScale = 4f;

        if (rb.velocity.y > 0.1f)
        {
            anim.SetBool("Jump", true);
            anim.SetBool("Fall", false);
        }
            
        else if (rb.velocity.y < -0.1f)
        {
            anim.SetBool("Fall", true);
            anim.SetBool("Jump", false);
        }
            
        else
        {
            anim.SetBool("Fall", false);
            anim.SetBool("Jump", false);
        }

    }
    void Flip()
    {
        if (facingRight && moveInput < 0 || !facingRight && moveInput > 0)
        {
            Vector3 localScale = transform.localScale;
            facingRight = !facingRight;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
        
    }
    void HandleJump()
    {
        if (isGrounded() && Input.GetKeyDown(KeyCode.UpArrow))
        {
            rb.velocity = Vector2.up * jumpForce;
            jump.Play();
        }
        else if(!isGrounded() && Input.GetKeyDown(KeyCode.DownArrow))
        {
            rb.velocity = -Vector2.up * jumpForce;
            jump.Play();
        }
    }
    public bool isGrounded()
    {
        RaycastHit2D hit = Physics2D.BoxCast(playerCollider.bounds.center, playerCollider.bounds.size, 0f, Vector2.down, 1f, groundMask);
        return hit.collider != null;
    }

}
