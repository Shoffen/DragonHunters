using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedDragonLogic : MonoBehaviour
{

    private Enemy enemy;
    private Animator animator;
    private Rigidbody rigidBody;
    void Start()
    {
        enemy = GetComponent<Enemy>();
        
        //animator = enemy.GetComponent<Animator>();
        //rigidBody = enemy.GetComponent<Rigidbody>();


    }
    // Update is called once per frame
    void Update()
    {

        if (enemy.IsInRadiusToFollow() && !(enemy.animator.GetBool("IsDead")))
        {
            // Calculate the direction from the AI to the player
            Vector3 direction = (enemy.playerTarget.position - transform.position).normalized;

            if (enemy.IsInRadiusToAttack())
            {
                enemy.animator.SetBool("CanAttack", true);
                enemy.animator.SetBool("CanFollow", false);
                // Set the movement vector
                enemy.movement = Vector3.zero;
            }
            else if (!enemy.IsInRadiusToAttack())
            {
                enemy.animator.SetBool("CanFollow", true);
                enemy.animator.SetBool("CanAttack", false);

                // Set the movement vector
                enemy.movement = new Vector3(direction.x, 0, direction.z) * 1.5F * Time.fixedDeltaTime;
            }
        }
        else
        {
            enemy.animator.SetBool("CanFollow", false);
            enemy.movement = Vector3.zero;
        }
        //Debug.Log(animator.GetBool("CanFollow"));*/
    }
   



}
