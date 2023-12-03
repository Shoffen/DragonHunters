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

    private Camera mainCamera;
    public Transform targetTransform;
    public LayerMask mouseAimMask;

    private Transform rigTransform;
    private Transform aimingBodyTransform;
    private Transform aimingHandTransform;
    private Transform aimingHeadTransform;
    private Transform notAimingBodyTransform;
    private Transform notAimingHandTransform;
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
        mainCamera = Camera.main;
        rigTransform = transform.Find("Rig 1");
        Debug.Log(rigTransform.name);

        aimingBodyTransform     = rigTransform.Find("AimingBody");
        aimingHandTransform     = rigTransform.Find("AimingHand");

        notAimingBodyTransform  = rigTransform.Find("NotAimingBody");
        notAimingHandTransform  = rigTransform.Find("NotAimingHand");
        aimingHeadTransform     = rigTransform.Find("AimingHead");
    }

    private void Update()
    {
        GetInput();
    }

    private void GetInput()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, mouseAimMask))
        {
            targetTransform.position = hit.point;
           
        }
        if (playerInput.ListenForClick(InputManager.PLAYER_ACTION.AIMING))
        {
            /*if (animator.GetLayerWeight(1) == 1)
            {
                //animator.SetLayerWeight(1, Mathf.Lerp(1, 0, Time.deltaTime * 10));
                animator.SetLayerWeight(1, 0);
                aimingBodyTransform.gameObject.SetActive(false);
                aimingHandTransform.gameObject.SetActive(false);
                aimingHeadTransform.gameObject.SetActive(false);


                notAimingBodyTransform.gameObject.SetActive(true);
                notAimingHandTransform.gameObject.SetActive(true);
            }
            else
            {
                //animator.SetLayerWeight(1, Mathf.Lerp(0, 1, Time.deltaTime * 10));
                animator.SetLayerWeight(1, 1);
                aimingBodyTransform.gameObject.SetActive(true);
                aimingHandTransform.gameObject.SetActive(true);
                aimingHeadTransform.gameObject.SetActive(true);

                notAimingBodyTransform.gameObject.SetActive(false);
                notAimingHandTransform.gameObject.SetActive(false);
            }*/
            if (animator.GetBool("Aiming"))
            {
                animator.SetBool("Aiming", false);
                Debug.Log("NESITAIKOM");
                aimingBodyTransform.gameObject.SetActive(false);
                aimingHandTransform.gameObject.SetActive(false);
                aimingHeadTransform.gameObject.SetActive(false);

                notAimingBodyTransform.gameObject.SetActive(true);
                notAimingHandTransform.gameObject.SetActive(true);
            }
            else
            {
                animator.SetBool("Aiming", true);
                Debug.Log("TAIKOMES");
                aimingBodyTransform.gameObject.SetActive(true);
                aimingHandTransform.gameObject.SetActive(true);
                aimingHeadTransform.gameObject.SetActive(true);

                notAimingBodyTransform.gameObject.SetActive(false);
                notAimingHandTransform.gameObject.SetActive(false);
            }
            
           
        }
 
        walkingChange = playerInput.GetAxis(InputManager.AXIS.MOVE);
    }

    private void FixedUpdate()
    {
        UpdateMovement();
        UpdateRotation();
    }
    private void LateUpdate()
    {

        
        /*if (animator.GetLayerWeight(1) == 1)
        {
            if(NeedBackwards)
            {
                mouseMaskTransform.position = new Vector3(mouseMaskTransform.position.x, mouseMaskTransform.position.y, 1f);
            }
            else if(!NeedBackwards)
            {
                mouseMaskTransform.position = new Vector3(mouseMaskTransform.position.x, mouseMaskTransform.position.y, -4.9f);
            }


        }
        else
        {
            mouseMaskTransform.position = new Vector3(mouseMaskTransform.position.x, mouseMaskTransform.position.y,  -0.083f);

        }*/
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
