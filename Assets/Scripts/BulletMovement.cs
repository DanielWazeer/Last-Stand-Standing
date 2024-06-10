using System.Collections;
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
    void Start()
    {
        int myLayer = gameObject.layer;
        myLayerName = LayerMask.LayerToName(myLayer);
        if (myLayerName == "EnemyBullet")
            target = FindObjectOfType<PlayerController>().transform;
        else target = GetClosestEnemy();
        follow = false;
    }
    void Update()
    {
        if (transform.position.y > 4 && target != null)
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
            transform.Translate(Vector2.right * bulletSpeed * Time.deltaTime);
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
        if(collision.gameObject.tag == "Ground")
            Destroy(gameObject);
    }
}
