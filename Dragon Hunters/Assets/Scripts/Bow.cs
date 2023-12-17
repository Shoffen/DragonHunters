using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : MonoBehaviour
{
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public Animator bowAnimator;
    public Animator playerAnimator;
    public GameObject arrowPrefab;
    public Transform shootingDirection;
    private GameObject currentArrow;
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public float trajectoryPower;
    [SerializeField] private TrajectoryLine trajectoryLine;
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------


    // Start is called before the first frame update
    void Start()
    {
       
        //bowAnimator.Play("Load");
        //bowAnimator.speed = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        if (playerAnimator.GetFloat("DrawTension") > 0)
        {
            trajectoryLine.lineRenderer.gameObject.SetActive(true);
            trajectoryLine.ShowTrajectoryLine(shootingDirection.position, shootingDirection.forward * trajectoryPower * playerAnimator.GetFloat("DrawTension"));
        }
        else
        {
            trajectoryLine.lineRenderer.gameObject.SetActive(false);
        }
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
    }
    private void FixedUpdate()
    {
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        RotateArrowToFaceGround();
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
    }
    public void Fire(float chargedPower)
    {
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        Vector3 direction = new Vector3(shootingDirection.forward.x, shootingDirection.forward.y, 0.0148f);
        Quaternion arrowRotation = Quaternion.LookRotation(direction) * Quaternion.Euler(90f, 0f, 0f);
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        currentArrow = Instantiate(arrowPrefab, shootingDirection.position, arrowRotation);
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        Rigidbody rb = currentArrow.GetComponent<Rigidbody>();
        rb.AddForce(direction * trajectoryPower * chargedPower, ForceMode.Impulse);
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------


    }
    void RotateArrowToFaceGround()
    {
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        if (currentArrow != null)
        {
            Rigidbody rb = currentArrow.GetComponent<Rigidbody>();

            // Check if the arrow is falling (y velocity is negative)
            if (rb.velocity.y < 0)
            {
                // Check the initial direction of the arrow's velocity
                float upwardAngle = Vector3.Angle(Vector3.up, rb.velocity.normalized);

                // Set a threshold angle (you can adjust this value based on your needs)
                float rotationThreshold = 30f; // Example threshold of 30 degrees

                // Calculate the rotation based on velocity only if the upward angle is above the threshold
                if (upwardAngle > rotationThreshold)
                {
                    Quaternion rotation = Quaternion.Euler(-2.182f, -61.337f, -180f);
                    float rotationSpeed = 1.45f;
                    
                    
                    // Set the rotation of the arrow to always point in the direction of motion
                    currentArrow.transform.rotation = Quaternion.Slerp(currentArrow.transform.rotation, rotation, rotationSpeed * Time.fixedDeltaTime);
                }
            }
        }
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------


    }
    

}
