using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody myRigidbody;
    public Animator animator;
    private PlayerInput playerInput;

    public float walkingSpeed;
    private Vector3 walkingChange;
    private Vector3 forward, right;
    private Vector3 heading;

    public float rotationSpeed;
    private float rotationAngle;
    private Quaternion targetRotation;

    private void Start()
    {
        myRigidbody = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();

        forward = Camera.main.transform.forward;
        forward.y = 0;
        forward = Vector3.Normalize(forward);
        right = Quaternion.Euler(new Vector3(0, 90, 0)) * forward;

    }

    private void Update()
    {
        GetInput();
    }

    private void GetInput()
    {
        walkingChange = Vector3.zero;

        if (this.playerInput.ListenForHeldDown(PlayerInput.PLAYER_ACTION.GO_LEFT))
        {
            walkingChange = new Vector3(-1.0f, 0.0f, 0.0f);
        }

        if (this.playerInput.ListenForHeldDown(PlayerInput.PLAYER_ACTION.GO_RIGHT))
        {
            walkingChange = new Vector3(1.0f, 0.0f, 0.0f);
        }

        if (this.playerInput.ListenForClick(PlayerInput.PLAYER_ACTION.SHOOT))
        {
            // Handle shooting logic here
        }
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
        Vector3 rightMovement = right * walkingSpeed * Time.fixedDeltaTime * walkingChange.x;

        // Only consider X-axis component for heading
        heading = Vector3.Normalize(new Vector3(rightMovement.x, 0.0f, 0.0f));

        myRigidbody.position += rightMovement;
    }


    private void UpdateRotation()
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
    }
}
