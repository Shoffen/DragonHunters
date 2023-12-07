using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueDragonLogic : MonoBehaviour
{
    public float followRadius;
    public float attackRadius;
    private Animator animator;
    private Rigidbody rigidBody;
    private Vector3 movement;
    public Transform playerTarget;

    public Transform headTransform;

    [SerializeField] private Collider headCollider;
    [SerializeField] private Collider chestCollider;


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody>();
    }
   
    // Update is called once per frame
    void Update()
    {

        animator.SetBool("CanFollow", IsInRadiusToFollow());
        animator.SetBool("CanAttack", IsInRadiusToAttack());
        if(animator.GetBool("CanFollow"))
        {
            // Calculate the direction from the AI to the player
            Vector3 direction = (playerTarget.position - transform.position).normalized;

            // Set the movement vector
            movement = new Vector3(direction.x, 0, direction.z)  * 1.5F * Time.fixedDeltaTime;
            Debug.Log(movement);
        }
       
    }
    private void FixedUpdate()
    {
        UpdateMovement();
        UpdateRotation();
    }
    private void UpdateMovement()
    {
        rigidBody.MovePosition(rigidBody.position + movement);
    }
    private void UpdateRotation()
    {
        Quaternion target = Quaternion.Euler(new Vector3(0, 90 * Mathf.Sign(playerTarget.position.x - transform.position.x)));

        rigidBody.rotation = Quaternion.Lerp(rigidBody.rotation, target, Time.deltaTime * 6);
    }
    private bool IsInRadiusToFollow()
    {
        float distance = Vector3.Distance(playerTarget.position, this.transform.position);
     
        return distance < followRadius;
    }
    private bool IsInRadiusToAttack()
    {
        float distance = Vector3.Distance(playerTarget.position, this.transform.position);
        return distance < attackRadius;
    }
   
}
