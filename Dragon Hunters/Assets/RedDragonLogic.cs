using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedDragonLogic : Enemy
{

    /*public HealthBar healthBar;
    void Start()
    {
        animator = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody>();

        healthBar.SetMaxHealth(maxHealth.value);

    }
    // Update is called once per frame
    void Update()
    {

        if (IsInRadiusToFollow() && !(animator.GetBool("IsDead")))
        {
            // Calculate the direction from the AI to the player
            Vector3 direction = (playerTarget.position - transform.position).normalized;

            if (IsInRadiusToAttack())
            {
                animator.SetBool("CanAttack", true);
                animator.SetBool("CanFollow", false);
                // Set the movement vector
                movement = Vector3.zero;
            }
            else if (!IsInRadiusToAttack())
            {
                animator.SetBool("CanFollow", true);
                animator.SetBool("CanAttack", false);

                // Set the movement vector
                movement = new Vector3(direction.x, 0, direction.z) * 1.5F * Time.fixedDeltaTime;
            }
        }
        else
        {
            animator.SetBool("CanFollow", false);
            movement = Vector3.zero;
        }
        Debug.Log(animator.GetBool("CanFollow"));
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
    }*/


}
