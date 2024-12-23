using UnityEngine;
using UnityEngine.UI;

public class PlayerShoot : MonoBehaviour
{
    public GameObject bullet;
    public GameObject spawn;
    private float nextAttackTime;
    public float attackDelay;
    public static bool CanMove;
    public Image cooldownImage;
    public AudioSource shoot;
    private float cooldownTimer;
    private Animator anim;

    private bool isHoldingUp = false;
    private bool isHoldingDown = false;

    private void Start()
    {
        nextAttackTime = 0f;
        anim = GetComponent<Animator>();
        CanMove = true;
        cooldownImage.fillAmount = 1; // Start with the cooldown filled
    }

    private void Update()
    {
        if (Time.time >= nextAttackTime)
        {
            if (Input.GetKey(KeyCode.X))
            {
                Shooting();
                nextAttackTime = Time.time + attackDelay;
            }
        }
        if (cooldownTimer < attackDelay)
        {
            cooldownTimer += Time.deltaTime;
            UpdateCooldownUI();
        }
        if(Input.GetKey(KeyCode.UpArrow))
        {
            isHoldingUp = true;
            isHoldingDown = false;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            isHoldingDown = true;
            isHoldingUp = false;
        }
        else
        {
            isHoldingUp = false;
            isHoldingDown = false;
        }

    }

    public void AllowMovement()
    {
        CanMove = true;
    }

    void Shooting()
    {
        shoot.Play();
        StartCooldown();
        anim.SetTrigger("Throw");
        GameObject projectile = (GameObject)Instantiate(bullet, spawn.transform.position, Quaternion.identity);
        if (PlayerController.facingRight)
        {
            if (transform.tag == "PlayerBurger")
            {
                projectile.transform.right = transform.right;
                GameObject projectile2 = (GameObject)Instantiate(bullet, spawn.transform.position, Quaternion.identity);
                projectile2.transform.right = transform.right + transform.up / 3f;
                GameObject projectile3 = (GameObject)Instantiate(bullet, spawn.transform.position, Quaternion.identity);
                projectile3.transform.right = transform.right - transform.up / 3f;
            }
            else if (transform.tag == "PlayerCandy")
            {
                if(isHoldingUp)
                {
                    projectile.transform.right = transform.right;
                    GameObject projectile2 = (GameObject)Instantiate(bullet, spawn.transform.position, Quaternion.identity);
                    projectile2.transform.right = transform.right - transform.up/6;
                }
                else if(isHoldingDown)
                {
                    projectile.transform.right = transform.right;
                    GameObject projectile2 = (GameObject)Instantiate(bullet, spawn.transform.position, Quaternion.identity);
                    projectile2.transform.right = transform.right + transform.up/6;
                }
                else
                {
                    projectile.transform.right = transform.up + transform.right;
                    GameObject projectile2 = (GameObject)Instantiate(bullet, spawn.transform.position, Quaternion.identity);
                    projectile2.transform.right = transform.up/2 + transform.right ;
                }
            }
            else
            {
                projectile.transform.right = transform.right;
            }
        }
        else //facing left
        {
            if (transform.tag == "PlayerBurger")
            {
                projectile.transform.right = -transform.right;
                if (isHoldingUp)
                {
                    GameObject projectile2 = (GameObject)Instantiate(bullet, spawn.transform.position, Quaternion.identity);
                    projectile2.transform.right = transform.right + transform.up / 3f;
                    GameObject projectile3 = (GameObject)Instantiate(bullet, spawn.transform.position, Quaternion.identity);
                    projectile3.transform.right = transform.right - transform.up / 3f;
                }
                else if (isHoldingDown)
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
            else if (transform.tag == "PlayerCandy")
            {
                if (isHoldingUp)
                {
                    projectile.transform.right = transform.right;
                    GameObject projectile2 = (GameObject)Instantiate(bullet, spawn.transform.position, Quaternion.identity);
                    projectile2.transform.right = transform.right + transform.up/6;
                }
                else if (isHoldingDown)
                {
                    projectile.transform.right = transform.right;
                    GameObject projectile2 = (GameObject)Instantiate(bullet, spawn.transform.position, Quaternion.identity);
                    projectile2.transform.right = transform.right - transform.up/6;
                }
                else
                {
                    projectile.transform.right = transform.up - transform.right;
                    GameObject projectile2 = (GameObject)Instantiate(bullet, spawn.transform.position, Quaternion.identity);
                    projectile2.transform.right = transform.up/2 - transform.right;
                }
            }
            else
            {
                projectile.transform.right = -transform.right;
            }
        }
    }

    public void StartCooldown()
    {
        cooldownTimer = 0;
        UpdateCooldownUI();
    }

    private void UpdateCooldownUI()
    {
        float fillAmount = Mathf.Clamp01(cooldownTimer / attackDelay);
        cooldownImage.fillAmount = fillAmount;
    }

    public bool CanAttack()
    {
        return cooldownTimer >= attackDelay;
    }
}
