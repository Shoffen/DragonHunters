using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : MonoBehaviour
{
    public Animator bowAnimator;
    public Animator playerAnimator;
    public GameObject arrowPrefab;
    public Transform shootingDirection;
    
    private GameObject currentArrow;
   

    // Start is called before the first frame update
    void Start()
    {
        
        //bowAnimator.Play("Load");
        //bowAnimator.speed = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
   
    private IEnumerator ResetAnimationCoroutine()
    {
        yield return new WaitForSeconds(1f / 2);
        //bowAnimator.SetBool("Load", false);

    }
    public void Fire()
    {
        Vector3 direction = shootingDirection.forward;
        Quaternion arrowRotation = Quaternion.LookRotation(direction) * Quaternion.Euler(90f, 0f, 0f);
        
        currentArrow = Instantiate(arrowPrefab, shootingDirection.position, arrowRotation);
        Rigidbody rb = currentArrow.GetComponent<Rigidbody>();
       
        rb.AddForce(direction * 15F, ForceMode.VelocityChange);
    }
}
