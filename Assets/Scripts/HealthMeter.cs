using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class HealthMeter : MonoBehaviour
{
    [SerializeField] private GameObject Panel;

    public TMP_Text resultTMP;
    public float maxValue;
    public float currentValue = 100f;
    [SerializeField] private GameObject healthSlider;
    string myName;
    Slider slider;
    string result;
    public bool night;
    public static int lives;
    public bool hasCam;
    public AudioSource hurt;

    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
        Time.timeScale = 1;
        int myLayer = gameObject.layer;
        myName = LayerMask.LayerToName(myLayer);
        Panel.SetActive(false);
        currentValue = maxValue;
        slider = healthSlider.GetComponent<Slider>();
        lives = 3;
    }

    public void TakeDamage(float damage)
    {
        anim.SetTrigger("Hit");
        currentValue -= damage;
        currentValue = Mathf.Clamp(currentValue, 0f, maxValue);
        slider.value = currentValue;
        if(myName == "Player")
        {
            PlayerShoot.CanMove = false;
            AllowMovement();
        }
    }
    public void AllowMovement()
    {
        PlayerShoot.CanMove = true;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        int collLayer = collision.gameObject.layer;
        string collName = LayerMask.LayerToName(collLayer);
        if((collName == "EnemyBullet" && myName == "Player") || (collName == "PlayerBullet" && myName == "Enemy"))
        {
            Destroy(collision.gameObject);
            TakeDamage(10);
            hurt.Play();
            if(currentValue <= 0f)
            {
                if (myName == "Player" && (!night || lives <= 0 || hasCam))
                {
                    Panel.SetActive(true);
                    result = "You Lose!";
                    resultTMP.text = result;
                    Time.timeScale = 0f;
                }
                else if (myName == "Player" && night)
                {
                    lives = Utility.FindGameObjectsInLayer(8).Length;
                    if (lives > 0)
                        Destroy(gameObject);
                }

                else if(myName == "Enemy")
                {
                    PlayerController.unDefeated = Utility.FindGameObjectsInLayer(6).Length;
                    if (PlayerController.unDefeated <= 1)
                    {
                        Panel.SetActive(true);
                        result = "You are the LAST STAND STANDING!";
                        resultTMP.text = result;
                        Time.timeScale = 0f;
                    }
                    Destroy(gameObject);
                }   
            }
        }
    }
}
