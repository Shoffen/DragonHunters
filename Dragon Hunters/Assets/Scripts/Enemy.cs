using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    public Animator animator;
    public Rigidbody rigidBody;
    public float followRadius;
    public float attackRadius;
    public Vector3 movement;
    public Transform playerTarget;

    public HealthBar healthBar;
    public FloatValue maxHealth;

    public bool IsInRadiusToFollow()
    {
        float distance = Vector3.Distance(playerTarget.position, this.transform.position);

        return distance < followRadius;
    }
    public bool IsInRadiusToAttack()
    {
        float distance = Vector3.Distance(playerTarget.position, this.transform.position);

        return distance < attackRadius;
    }
    public void OnTriggerEnter(Collider other)
    {
        //Debug.Log("NUMUN");
    }
    public void GetDamage(int damage)
    {
        healthBar.ApplyDamage(damage);

        if (IsDead() && !(animator.GetBool("IsDead")))
        {
            animator.SetTrigger("Dead");
            animator.SetBool("IsDead", true);
            StartCoroutine(Vanish());
        }
    }
    public bool IsDead()
    {
        return healthBar.slider.value == 0;
    }
    private IEnumerator Vanish()
    {
        yield return new WaitForSeconds(8f);
        Destroy(rigidBody.gameObject);


    }
}
