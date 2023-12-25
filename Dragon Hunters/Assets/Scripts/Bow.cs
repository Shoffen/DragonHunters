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
    private GameObject fire;
    private GameObject particleSystem;

    [SerializeField] private GameObject blueFireVFX;
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
            trajectoryLine.ShowTrajectoryLine(shootingDirection.position, shootingDirection.forward * trajectoryPower * playerAnimator.GetFloat("DrawTension"), playerAnimator.GetFloat("DrawTension"));
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
    public void Fire(float chargedPower, float time)
    {
        Transform specificChild = null;
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        Vector3 direction = new Vector3(shootingDirection.forward.x, shootingDirection.forward.y, 0.0148f);
        Quaternion arrowRotation = Quaternion.LookRotation(direction) * Quaternion.Euler(90f, 0f, 0f);
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        currentArrow = Instantiate(arrowPrefab, shootingDirection.position, arrowRotation);
        if (chargedPower >= 0.55)
        {
            for(int i = 0; i < currentArrow.transform.childCount; i++)
            {
                if(currentArrow.transform.GetChild(i).name == "end")
                {
                    specificChild = currentArrow.transform.GetChild(i);
                }
                if (currentArrow.transform.GetChild(i).name == "Particle System")
                {
                    particleSystem = currentArrow.transform.GetChild(i).gameObject;
                }
            }
            fire = Instantiate(blueFireVFX, specificChild);
            fire.GetComponent<ParticleSystem>().Play();
        }
        if(time >= 1.45f)
        {
            particleSystem.SetActive(true);
        }
        currentArrow.GetComponent<ArrowLogic>().tension = chargedPower;
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        Rigidbody rb = currentArrow.GetComponent<Rigidbody>();
        rb.AddForce(direction * trajectoryPower * chargedPower, ForceMode.Impulse);
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------


    }


}
