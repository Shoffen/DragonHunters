using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitColliderData : MonoBehaviour
{
    public Collider[] headColliders;
    public Collider[] chestColliders;
    public Collider boxCollider;
    public float headMultiplier;
    public float chestMultiplier;

    private Enemy enemy;



    // Start is called before the first frame update
    void Start()
    {
        enemy = GetComponent<Enemy>();
    }

    public void PassInHit(Collider hit, float damage)
    {
        
        if (hit == null) return;

        

        foreach (Collider headCollider in headColliders)
        {
            if (hit == headCollider)
            {
                enemy.GetDamage((int)(damage * headMultiplier));
            }
        }
        foreach (Collider chestCollider in chestColliders)
        {
            if (hit == chestCollider)
            {
                enemy.GetDamage((int)(damage * chestMultiplier));
            }
        }
    }
    public void DisableColliders()
    {
        foreach (Collider headCollider in headColliders)
        {
            headCollider.enabled = false;
        }

        foreach (Collider chestCollider in chestColliders)
        {
            chestCollider.enabled = false;
        }

        if (boxCollider != null)
        {
            boxCollider.enabled = false;
        }
    }
}
