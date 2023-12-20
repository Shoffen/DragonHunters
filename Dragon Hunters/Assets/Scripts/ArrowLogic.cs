using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ArrowLogic : MonoBehaviour
{
    private Rigidbody myBody;
    public BoxCollider[] colliders;
    private Transform headObject;
    public BlueDragonLogic blueDragonLogic;
    public int arrowDamage;
    public float groundRadius;
    public LayerMask groundLayer;
    public float rayLength;
    [SerializeField] private Animator playerAnimator;
    public float tension;

    private bool needToRotate;
    private bool enoughPower
    {
        get
        {
            return tension >= 0.55 ? true : false;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        myBody = GetComponent<Rigidbody>();
       
    }

    private void Update()
    {
        if(myBody.velocity.y < 0)
        {
            needToRotate = enoughPower ? false : true;
        }
    }

    private void FixedUpdate()
    {
        if (needToRotate)
        {
            RotateArrowToFaceGround();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Glass"))
        {
            
        }
        else if(other.CompareTag("Enemy"))
        {

            other.transform.root.gameObject.GetComponent<HitColliderData>().PassInHit(other, arrowDamage * tension);
            StickArrowToEnemy(other.transform);
            
        }
        else
        {
            myBody.isKinematic = true;
            foreach (BoxCollider collider in colliders)
            {
                collider.enabled = false;
            }
        }
    }
    private void StickArrowToEnemy(Transform enemyHead)
    {
       myBody.isKinematic = true;
       foreach (BoxCollider collider in colliders)
       {
           collider.enabled = false;
       }
       transform.parent = enemyHead;
    }
    private void RotateArrowToFaceGround()
    {
       
        // Check if the arrow is falling (y velocity is negative)
        if (myBody.velocity.y < 0)
        {
            // Check the initial direction of the arrow's velocity
            float upwardAngle = Vector3.Angle(Vector3.up, myBody.velocity.normalized);

            // Set a threshold angle (you can adjust this value based on your needs)
            float rotationThreshold = 30f; // Example threshold of 30 degrees

            // Calculate the rotation based on velocity only if the upward angle is above the threshold
            if (upwardAngle > rotationThreshold)
            {
                Quaternion rotation = Quaternion.Euler(-2.182f, -61.337f, -180f);
                float rotationSpeed = 1.45f;


                // Set the rotation of the arrow to always point in the direction of motion
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.fixedDeltaTime);
            }
        }
        
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------


    }
    /*private bool NeedToRotate()
    {
        //Cast a ray downward from the GameObject's position
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, rayLength))
        {
            // Get the normal of the surface the ray hits
            Vector3 groundNormal = hit.normal;

            // Calculate the angle between the GameObject's up vector and the ground normal
            float angleToGround = Vector3.Angle(transform.up, groundNormal);

            // Now you have the angle to the ground in degrees
            Debug.Log("Angle to Ground: " + angleToGround);

            // Visualize the ray for debugging
            Debug.DrawRay(transform.position, Vector3.down * rayLength, Color.green);

            if (playerAnimator.GetFloat("DrawTension") < 0.75f)
            {
                return true;
            }
            else
            {
                if (angleToGround <= 80f)
                {
                    return true;
                }
            }
            
        }

        return false;
    }*/








}
