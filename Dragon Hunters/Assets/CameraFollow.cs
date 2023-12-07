using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
   
    private Vector3 offset;
    public float smoothing = 5f;
    public float fixedYCoordinate = 1f;
    [SerializeField] private Transform target;

    private void Start()
    {
        offset = transform.position - target.position;
    }
    private void FixedUpdate()
    {
        Vector3 targetCamPos = new Vector3(target.position.x, fixedYCoordinate, target.position.z)  + offset;
        transform.position = Vector3.Lerp(transform.position, targetCamPos, smoothing * Time.deltaTime);
    }
   
}
