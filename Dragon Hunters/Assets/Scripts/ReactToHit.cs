using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactToHit : MonoBehaviour
{
    public BlueDragonLogic dragonLogic;
    public int damageValue;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Arrow"))
        {
            dragonLogic.GetDamage(damageValue);
            Animator animator = dragonLogic.GetComponent<Animator>();
            if (!animator.GetBool("IsDead"))
            {
                Debug.Log("IKIRTAU");
                animator.SetTrigger("PlayGetHit");
            }
        }
    }
}
