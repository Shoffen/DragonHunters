using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : MonoBehaviour
{
    public Animator bowAnimator;
    public Animator playerAnimator;
    public Rigidbody arrowPrefab;
    public Transform shootingDirection;
    
    private Rigidbody currentArrow;
    private InputManager inputManager;

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
    public void LoadBow()
    {
        bowAnimator.Play("Load");
        //bowAnimator.SetBool("Load", true);
        //StartCoroutine(ResetAnimationCoroutine());
    }
    private IEnumerator ResetAnimationCoroutine()
    {
        yield return new WaitForSeconds(1f / 2);
        //bowAnimator.SetBool("Load", false);

    }
    public void Fire()
    {
        Vector3 direction = new Vector3(shootingDirection.position.x, shootingDirection.position.y, shootingDirection.position.z).normalized;
        currentArrow = Instantiate(arrowPrefab);
        currentArrow.AddForce(direction * 2F, ForceMode.VelocityChange);
    }
}
