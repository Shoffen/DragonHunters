using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ArrowLogic : MonoBehaviour
{
    private Rigidbody myBody;
    public BoxCollider[] colliders;
    private Transform headObject;
    public BlueDragonLogic blueDragonLogic;
    public int arrowDamage;
    
    // Start is called before the first frame update
    void Start()
    {
        myBody = GetComponent<Rigidbody>();
        Debug.Log("OPAAAAAAAAAAAAA" + myBody.velocity.y);
    }

    // Update is called once per frame
    void Update()
    {
        if (this.gameObject.name == "ArrowPre")
        {
            myBody.position = new Vector3(-1.24f, 1.32f, 0.094f);

        }
       
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Glass"))
        {
            
        }
        else if(other.CompareTag("Enemy"))
        {

            other.transform.root.gameObject.GetComponent<HitColliderData>().PassInHit(other, arrowDamage);
            StickArrowToEnemy(other.transform);
            
        }
        else
        {
            myBody.isKinematic = true;
            foreach (BoxCollider collider in colliders)
            {
                collider.enabled = false;
            }
        }
    }
    private void StickArrowToEnemy(Transform enemyHead)
    {
       myBody.isKinematic = true;
       foreach (BoxCollider collider in colliders)
       {
           collider.enabled = false;
       }
       transform.parent = enemyHead;
    }
}
