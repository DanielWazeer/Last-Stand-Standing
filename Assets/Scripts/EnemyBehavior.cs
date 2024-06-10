using System.Collections;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    #region Enemy Variables
    private Rigidbody2D rb;
    private BoxCollider2D coll;
    private GameObject player;
    private bool inAction;
    #endregion

    #region Behavior Variables
    public GameObject bullet;
    public GameObject spawn;
    public float attackSpeed;
    private float nextAttackTime;
    private float nextJumpTime;
    private bool facingRight = true;
    public float speed;
    private int move;
    public float jumpPower;
    private bool canMove = true;
    [SerializeField] private LayerMask groundMask;
    #endregion

    #region State Time
    private float stateTime = 1;
    [SerializeField] private float idleTime;
    [SerializeField] private float walkTime;
    [SerializeField] private float jumpTime;
    [SerializeField] private float attack1Time;
    #endregion

    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        player = FindObjectOfType<PlayerController>().gameObject;
        coll = GetComponent<BoxCollider2D>();
        nextAttackTime = 0f;
        nextJumpTime = 0f;
        enemyState = EnemyState.Idle;
        StartCoroutine(ChangeState());
    }
    public enum EnemyState
    {
        Idle,
        Walk,
        Jump,
        Attack1
    }
    public EnemyState enemyState;

    IEnumerator ChangeState()
    {
        yield return new WaitForSeconds(0.1f);
        enemyState = (EnemyState)Random.Range(0, 4);
        yield return new WaitForSeconds(stateTime);
        StartCoroutine(ChangeState());
    }
    void Update()
    {
        HandleGravity();
        Flip();
        if(rb.velocity.x != 0f)
        {
            anim.SetBool("Walk", true); ;
        }
        else
        {
            anim.SetBool("Walk", false);
        }
        switch (enemyState)
        {
            case EnemyState.Idle:
                if (!inAction)
                {
                    move = 0;
                    stateTime = idleTime;
                }
                break;
            case EnemyState.Walk:
                if (!inAction)
                {
                    if (canMove)
                        Move();
                    else move = 0;
                    stateTime = walkTime;
                }
                break;
            case EnemyState.Jump:
                if (!inAction)
                {
                    if (Time.time >= nextJumpTime)
                    {
                        if (isGrounded() && canMove)
                        {
                            rb.velocity = Vector2.up * jumpPower;
                            nextJumpTime = Time.time + 1f;
                            stateTime = jumpTime;
                        }
                    }
                }
                break;
            case EnemyState.Attack1:
                if (!inAction)
                {
                    if (Time.time >= nextAttackTime)
                    {
                        StartCoroutine(Attack1());
                    }
                }
                break;
        }
    }
    IEnumerator Attack1()
    {
        Shooting();
        inAction = true;
        stateTime = attack1Time;
        canMove = false;
        nextAttackTime = Time.time + stateTime;
        move = 0;
        yield return new WaitForSeconds(stateTime);
        canMove = true;
        inAction = false;
    }

    void Move()
    {
        inAction = false;
        if (player.transform.position.x < transform.position.x)
        {
            move = -1;
        }
        else
        {
            move = 1;
        }

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
    void FixedUpdate()
    {
        rb.velocity = new Vector2((move * speed), rb.velocity.y);
    }
    public IEnumerator IsHit()
    {
        inAction = true;
        enemyState = EnemyState.Idle;
        canMove = false;
        move = 0;
        yield return new WaitForSeconds(0.1f);
        canMove = true;
        inAction = false;
    }
    void Flip()
    {
        if (facingRight && player.transform.position.x < transform.position.x - 1 || !facingRight && player.transform.position.x > transform.position.x + 1)
        {
            Vector3 localScale = transform.localScale;
            facingRight = !facingRight;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }
    private bool isGrounded()
    {
        RaycastHit2D hit = Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, 1f, groundMask);
        return hit.collider != null;
    }

    void Shooting()
    {
        anim.SetTrigger("Throw");
        GameObject projectile = (GameObject)Instantiate(bullet, spawn.transform.position, Quaternion.identity);
        if (facingRight)
        {
            if (transform.tag == "EnemyBurger")
            {
                projectile.transform.right = transform.right;
                GameObject projectile2 = (GameObject)Instantiate(bullet, spawn.transform.position, Quaternion.identity);
                projectile2.transform.right = transform.right + transform.up / 4f;
                GameObject projectile3 = (GameObject)Instantiate(bullet, spawn.transform.position, Quaternion.identity);
                projectile3.transform.right = transform.right - transform.up / 4f;
            }
            else if (transform.tag == "EnemyCandy")
            {
                projectile.transform.right = transform.up;
                GameObject projectile2 = (GameObject)Instantiate(bullet, spawn.transform.position, Quaternion.identity);
                projectile2.transform.right = transform.right / 3 + transform.up;
            }
            else
                projectile.transform.right = transform.right;
        }
        else
        {
            if (transform.tag == "EnemyBurger")
            {
                projectile.transform.right = -transform.right;
                GameObject projectile2 = (GameObject)Instantiate(bullet, spawn.transform.position, Quaternion.identity);
                projectile2.transform.right = -transform.right + transform.up / 4f;
                GameObject projectile3 = (GameObject)Instantiate(bullet, spawn.transform.position, Quaternion.identity);
                projectile3.transform.right = -transform.right - transform.up / 4f;
            }
            else if (transform.tag == "EnemyCandy")
            {
                projectile.transform.right = transform.up;
                GameObject projectile2 = (GameObject)Instantiate(bullet, spawn.transform.position, Quaternion.identity);
                projectile2.transform.right = -transform.right / 3 + transform.up;
            }
            else
                projectile.transform.right = -transform.right;
        }

    }
}
