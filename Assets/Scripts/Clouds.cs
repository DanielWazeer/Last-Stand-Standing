
using UnityEngine;

public class Clouds : MonoBehaviour
{
    public float speed;
    void Start()
    {
    }

    void Update()
    {
        transform.position += Vector3.right * speed * Time.deltaTime;
    }
}
