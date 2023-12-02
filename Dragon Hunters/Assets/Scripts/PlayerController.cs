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

    //public Transform bone;
    //public Vector3 offset;
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
        //bone = animator.GetBoneTransform(HumanBodyBones.Chest);
        //Debug.Log(bone.gameObject.name);
    }

    private void Update()
    {
        Debug.LogError(NeedBackwards);
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
        walkingChange = playerInput.GetAxis(InputManager.AXIS.MOVE);
    }
    /*private void LateUpdate()
    {
        RaycastHit hit;
        if(Physics.Raycast(bone.position, mainCamera.transform.forward, out hit))
        {
            bone.LookAt(hit.point);
            bone.rotation = bone.rotation * Quaternion.Euler(offset);
        }
    }*/
    private void FixedUpdate()
    {
        UpdateMovement();
        UpdateRotation();
    }

    private void UpdateMovement()
    {
        if (NeedBackwards)
        {
            animator.SetFloat("MoveSpeed", 0);
            currentMoveX = Mathf.Lerp(currentMoveX, walkingChange.x, Time.deltaTime * ACCELERATION);
            Vector3 movement = new Vector3(currentMoveX, 0, 0) * walkingSpeed / 2 * Time.fixedDeltaTime;
            moveAnimationSpeedTarget = Mathf.Lerp(moveAnimationSpeedTarget, walkingChange.magnitude, Time.deltaTime * (ACCELERATION / 2));
            animator.SetFloat("MoveBackwards", moveAnimationSpeedTarget);

            myRigidbody.MovePosition(myRigidbody.position + movement);
        }
        if(!NeedBackwards)
        {
            animator.SetFloat("MoveBackwards", 0);
            currentMoveX = Mathf.Lerp(currentMoveX, walkingChange.x, Time.deltaTime * ACCELERATION);
            Vector3 movement = new Vector3(currentMoveX, 0, 0) * walkingSpeed * Time.fixedDeltaTime;
            moveAnimationSpeedTarget = Mathf.Lerp(moveAnimationSpeedTarget, walkingChange.magnitude, Time.deltaTime * (ACCELERATION / 2));
            animator.SetFloat("MoveSpeed", moveAnimationSpeedTarget);

            myRigidbody.MovePosition(myRigidbody.position + movement);
        }
      



    }

    private void UpdateRotation()
    {
        //float targetRotationY = walkingChange.x * 90f;
        //if (targetRotationY != 0)
        //{
            
            //Quaternion targetRotation = Quaternion.Euler(0f, targetRotationY, 0f);
        

            //this.transform.rotation = Quaternion.Slerp(this.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
       
        //}
        myRigidbody.MoveRotation(Quaternion.Euler(new Vector3(0, 90 * Mathf.Sign(targetTransform.position.x - transform.position.x), 0)));
 

    }
    private void OnAnimatorIK()
    {
        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
        animator.SetIKPosition(AvatarIKGoal.RightHand, transform.position);

    }



}
