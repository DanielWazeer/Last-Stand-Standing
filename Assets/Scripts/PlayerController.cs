using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D rb;
    public BoxCollider2D playerCollider;
    private float moveInput;
    public float moveSpeed;
    public float jumpForce;
    public static bool facingRight = false;
    [SerializeField] private LayerMask groundMask;

    public static int unDefeated;
    public AudioSource jump;
    public AudioSource walk;
    private Animator anim;
    Vector2 vecGravity;

    [Header("Jump System")]
    public float jumpTime;
    public float jumpMult;
    private float jumpCounter;
    private bool isJumping;
    private float coyoteTime = 0.15f;
    private float coyoteCounter;
    private float? jumpPressedTime;
    private float? jumpReleasedTime;
    private float jumpGracePeriod = 0.1f;
    private float nextJumpTime;

    [Header("Dash")]
    private bool canDash = true;
    public bool isDashing;
    public float dashPower;
    private float dashTime = 0.1f;
    private float nextDashTime;
    public float dashDelay;
    public Image cooldownImage;
    public AudioSource dash;
    private float cooldownTimer;

    // Reference to the Canvas object
    public GameObject playerCanvas;

    void Start()
    {
        playerCollider = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        vecGravity = new Vector2(0, -Physics2D.gravity.y);
        nextJumpTime = 0f;
        nextDashTime = 0f;
        cooldownImage.fillAmount = 1;
        facingRight = false;
    }

    void Update()
    {
        if (isDashing) { return; }
        Flip(); 
        if (PlayerShoot.CanMove)
        {
            Move();
            HandleJump();
            HandleGravity();
        }
        else
        {
            //moveInput = 0;
            //rb.velocity = Vector2.zero;
        }
        if (isGrounded())
        {
            coyoteCounter = coyoteTime;
        }
        else
        {
            coyoteCounter -= Time.deltaTime;
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            jumpPressedTime = Time.time;
        }
        else if (Input.GetKeyUp(KeyCode.Z))
        {
            jumpReleasedTime = Time.time;
            coyoteCounter = 0f;
            isJumping = false;
            jumpCounter = 0;
            if (rb.velocity.y > 0f)
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.6f);
            }
        }
        if (Input.GetKeyDown(KeyCode.C) && canDash && Time.time >= nextDashTime)
        {
            StartCoroutine(Dash());
            StartCooldown();
            nextDashTime = Time.time + dashDelay;
        }
        if (cooldownTimer < dashDelay)
        {
            cooldownTimer += Time.deltaTime;
            UpdateCooldownUI();
        }
    }

    public void StartCooldown()
    {
        cooldownTimer = 0;
        UpdateCooldownUI();
    }

    private void UpdateCooldownUI()
    {
        float fillAmount = Mathf.Clamp01(cooldownTimer / dashDelay);
        cooldownImage.fillAmount = fillAmount;
    }

    void Move()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
        if (moveInput != 0)
        {
            anim.SetBool("Walk", true);
            if (!walk.isPlaying && isGrounded())
                walk.Play();
        }
        else
        {
            anim.SetBool("Walk", false);
            walk.Stop();
        }
    }

    private IEnumerator Dash()
    {
        dash.Play();
        canDash = false;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.velocity = new Vector2(transform.localScale.x * dashPower, 0f);
        rb.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        yield return new WaitForSeconds(dashTime);
        rb.gravityScale = originalGravity;
        isDashing = false;
        canDash = true;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        yield return null;
    }

    private void FixedUpdate()
    {
        if (isDashing) { return; }
        if(PlayerShoot.CanMove)
            rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
    }

    void HandleGravity()
    {
        if (rb.velocity.y < -0.01f)
        {
            rb.gravityScale = 12f;
            rb.velocity -= vecGravity * 6 * Time.deltaTime;
            anim.SetBool("Fall", true);
            anim.SetBool("Jump", false);
        }
        else if (rb.velocity.y > 0.01f)
        {
            rb.gravityScale = 6f;
            anim.SetBool("Jump", true);
            anim.SetBool("Fall", false);
            if (isJumping)
            {
                jumpCounter += Time.deltaTime;
                if (jumpCounter > jumpTime) isJumping = false;
                float t = jumpCounter / jumpTime;
                float currentJumpM = jumpMult;
                if (t > 0.5f)
                {
                    currentJumpM = jumpMult * (1 - t);
                }
                rb.velocity += vecGravity * jumpMult * Time.deltaTime;
            }
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

            // Reset the canvas rotation
            if (playerCanvas != null)
            {
                playerCanvas.transform.localScale= localScale;
            }
        }
    }

    void HandleJump()
    {
        if (Time.time >= nextJumpTime && coyoteCounter > 0f && (Time.time - jumpPressedTime <= jumpGracePeriod))
        {
            nextJumpTime = Time.time + 0.15f;
            jumpPressedTime = null;
            rb.velocity = Vector2.up * jumpForce;
            jump.Play();
            isJumping = true;
            jumpCounter = 0;
            if (Time.time - jumpReleasedTime <= jumpGracePeriod)
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.6f);
                jumpReleasedTime = null;
            }
        }
        else if (!isGrounded() && Input.GetKeyDown(KeyCode.DownArrow))
        {
            rb.velocity += -Vector2.up * jumpForce * 2;
            jump.Play();
        }
    }

    public bool isGrounded()
    {
        RaycastHit2D hit = Physics2D.BoxCast(playerCollider.bounds.center, playerCollider.bounds.size, 0f, Vector2.down, 1f, groundMask);
        return hit.collider != null;
    }
}
