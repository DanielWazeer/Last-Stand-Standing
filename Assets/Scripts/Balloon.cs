using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Balloon : MonoBehaviour
{
    public float speed;
    void Start()
    {
    }

    void Update()
    {
        transform.position += Vector3.up * speed * Time.deltaTime;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "BalTrig")
        {
            ResetBalloon();
        }
    }

    void ResetBalloon()
    {
        if(gameObject.tag == "Ground")
            Destroy(gameObject);
        else
            transform. position = new Vector2 (UnityEngine.Random.Range(-35, 35), UnityEngine.Random.Range(-20, -25));
    }
}
