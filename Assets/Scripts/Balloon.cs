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
            transform. position = new Vector2 (UnityEngine.Random.Range(-40, 40), UnityEngine.Random.Range(-30, -35));
    }
}
