using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    private float smoothSpeed = 2f;
    void Start()
    {
    }
    void LateUpdate()
    {
        Vector3 desiredPosition = new Vector3(target.position.x + offset.x, transform.position.y, transform.position.z);

        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        transform.position = smoothedPosition;
    }
}
