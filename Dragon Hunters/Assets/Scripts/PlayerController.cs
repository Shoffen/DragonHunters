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

<<<<<<< Updated upstream
    private void Start()
    {
        myRigidbody = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();

        forward = Camera.main.transform.forward;
        forward.y = 0;
        forward = Vector3.Normalize(forward);
        right = Quaternion.Euler(new Vector3(0, 90, 0)) * forward;

=======
    private float targetAimDelta;
    private float weightStart;
    private float weightTarget;
    private float startAimDelta;
    private float aimDeltaTime;
    private bool isAimStateChangeing;
    private AIMING_STATE aimingState;

    

    private enum AIMING_STATE
    {
        IDLE,
        AIMING
    }
    

    private bool NeedBackwards
    {
        get
        {
            return targetTransform.position.x > transform.position.x && walkingChange.x == 1? false : 
                targetTransform.position.x < transform.position.x && walkingChange.x == -1 ? false : true;
        }
    }
    private void Start()
    {
        myRigidbody = GetComponent<Rigidbody>();
        playerInput = GetComponent<InputManager>();
        SetAimingState(AIMING_STATE.IDLE, true);
        
        mainCamera = Camera.main;
>>>>>>> Stashed changes
    }

    private void Update()
    {
        GetInput();
    }
<<<<<<< Updated upstream
=======
    
    private void SetAimingState(AIMING_STATE _state = AIMING_STATE.IDLE, bool _isInstant = false)
    {
        aimingState = _state;
        isAimStateChangeing = true;
        if (aimingState == AIMING_STATE.IDLE)
        {
            startAimDelta = 1;
            targetAimDelta = 0;
            weightStart = AIM_WEIGHT;
            weightTarget = IDLE_WEIGHT;
            if (_isInstant)
            {
                foreach (MultiAimConstraint constraint in spineBones)
                {
                    constraint.data.offset = new Vector3(0, 0, 0);
                    constraint.weight = IDLE_WEIGHT;
                }
                animator.SetLayerWeight(1, targetAimDelta);
                isAimStateChangeing = false;
            }
        }
        else
        {
            weightStart = IDLE_WEIGHT;
            weightTarget = AIM_WEIGHT;
            startAimDelta = 0;
            targetAimDelta = 1;
            if (_isInstant)
            {
                animator.SetLayerWeight(1, targetAimDelta);
                foreach (MultiAimConstraint constraint in spineBones)
                {
                    constraint.data.offset = new Vector3(0, AIMING_Y_OFFSET_TARGET, 0);
                    constraint.weight = AIM_WEIGHT;
                }
                isAimStateChangeing = false;
            }
        }
    }

    private void UpdateAimingState()
    {
        if (isAimStateChangeing)
        {
            aimDeltaTime = Mathf.Clamp01(aimDeltaTime + Time.deltaTime * AIM_PREPARE_DURATION);
            animator.SetLayerWeight(1, Mathf.Lerp(startAimDelta, targetAimDelta, aimPrepareCurve.Evaluate(aimDeltaTime)));
            float _yAxis = Mathf.Lerp(startAimDelta, targetAimDelta * AIMING_Y_OFFSET_TARGET, aimPrepareCurve.Evaluate(aimDeltaTime));
            foreach (MultiAimConstraint constraint in spineBones)
            {
                constraint.data.offset = new Vector3(0, _yAxis, 0);
                constraint.weight = Mathf.Lerp(weightStart, weightTarget, aimPrepareCurve.Evaluate(aimDeltaTime));
            }
            if (aimDeltaTime == 1)
            {
                isAimStateChangeing = false;
                aimDeltaTime = 0;
            }
        }
    }
>>>>>>> Stashed changes

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
<<<<<<< Updated upstream
            // Handle shooting logic here
=======
            if(aimingState == AIMING_STATE.IDLE)
            {
                SetAimingState(AIMING_STATE.AIMING);
               
            }
            else
            {
                SetAimingState(AIMING_STATE.IDLE);
               
            }
        }
        if (playerInput.ListenForClick(InputManager.PLAYER_ACTION.SHOOTING))
        {
            if(aimingState == AIMING_STATE.AIMING)
            {

                if (!(animator.GetCurrentAnimatorStateInfo(1)).IsName("Shooting"))
                {
                    animator.Play("Shooting", 1, 0f);
                    StartCoroutine(ResetAnimationState());
                   
                }
            }
>>>>>>> Stashed changes
        }
    }
    IEnumerator ResetAnimationState()
    {
        yield return new WaitForSeconds(1f);
        animator.SetBool("Shooting", false);

    }
    private void FixedUpdate()
    {
        UpdateMovement();
        UpdateRotation();
    }

    private void UpdateMovement()
    {
<<<<<<< Updated upstream
        if (walkingChange != Vector3.zero)
        {
            animator.SetBool("idle", false);
            animator.SetBool("walking", true);
        }
        else
        {
            animator.SetBool("walking", false);
            animator.SetBool("idle", true);
=======
        Vector3 movement;
        
        currentMoveX = Mathf.Lerp(currentMoveX, walkingChange.x, Time.deltaTime * ACCELERATION);
        moveAnimationSpeedTarget = Mathf.Lerp(moveAnimationSpeedTarget, walkingChange.magnitude, Time.deltaTime * (ACCELERATION / 2));

        if (NeedBackwards)
        {
            
            animator.SetFloat("MoveSpeed", moveAnimationSpeedTarget * (-1));
            movement = new Vector3(currentMoveX, 0, 0) * walkingSpeed / 2 * Time.fixedDeltaTime;
        }
        else
        {
            
            animator.SetFloat("MoveSpeed", moveAnimationSpeedTarget);
            movement = new Vector3(currentMoveX, 0, 0) * walkingSpeed * Time.fixedDeltaTime;
           
>>>>>>> Stashed changes
        }

        walkingChange = Vector3.ClampMagnitude(walkingChange, 1f);
        Vector3 rightMovement = right * walkingSpeed * Time.fixedDeltaTime * walkingChange.x;

        // Only consider X-axis component for heading
        heading = Vector3.Normalize(new Vector3(rightMovement.x, 0.0f, 0.0f));

        myRigidbody.position += rightMovement;
    }


    private void UpdateRotation()
    {
<<<<<<< Updated upstream
        float rotationInput = walkingChange.x;

        if (!Mathf.Approximately(rotationInput, 0.0f))
        {
            Vector3 rotationDirection = Vector3.Cross(Vector3.forward, heading);
            rotationAngle = Mathf.Sign(rotationInput) * Mathf.Rad2Deg * Mathf.Acos(Vector3.Dot(Vector3.forward, heading));

            // Apply rotation
            targetRotation = Quaternion.Euler(0, rotationAngle, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
=======

        Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 90 * Mathf.Sign(targetTransform.position.x - transform.position.x), 0));
        myRigidbody.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        //myRigidbody.MoveRotation(Quaternion.Euler(new Vector3(0, 90 * Mathf.Sign(targetTransform.position.x - transform.position.x), 0)));
       

>>>>>>> Stashed changes
    }
}
