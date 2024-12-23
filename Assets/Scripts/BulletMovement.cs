using System.Collections.Generic;
using UnityEngine;
public static class Utility
{
    public static GameObject[] FindGameObjectsInLayer(int layer)
    {
        List<GameObject> gameObjectsInLayer = new List<GameObject>();

        foreach (GameObject obj in GameObject.FindObjectsOfType<GameObject>())
        {
            if (obj.layer == layer)
            {
                gameObjectsInLayer.Add(obj);
            }
        }

        return gameObjectsInLayer.ToArray();
    }
}

public class BulletMovement : MonoBehaviour
{
    public float bulletSpeed;
    public float bulletLife;
    private Transform target;
    private bool follow;
    string myLayerName;
    Vector3 direction;
    private Vector3 mypos;
    Vector2 dir;
    private PlayerController player;

    private bool bullRight = true;

    void Flip()
    {
        if (bullRight && player.transform.position.x < transform.position.x - 1 || !bullRight && player.transform.position.x > transform.position.x + 1)
        {
            bullRight = !bullRight;
        }
    }
    void Start()
    {
        int myLayer = gameObject.layer;
        myLayerName = LayerMask.LayerToName(myLayer);
        player = FindObjectOfType<PlayerController>();
        follow = false;
        mypos = gameObject.transform.position;
        Flip();
        if (myLayerName == "EnemyBullet")
        {
            target = player.transform;
            if(EnemyBehavior.upBullet == true && bullRight)
            {
                dir = Vector2.up;
            }
            else if(EnemyBehavior.upBullet == true && !bullRight)
            {
                dir = -Vector2.up;
            }
            else
            {
                dir = Vector2.right;
            }
        }
        else if (myLayerName == "PlayerBullet")
        {
            target = GetClosestEnemy();
            if (Input.GetKey(KeyCode.UpArrow))
            {
                dir = Vector2.up;
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                dir = Vector2.down;
            }
            else
            {
                dir = Vector2.right;
            }
        }
    }
    void Update()
    {
        if (transform.position.y > mypos.y + 4 && target != null)
        {
            direction = (target.position - transform.position).normalized;
            follow = true;
        }
        if(follow && target != null && transform.tag == "Candy")
        {
            transform.position += direction * bulletSpeed * Time.deltaTime;
        }
        else
        {
            transform.Translate(dir * bulletSpeed * Time.deltaTime);
        }
        Destroy(gameObject, bulletLife);
    }
    public int enemyLayer;
    Transform GetClosestEnemy()
    {
        enemyLayer = LayerMask.NameToLayer("Enemy");
        GameObject[] enemies = Utility.FindGameObjectsInLayer(enemyLayer);
        Transform closestEnemy = null;
        float minDistance = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(currentPosition, enemy.transform.position);
            if (distance < minDistance)
            {
                closestEnemy = enemy.transform;
                minDistance = distance;
            }
        }
        return closestEnemy;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((collision.gameObject.tag == "Ground") || (gameObject.layer == 7 && collision.gameObject.layer == 9) || (gameObject.layer == 9 && collision.gameObject.layer == 7))
            Destroy(gameObject);
    }
}
