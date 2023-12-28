using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public HealthBar healthBar;
    public Animator animator;
    public Rigidbody rigidBody;
    public float followRadius;
    public float attackRadius;
    public Vector3 movement;
    public Transform playerTarget;
    public FloatValue maxHealth;
    public GameObject damageLabel;
    public Transform damageLabelSpawn;
    public float movementSpeed;
    private HitColliderData colliderData;
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------
    private void Start()
    {
        playerTarget = GameObject.Find("Player").transform;
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        healthBar.SetMaxHealth(maxHealth.value);
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
    }
    public bool IsInRadiusToFollow()
    {
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        float distance = Vector3.Distance(playerTarget.position, this.transform.position);
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        return distance < followRadius;
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
    }
    public bool IsInRadiusToAttack()
    {
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        float distance = Vector3.Distance(playerTarget.position, this.transform.position);
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        return distance < attackRadius;
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
    }
    public void GetDamage(int damage)
    {
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        healthBar.ApplyDamage(damage);
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        if (IsDead(healthBar) && !(animator.GetBool("IsDead")))
        {
            animator.SetTrigger("Dead");
            animator.SetBool("IsDead", true);
            GameObject newDamageLabel = Instantiate(damageLabel, damageLabelSpawn.position, damageLabel.transform.rotation);

            newDamageLabel.GetComponent<DamageLabel>().hitDamage = damage;

            rigidBody.isKinematic = true;
            rigidBody.GetComponent<HitColliderData>().DisableColliders();

            StartCoroutine(Vanish());
        }
        if (!animator.GetBool("IsDead"))
        {
            animator.SetTrigger("PlayGetHit");
            GameObject newDamageLabel = Instantiate(damageLabel, damageLabelSpawn.position, damageLabel.transform.rotation);

            newDamageLabel.GetComponent<DamageLabel>().hitDamage = damage;
        }
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------

    }
    public bool IsDead(HealthBar healthBar)
    {
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        return healthBar.slider.value == 0;
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
    }
    private IEnumerator Vanish()
    {
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        yield return new WaitForSeconds(8f);
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        Destroy(rigidBody.gameObject);
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------


    }
    private void FixedUpdate()
    {
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        if (!animator.GetBool("IsDead"))
        {
            UpdateMovement();
            UpdateRotation();
        }
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
    }
    public void UpdateMovement()
    {
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        rigidBody.MovePosition(rigidBody.position + movement * (animator.GetBool("CanFollow") ? 1 : 0));
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
    }
    public void UpdateRotation()
    {
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        Quaternion target = Quaternion.Euler(new Vector3(0, 90 * Mathf.Sign(playerTarget.position.x - transform.position.x)));
        rigidBody.rotation = Quaternion.Lerp(rigidBody.rotation, target, Time.deltaTime * 6);
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
    }
}
