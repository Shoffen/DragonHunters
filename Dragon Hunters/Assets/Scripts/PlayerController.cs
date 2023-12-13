using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private const float ACCELERATION = 12f;
    private const float AIMING_Y_OFFSET_TARGET = 80f;
    private const float AIM_PREPARE_DURATION = 4f; // second / n
    private const float AIM_WEIGHT = 0.5f;
    private const float IDLE_WEIGHT = 0.1f;
    private Coroutine releaseCoroutine;

    [SerializeField] private Animator playerAnimator;
    [SerializeField] private Animator bowAnimator;
    [SerializeField] private AnimationCurve aimPrepareCurve;
    [SerializeField] private MultiAimConstraint[] spineBones;
    [SerializeField] private MultiAimConstraint shoulderBone;
    [SerializeField] private AudioSource releaseArrow;
    [SerializeField] private AudioSource loadArrow;
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpDuration;
    [SerializeField] private float groundRadius;

  
    private Rigidbody myRigidbody;
    public Bow bow;
    private InputManager playerInput;
    private float moveAnimationSpeedTarget;
    private float currentMoveX;
    public Transform opa;

    public float walkingSpeed;
    public float rotationSpeed;
    private Vector3 walkingChange;

    private Camera mainCamera;
    public Transform targetTransform;
    public LayerMask mouseAimMask;
    public LayerMask groundLayer;

    private float targetAimDelta;
    private float weightStart;
    private float weightTarget;
    private float startAimDelta;
    private float aimDeltaTime;
    private bool isAimStateChangeing;
    private AIMING_STATE aimingState;


    private bool isJumping = false;
    private float jumpTimer = 0f;
    private float drawTension;

    private enum AIMING_STATE
    {
        IDLE,
        AIMING,
        CHARGING
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
        
        SetAimingState(AIMING_STATE.AIMING, true);
        mainCamera = Camera.main;

        
    }

    private void Update()
    {
        GetInput();
    }

    public void SetPlayerState(int _state)
    {
        aimingState = (AIMING_STATE)_state;
    }

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
            shoulderBone.weight = 0f;
            if (_isInstant)
            {
                foreach (MultiAimConstraint constraint in spineBones)
                {
                    constraint.data.offset = new Vector3(0, 0, 0);
                    constraint.weight = IDLE_WEIGHT;
                }
                playerAnimator.SetLayerWeight(1, targetAimDelta);
                isAimStateChangeing = false;
            }
        }
        else
        {
            shoulderBone.weight = 0.3f;
            weightStart = IDLE_WEIGHT;
            weightTarget = AIM_WEIGHT;
            startAimDelta = 0;
            targetAimDelta = 1;
            if (_isInstant)
            {
                playerAnimator.SetLayerWeight(1, targetAimDelta);
               
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
            playerAnimator.SetLayerWeight(1, Mathf.Lerp(startAimDelta, targetAimDelta, aimPrepareCurve.Evaluate(aimDeltaTime)));
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

    private void GetInput()
    {
       
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, mouseAimMask))
        {
            targetTransform.position = new Vector3(hit.point.x, hit.point.y, hit.point.z - 2f);
        }
        walkingChange = playerInput.GetAxis(InputManager.AXIS.MOVE);
        if (isAimStateChangeing)
        {
            return;
        }
        switch (aimingState)
        {
            case AIMING_STATE.IDLE:
                return;
            case AIMING_STATE.AIMING:
                playerAnimator.SetLayerWeight(1, 1);
                return;
            case AIMING_STATE.CHARGING:
                if (playerInput.IsButtonHeld(InputManager.PLAYER_ACTION.SHOOTING))
                {
                    drawTension = Mathf.Clamp01(drawTension + Time.deltaTime);
                    playerAnimator.SetFloat("DrawTension", drawTension);
                }
                else
                {
                    if(drawTension != 0)
                    {
                        drawTension = 0;
                        playerAnimator.SetFloat("DrawTension", drawTension);
                        playerAnimator.SetTrigger("Shoot");
                        SetPlayerState((int)AIMING_STATE.AIMING);
                        //shot();
                    }
                    playerAnimator.SetFloat("DrawTension", drawTension);
                }
                return;
        }
        if (playerInput.ListenForClick(InputManager.PLAYER_ACTION.SHOOTING))
        {
            if (aimingState == AIMING_STATE.AIMING)
            {
                /*if(playerAnimator.GetBool("CanShoot"))
                {
                    playerAnimator.Play("AimRecoil", 1, 0f);
                    releaseArrow.Play();
                    playerAnimator.SetBool("Shooting", true);
                    playerAnimator.SetBool("CanShoot", false);
                    bowAnimator.SetBool("Unstuck", true);
                    bowAnimator.SetBool("Stuck", false);
                    //Debug.Log("REIK BOW");
                    bow.Fire();
                    StartCoroutine(SetLoadArrowTimer());
                    //bowAnimator.Play("UnstuckLoad");
                    StartCoroutine(ResetAnimationCoroutine());
                    StartCoroutine(ResetShootTimer());
                    
                    //bow.Load();
                }*/
                
               
            }
          
        }
        //--------------------------------------------------------------------------------------------------------------------------------------
        playerAnimator.SetBool("IsGrounded", IsInGroundRadius());
        //--------------------------------------------------------------------------------------------------------------------------------------
        if (playerInput.ListenForClick(InputManager.PLAYER_ACTION.JUMPING))
        {
            if (playerAnimator.GetBool("CanJump"))
            {
                isJumping = true;
                StartCoroutine(ResetJump(jumpDuration));
            }

        }
        //--------------------------------------------------------------------------------------------------------------------------------------
        if (!playerAnimator.GetBool("IsJumping"))
        {
            playerAnimator.SetBool("IsFalling", !IsInGroundRadius());
        }
        if (playerAnimator.GetBool("IsFalling"))
        {

            jumpTimer += Time.fixedDeltaTime;
            float jumpProgress = jumpTimer / jumpDuration;


            Vector2 newVelocity = Vector2.Lerp(myRigidbody.velocity, Vector2.down * jumpForce, jumpProgress);


            //Debug.Log("Final velocity: " + myRigidbody.velocity.y);
        }

        //--------------------------------------------------------------------------------------------------------------------------------------

        //--------------------------------------------------------------------------------------------------------------------------------------
    }
    private IEnumerator SetToFall()
    {
        yield return new WaitForSeconds(1 / 2f);
        myRigidbody.mass = 1000f;
        playerAnimator.SetBool("IsJumping", false);
        myRigidbody.velocity = Vector2.zero;
        playerAnimator.SetBool("IsFalling", true);

    }
    private IEnumerator ResetJump(float jumpDuration)
    {
        yield return new WaitForSeconds(jumpDuration + 2f);
        playerAnimator.SetBool("CanJump", true);
        jumpTimer = 0f;
    }

    private IEnumerator ResetAnimationCoroutine()
    {
        yield return new WaitForSeconds(1f / 4);
        playerAnimator.SetBool("Shooting", false);
        bowAnimator.SetBool("Unstuck", false);
     

    }
    private void FixedUpdate()
    {
        UpdateMovement();
        UpdateRotation();
        UpdateAimingState();
        UpdateJumpingCycle();
    }

    private void UpdateJumpingCycle()
    {
        if (isJumping)
        {
            
            jumpTimer += Time.fixedDeltaTime;
            float jumpProgress = jumpTimer / jumpDuration;

            // Interpolate between the current velocity and the desired jump velocity
            Vector2 newVelocity = Vector2.Lerp(myRigidbody.velocity, Vector2.up * jumpForce, jumpProgress);

            // Apply the new velocity to the rigidbody
            myRigidbody.velocity = newVelocity;

            playerAnimator.SetBool("IsJumping", true);
            //playerAnimator.SetBool("IsGrounded", false);
            myRigidbody.mass = 1.0f;
            StartCoroutine(SetToFall());
            playerAnimator.SetBool("CanJump", false);
            // Check if the jump duration is complete
            if (jumpTimer >= jumpDuration)
            {
                isJumping = false;
                
            }
        }

    }
    private void UpdateMovement()
    {

        Vector3 movement;
        playerAnimator.SetFloat("MoveSpeed", 0);
        currentMoveX = Mathf.Lerp(currentMoveX, walkingChange.x, Time.deltaTime * ACCELERATION);
        moveAnimationSpeedTarget = Mathf.Lerp(moveAnimationSpeedTarget, walkingChange.magnitude, Time.deltaTime * (ACCELERATION / 2));

        if (NeedBackwards)
        {
            playerAnimator.SetFloat("MoveSpeed", moveAnimationSpeedTarget * (-1));
            movement = new Vector3(currentMoveX, 0, 0) * walkingSpeed / 2 * Time.fixedDeltaTime;
        }
        else
        {
            playerAnimator.SetFloat("MoveSpeed", moveAnimationSpeedTarget);
            movement = new Vector3(currentMoveX, 0, 0) * walkingSpeed * Time.fixedDeltaTime;
        }
        myRigidbody.MovePosition(myRigidbody.position + movement);
        
        
    }

    private void UpdateRotation()
    {
        Quaternion target = Quaternion.Euler(new Vector3(0, 90 * Mathf.Sign(targetTransform.position.x - transform.position.x)));

        myRigidbody.rotation = Quaternion.Lerp(myRigidbody.rotation, target, Time.deltaTime * rotationSpeed);
    }
    private bool IsInGroundRadius()
    {
        bool isInRadius = false;
        // Check for GROUND objects within the specified radius
        Collider[] colliders = Physics.OverlapSphere(myRigidbody.position, groundRadius, groundLayer);

        // Iterate through the colliders to see if any of them have the "GROUND" tag
        foreach (Collider col in colliders)
        {
            if (col.CompareTag("Ground"))
            {
                // The "GROUND" object is within the specified radius
                isInRadius = true;
                // You can perform additional actions here if needed
            }
        }
        return isInRadius;
    }


}
