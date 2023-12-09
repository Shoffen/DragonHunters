using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreamState : StateMachineBehaviour
{
    // Reference to the character's rigidbody
    private Rigidbody rigidbody;

    //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Get the rigidbody component
        rigidbody = animator.GetComponent<Rigidbody>();

        // Freeze the position constraints
        if (rigidbody != null)
        {
            rigidbody.constraints = RigidbodyConstraints.FreezePosition;
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Unfreeze the position constraints when the scream animation is finished
        if (rigidbody != null)
        {
            rigidbody.constraints = RigidbodyConstraints.None;
        }
    }
}
