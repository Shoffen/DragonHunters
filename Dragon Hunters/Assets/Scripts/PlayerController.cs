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

    [SerializeField] private Animator playerAnimator;
    [SerializeField] private Animator bowAnimator;
    [SerializeField] private AnimationCurve aimPrepareCurve;
    [SerializeField] private MultiAimConstraint[] spineBones;
    [SerializeField] private MultiAimConstraint shoulderBone;
   

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
                bowAnimator.SetBool("Unstuck", false);
                bowAnimator.SetBool("Reset", false);
                StartCoroutine(SetToLoad());
                StartCoroutine(SetBowStringToLoad());
            }
            else
            {
                SetAimingState(AIMING_STATE.IDLE);
                playerAnimator.SetBool("Aiming", false);
                bowAnimator.SetBool("Unstuck", true);
                bowAnimator.SetBool("Stuck", false);
                

            }
           
        }
        if (playerInput.ListenForClick(InputManager.PLAYER_ACTION.SHOOTING))
        {
            if (aimingState == AIMING_STATE.AIMING)
            {
                if(playerAnimator.GetBool("CanShoot"))
                {
                    playerAnimator.Play("AimRecoil", 1, 0f);
                    playerAnimator.SetBool("Shooting", true);
                    playerAnimator.SetBool("CanShoot", false);
                    bowAnimator.SetBool("Unstuck", true);
                    bowAnimator.SetBool("Stuck", false);
                    //bowAnimator.Play("UnstuckLoad");
                    StartCoroutine(ResetAnimationCoroutine());
                    StartCoroutine(ResetShootTimer());
                    
                    //bow.Load();
                }
                
               
            }
          
        }
    }
    private IEnumerator SetBowStringToLoad()
    {
        yield return new WaitForSeconds(1 / 1.5f);
        bowAnimator.SetBool("Stuck", true);



    }
    private IEnumerator SetToLoad()
    {
        yield return new WaitForSeconds(1f/10);
        playerAnimator.SetBool("Aiming", true);
       


    }
    private IEnumerator ResetAnimationCoroutine()
    {
        yield return new WaitForSeconds(1f / 4);
        playerAnimator.SetBool("Shooting", false);
        bowAnimator.SetBool("Unstuck", false);
     

    }
    private IEnumerator ResetShootTimer()
    {
        yield return new WaitForSeconds(1 /1.15f);
        playerAnimator.SetBool("CanShoot", true);
        bowAnimator.SetBool("Stuck", true);


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
}
