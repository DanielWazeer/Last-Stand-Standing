using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;


public class PlayerShoot : MonoBehaviour
{
    public GameObject bullet;
    public GameObject spawn;
    private float nextAttackTime;
    public float attackSpeed;
    private PlayerController playerController;
    public static bool CanMove;
    public Image cooldownImage; // Assign this in the Inspector
    public AudioSource shoot;
    private float cooldownTimer;
    private Animator anim;

    private void Start()
    {
        playerController = GetComponent<PlayerController>();
        nextAttackTime = 0f;
        anim = GetComponent<Animator>();
        CanMove = true;
    }
    private void Update()
    {
        if (Time.time >= nextAttackTime)
        {
            if (Input.GetKey(KeyCode.X))
            {
                Shooting();
                nextAttackTime = Time.time + attackSpeed;
                UnityEngine.Debug.Log(nextAttackTime);
            }
        }
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
            UpdateCooldownUI();
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
        CanMove = false;
        anim.SetTrigger("Throw");
        GameObject projectile = (GameObject)Instantiate(bullet, spawn.transform.position, Quaternion.identity);
        if(playerController.facingRight)
        {
            if(transform.tag == "PlayerBurger")
            {
                projectile.transform.right = transform.right;
                GameObject projectile2 = (GameObject)Instantiate(bullet, spawn.transform.position, Quaternion.identity);
                projectile2.transform.right = transform.right + transform.up / 3f;
                GameObject projectile3 = (GameObject)Instantiate(bullet, spawn.transform.position, Quaternion.identity);
                projectile3.transform.right = transform.right - transform.up / 3f;
            }
            else if (transform.tag == "PlayerCandy")
            {
                projectile.transform.right = transform.up;
                GameObject projectile2 = (GameObject)Instantiate(bullet, spawn.transform.position, Quaternion.identity);
                projectile2.transform.right = transform.right/2 + transform.up;
            }
            else
                projectile.transform.right = transform.right;
        }
        else
        {
            if (transform.tag == "PlayerBurger")
            {
                projectile.transform.right = -transform.right;
                GameObject projectile2 = (GameObject)Instantiate(bullet, spawn.transform.position, Quaternion.identity);
                projectile2.transform.right = -transform.right + transform.up / 3f;
                GameObject projectile3 = (GameObject)Instantiate(bullet, spawn.transform.position, Quaternion.identity);
                projectile3.transform.right = -transform.right - transform.up / 3f;
            }
            else if (transform.tag == "PlayerCandy")
            {
                projectile.transform.right = transform.up;
                GameObject projectile2 = (GameObject)Instantiate(bullet, spawn.transform.position, Quaternion.identity);
                projectile2.transform.right = -transform.right/2 + transform.up;
            }
            else
                projectile.transform.right = -transform.right;
        }
        AllowMovement();
            
    }
    public void StartCooldown()
    {
        cooldownTimer = attackSpeed;
        UpdateCooldownUI();
    }

    private void UpdateCooldownUI()
    {
        float fillAmount = Mathf.Clamp01(cooldownTimer / attackSpeed);
        cooldownImage.fillAmount = fillAmount;
    }

    public bool CanAttack()
    {
        return cooldownTimer <= 0;
    }
}