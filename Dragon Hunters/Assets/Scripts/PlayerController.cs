using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private const float ACCELERATION = 12f;

    [SerializeField] private Animator animator;

    private Rigidbody myRigidbody;
    private InputManager playerInput;
    private float moveAnimationSpeedTarget;
    private float currentMoveX;

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
        walkingChange = playerInput.GetAxis(InputManager.AXIS.MOVE);
    }

    private void FixedUpdate()
    {
        UpdateMovement();
        UpdateRotation();
    }

    private void UpdateMovement()
    {
        currentMoveX = Mathf.Lerp(currentMoveX, walkingChange.x, Time.deltaTime * ACCELERATION);
        Vector3 movement = new Vector3(currentMoveX, 0, 0) * walkingSpeed * Time.fixedDeltaTime;
        moveAnimationSpeedTarget = Mathf.Lerp(moveAnimationSpeedTarget, walkingChange.magnitude, Time.deltaTime * (ACCELERATION / 2));
        animator.SetFloat("MoveSpeed", moveAnimationSpeedTarget);
        myRigidbody.MovePosition(myRigidbody.position + movement);
        myRigidbody.velocity = Physics.gravity;
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
