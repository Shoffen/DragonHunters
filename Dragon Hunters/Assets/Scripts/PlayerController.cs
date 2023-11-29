using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody myRigidbody;
    public Animator animator;
    private InputManager playerInput;

    public float walkingSpeed;
    public float rotationSpeed;
    private Vector3 walkingChange;

    private void Start()
    {
        myRigidbody = GetComponent<Rigidbody>();
        playerInput = GetComponent<InputManager>();
    }

    private void Update()
    {
        GetInput();
    }

    private void GetInput()
    {
        //walkingChange = Vector3.zero;



        walkingChange = playerInput.GetAxis(InputManager.AXIS.MOVE);

        
        //Debug.Log(walkingChange);
        /*if (this.playerInput.ListenForClick(InputManager.PLAYER_ACTION.SHOOT))
        {
            // Handle shooting logic here
        }*/
    }

    private void FixedUpdate()
    {
        UpdateMovement();
        UpdateRotation();
    }

    private void UpdateMovement()
    {
        if (walkingChange != Vector3.zero)
        {
            animator.SetBool("idle", false);
            animator.SetBool("walking", true);
        }
        else
        {
            animator.SetBool("walking", false);
            animator.SetBool("idle", true);
        }

        walkingChange = Vector3.ClampMagnitude(walkingChange, 1f);
        Vector3 movement = walkingChange * walkingSpeed * Time.fixedDeltaTime;

        // Add movement to the current position
        myRigidbody.MovePosition(myRigidbody.position + movement);
    }

    private void UpdateRotation()
    {

        float targetRotationY = walkingChange.x * 90f;
        if (targetRotationY != 0)
        {
            //Debug.Log(targetRotationY);
            Quaternion targetRotation = Quaternion.Euler(0f, targetRotationY, 0f);
         // transform.rotation = targetRotation;

            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        //this.transform.rotation = Quaternion.Lerp(this.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

    }
    /*private void UpdateRotation()
    {
        float rotationInput = walkingChange.x;

        if (!Mathf.Approximately(rotationInput, 0.0f))
        {
            Vector3 rotationDirection = Vector3.Cross(Vector3.forward, heading);
            rotationAngle = Mathf.Sign(rotationInput) * Mathf.Rad2Deg * Mathf.Acos(Vector3.Dot(Vector3.forward, heading));

            // Apply rotation
            targetRotation = Quaternion.Euler(0, rotationAngle, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
    }*/
    /*private void UpdateMovement()
    {
        if (walkingChange != Vector3.zero)
        {
            animator.SetBool("idle", false);
            animator.SetBool("walking", true);
        }
        else
        {
            animator.SetBool("walking", false);
            animator.SetBool("idle", true);
        }

        walkingChange = Vector3.ClampMagnitude(walkingChange, 1f);
        Vector3 rightMovement = right * walkingSpeed * Time.fixedDeltaTime * walkingChange.x;

        // Only consider X-axis component for heading
        heading = Vector3.Normalize(new Vector3(rightMovement.x, 0.0f, 0.0f));

        myRigidbody.position += rightMovement;
    }*/


}
