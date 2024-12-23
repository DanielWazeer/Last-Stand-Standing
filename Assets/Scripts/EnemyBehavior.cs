using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    private float nextAttackTime;
    private float nextJumpTime;
    public bool enfacingRight = true;
    public float speed;
    private int move;
    public float jumpPower;
    private bool enemyMove = true;
    private int hitMult;
    public bool dashCan;
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
    public static bool upBullet;
    public static bool downBullet;


    [Header("Dash")]
    private bool canDash = true;
    public bool isDashing;
    public float dashPower;
    private float dashTime = 0.1f;
    private float nextDashTime;
    public float dashDelay;
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
        enfacingRight = true;
        hitMult = 1;
    }
    public enum EnemyState
    {
        Idle,
        Walk,
        Jump,
        Dash,
        Attack1,
        Attack2,
        Attack3
    }
    public EnemyState enemyState;

    IEnumerator ChangeState()
    {
        yield return new WaitForSeconds(0.2f);
        enemyState = (EnemyState)Random.Range(0, 7);
        yield return new WaitForSeconds(stateTime);
        StartCoroutine(ChangeState());
    }
    void Update()
    {
        if (isDashing)
        {
            return;
        }
        Flip();
        if(rb.velocity.x != 0f)
        {
            anim.SetBool("Walk", true); ;
        }
        else
        {
            anim.SetBool("Walk", false);
        }
        if(enemyMove && (transform.position.x < 24 && transform.position.x > -24))
        {
            HandleGravity();
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
                        if (enemyMove)
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
                            if (isGrounded() && enemyMove)
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
                case EnemyState.Attack2:
                    if (!inAction && transform.position.y < (player.transform.position.y))
                    {
                        if (Time.time >= nextAttackTime)
                        {
                            StartCoroutine(Attack2());
                        }
                    }
                    break;
                case EnemyState.Attack3:
                    if (!inAction && transform.position.y > (player.transform.position.y + 0.5f))
                    {
                        if (Time.time >= nextAttackTime)
                        {
                            StartCoroutine(Attack3());
                        }
                    }
                    break;
                case EnemyState.Dash:
                    if (!inAction && dashCan)
                    {
                        if (canDash && Time.time >= nextDashTime)
                        {
                            StartCoroutine(Dash());
                            nextDashTime = Time.time + dashDelay;
                        }
                    }
                    break;
            }
        }
        else if(enemyMove)
        {
            if((transform.position.x > 24f && !enfacingRight) || (transform.position.x < -24f && enfacingRight))
            {
                enemyState = (EnemyState)Random.Range(1, 3);
                switch (enemyState)
                {
                    case EnemyState.Walk:
                        if (!inAction)
                        {
                            if (enemyMove)
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
                                if (isGrounded() && enemyMove)
                                {
                                    rb.velocity = Vector2.up * jumpPower;
                                    nextJumpTime = Time.time + 1f;
                                    stateTime = jumpTime;
                                }
                            }
                        }
                        break;
                    case EnemyState.Dash:
                        if (!inAction && dashCan)
                        {
                            if (canDash && Time.time >= nextDashTime)
                            {
                                StartCoroutine(Dash());
                                nextDashTime = Time.time + dashDelay;
                            }
                        }
                        break;
                }
            }
        }
    }
    IEnumerator Attack1()
    {
        upBullet = false;
        downBullet = false;
        Shooting();
        inAction = true;
        stateTime = attack1Time;
        enemyMove = false;
        nextAttackTime = Time.time + stateTime;
        move = 0;
        yield return new WaitForSeconds(stateTime);
        enemyMove = true;
        inAction = false;
    }
    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.velocity = new Vector2(transform.localScale.x * dashPower, 0f);
        rb.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        inAction = true;
        enemyMove = false;
        move = 0;
        yield return new WaitForSeconds(dashTime);
        rb.gravityScale = originalGravity;
        isDashing = false;
        canDash = true;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        enemyMove = true;
        inAction = false;
    }
    IEnumerator Attack2()
    {
        upBullet = true;
        downBullet = false;
        Shooting();
        inAction = true;
        stateTime = attack1Time;
        enemyMove = false;
        nextAttackTime = Time.time + stateTime;
        move = 0;
        yield return new WaitForSeconds(stateTime);
        enemyMove = true;
        inAction = false;
    }
    IEnumerator Attack3()
    {
        upBullet = false;
        downBullet = true;
        Shooting();
        inAction = true;
        stateTime = attack1Time;
        enemyMove = false;
        nextAttackTime = Time.time + stateTime;
        move = 0;
        yield return new WaitForSeconds(stateTime);
        enemyMove = true;
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
        else rb.gravityScale = 5f;

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
        if (isDashing)
        {
            return;
        }
        else if(enemyMove)
            rb.velocity = new Vector2((move * speed), rb.velocity.y);
    }
    public IEnumerator IsHit()
    {
        inAction = true;
        enemyState = EnemyState.Idle;
        enemyMove = false;
        move = 0;
        hitMult += 5;
        if(enfacingRight)
            rb.velocity = (Vector2.left + Vector2.up/2) * hitMult;
        else
            rb.velocity = (Vector2.right + Vector2.up / 2) * hitMult;
        yield return new WaitForSeconds(0.1f + hitMult * 0.001f);
        enemyMove = true;
        inAction = false;
    }
    void Flip()
    {
        if (enfacingRight && player.transform.position.x < transform.position.x - 4 || !enfacingRight && player.transform.position.x > transform.position.x + 4)
        {
            Vector3 localScale = transform.localScale;
            enfacingRight = !enfacingRight;
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
        if (enfacingRight)
        {
            if (transform.tag == "EnemyBurger")
            {
                projectile.transform.right = transform.right;
                GameObject projectile2 = (GameObject)Instantiate(bullet, spawn.transform.position, Quaternion.identity);
                projectile2.transform.right = transform.right + transform.up / 3f;
                GameObject projectile3 = (GameObject)Instantiate(bullet, spawn.transform.position, Quaternion.identity);
                projectile3.transform.right = transform.right - transform.up / 3f;
            }
            else if (transform.tag == "EnemyCandy")
            {
                if (enemyState == EnemyState.Attack2)
                {
                    projectile.transform.right = transform.right;
                    GameObject projectile2 = (GameObject)Instantiate(bullet, spawn.transform.position, Quaternion.identity);
                    projectile2.transform.right = transform.right - transform.up / 6;
                }
                else if (enemyState == EnemyState.Attack3)
                {
                    projectile.transform.right = transform.right;
                    GameObject projectile2 = (GameObject)Instantiate(bullet, spawn.transform.position, Quaternion.identity);
                    projectile2.transform.right = transform.right + transform.up / 6;
                }
                else
                {
                    projectile.transform.right = transform.up + transform.right;
                    GameObject projectile2 = (GameObject)Instantiate(bullet, spawn.transform.position, Quaternion.identity);
                    projectile2.transform.right = transform.up / 2 + transform.right;
                }
            }
            else
            {
                projectile.transform.right = transform.right;
            }
        }
        else //facing left
        {
            if (transform.tag == "EnemyBurger")
            {
                projectile.transform.right = -transform.right;
                if (enemyState == EnemyState.Attack2)
                {
                    GameObject projectile2 = (GameObject)Instantiate(bullet, spawn.transform.position, Quaternion.identity);
                    projectile2.transform.right = transform.right + transform.up / 3f;
                    GameObject projectile3 = (GameObject)Instantiate(bullet, spawn.transform.position, Quaternion.identity);
                    projectile3.transform.right = transform.right - transform.up / 3f;
                }
                else if (enemyState == EnemyState.Attack3)
                {
                    GameObject projectile2 = (GameObject)Instantiate(bullet, spawn.transform.position, Quaternion.identity);
                    projectile2.transform.right = transform.right + transform.up / 3f;
                    GameObject projectile3 = (GameObject)Instantiate(bullet, spawn.transform.position, Quaternion.identity);
                    projectile3.transform.right = transform.right - transform.up / 3f;
                }
                else //not holding
                {
                    GameObject projectile2 = (GameObject)Instantiate(bullet, spawn.transform.position, Quaternion.identity);
                    projectile2.transform.right = -transform.right + transform.up / 3f;
                    GameObject projectile3 = (GameObject)Instantiate(bullet, spawn.transform.position, Quaternion.identity);
                    projectile3.transform.right = -transform.right - transform.up / 3f;
                }
            }
            else if (transform.tag == "EnemyCandy")
            {
                if (enemyState == EnemyState.Attack2)
                {
                    projectile.transform.right = transform.right;
                    GameObject projectile2 = (GameObject)Instantiate(bullet, spawn.transform.position, Quaternion.identity);
                    projectile2.transform.right = transform.right + transform.up / 6;
                }
                else if (enemyState == EnemyState.Attack3)
                {
                    projectile.transform.right = transform.right;
                    GameObject projectile2 = (GameObject)Instantiate(bullet, spawn.transform.position, Quaternion.identity);
                    projectile2.transform.right = transform.right - transform.up / 6;
                }
                else
                {
                    projectile.transform.right = transform.up - transform.right;
                    GameObject projectile2 = (GameObject)Instantiate(bullet, spawn.transform.position, Quaternion.identity);
                    projectile2.transform.right = transform.up / 2 - transform.right;
                }
            }
            else
            {
                projectile.transform.right = -transform.right;
            }
        }
    }
}
