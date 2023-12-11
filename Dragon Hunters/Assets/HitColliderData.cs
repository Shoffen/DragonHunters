using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitColliderData : MonoBehaviour
{
    public Collider[] headColliders;
    public Collider[] chestColliders;

    public int headMultiplier;
    public float chestMultiplier;

    private Enemy enemy;

   
   

    // Start is called before the first frame update
    void Start()
    {
        enemy = GetComponent<Enemy>();
    }

    public void PassInHit(Collider hit, int damage)
    {
        Debug.Log("ATNEÐIAU: " + damage);
        if (hit == null) return;
        foreach (Collider headCollider in headColliders)
        {
            if (hit == headCollider)
            {
                enemy.GetDamage(damage * headMultiplier);
            }
        }
        foreach (Collider chestCollider in chestColliders)
        {
            if (hit == chestCollider)
            {
                enemy.GetDamage(damage * headMultiplier);
            }
        }
    }
}
