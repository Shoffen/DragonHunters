using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : MonoBehaviour
{
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public Animator bowAnimator;
    public Animator playerAnimator;
    public GameObject arrowPrefab;
    public Transform shootingDirection;
    private GameObject currentArrow;
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public float trajectoryPower;
    [SerializeField] private TrajectoryLine trajectoryLine;
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------


    // Start is called before the first frame update
    void Start()
    {
       
        //bowAnimator.Play("Load");
        //bowAnimator.speed = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        if (playerAnimator.GetFloat("DrawTension") > 0)
        {
            trajectoryLine.lineRenderer.gameObject.SetActive(true);
            trajectoryLine.ShowTrajectoryLine(shootingDirection.position, shootingDirection.forward * trajectoryPower * playerAnimator.GetFloat("DrawTension"));
        }
        else
        {
            trajectoryLine.lineRenderer.gameObject.SetActive(false);
        }
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
    }
    private void FixedUpdate()
    {
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        //RotateArrowToFaceGround();
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
    }
    public void Fire(float chargedPower)
    {
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        Vector3 direction = new Vector3(shootingDirection.forward.x, shootingDirection.forward.y, 0.0148f);
        Quaternion arrowRotation = Quaternion.LookRotation(direction) * Quaternion.Euler(90f, 0f, 0f);
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        currentArrow = Instantiate(arrowPrefab, shootingDirection.position, arrowRotation);
        currentArrow.GetComponent<ArrowLogic>().tension = chargedPower;
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        Rigidbody rb = currentArrow.GetComponent<Rigidbody>();
        rb.AddForce(direction * trajectoryPower * chargedPower, ForceMode.Impulse);
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------


    }


}
