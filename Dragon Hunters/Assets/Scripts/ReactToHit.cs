using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactToHit : MonoBehaviour
{
    public BlueDragonLogic dragonLogic;
    [SerializeField] HealthBar healthBar;
    public int damageValue;
   

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Arrow"))
        {
            dragonLogic.GetDamage(damageValue, healthBar);
            Animator animator = dragonLogic.GetComponent<Animator>();
            if (!animator.GetBool("IsDead"))
            {
                animator.SetTrigger("PlayGetHit");
            }
        }
    }
}
