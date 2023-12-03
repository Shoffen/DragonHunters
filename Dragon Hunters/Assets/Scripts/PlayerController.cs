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

    [SerializeField] private Animator animator;
    [SerializeField] private AnimationCurve aimPrepareCurve;
    [SerializeField] private MultiAimConstraint[] spineBones;

    private Rigidbody myRigidbody;
    private InputManager playerInput;
    private float moveAnimationSpeedTarget;
    private float currentMoveX;

    public float walkingSpeed;
    public float rotationSpeed;
    private Vector3 walkingChange;

    private Camera mainCamera;
    public Transform targetTransform;
    public LayerMask mouseAimMask;

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
    }

    private void Update()
    {
        GetInput();
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

    private void GetInput()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, mouseAimMask))
        {
            targetTransform.position = hit.point;
        }
        walkingChange = playerInput.GetAxis(InputManager.AXIS.MOVE);
        if (isAimStateChangeing)
        {
            return;
        }
        if (playerInput.ListenForClick(InputManager.PLAYER_ACTION.AIMING))
        {
            if(aimingState == AIMING_STATE.IDLE)
            {
                SetAimingState(AIMING_STATE.AIMING);
            }
            else
            {
                SetAimingState(AIMING_STATE.IDLE);
            }
        }
    }

    private void FixedUpdate()
    {
        UpdateMovement();
        UpdateRotation();
        UpdateAimingState();
    }

    private void UpdateMovement()
    {
        Vector3 movement;
        animator.SetFloat("MoveSpeed", 0);
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
        }
        myRigidbody.MovePosition(myRigidbody.position + movement);
        
        
    }

    private void UpdateRotation()
    {
       
        myRigidbody.MoveRotation(Quaternion.Euler(new Vector3(0, 90 * Mathf.Sign(targetTransform.position.x - transform.position.x), 0)));
    }
}
