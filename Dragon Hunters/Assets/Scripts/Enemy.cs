using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class Enemy : MonoBehaviour
{


    public float followRadius;
    public float attackRadius;
    public Animator animator;
    public Rigidbody rigidBody;
    public Vector3 movement;
    public Transform playerTarget;
    public HealthBar healthBar;
    public Transform headTransform;

    [SerializeField] private FloatValue maxHealth;
    


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody>();

        healthBar.SetMaxHealth(maxHealth.value);

    }

    
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

        if (IsDead())
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
