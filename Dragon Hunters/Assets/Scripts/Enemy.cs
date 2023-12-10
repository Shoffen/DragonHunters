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
    public FloatValue maxHealth;
    public GameObject damageLabel;
    public Transform damageLabelSpawn;

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
    public void GetDamage(int damage, HealthBar healthBar)
    {
        healthBar.ApplyDamage(damage);

        if (IsDead(healthBar) && !(animator.GetBool("IsDead")))
        {
            animator.SetTrigger("Dead");
            animator.SetBool("IsDead", true);
            StartCoroutine(Vanish());
        }
        GameObject newDamageLabel = Instantiate(damageLabel, damageLabelSpawn.position, damageLabel.transform.rotation);
       
        newDamageLabel.GetComponent<DamageLabel>().hitDamage = damage;

    }
    public bool IsDead(HealthBar healthBar)
    {
        return healthBar.slider.value == 0;
    }
    private IEnumerator Vanish()
    {
        yield return new WaitForSeconds(8f);
        Destroy(rigidBody.gameObject);


    }
}
