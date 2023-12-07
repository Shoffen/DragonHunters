using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ArrowLogic : MonoBehaviour
{
    private Rigidbody myBody;
    public BoxCollider[] colliders;
    // Start is called before the first frame update
    void Start()
    {
        myBody = GetComponent<Rigidbody>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Glass"))
        {

        }
        else
        {
            myBody.isKinematic = true;
            foreach(BoxCollider collider in colliders)
            {
                collider.enabled = false;
            }

        }
        
    }
}
