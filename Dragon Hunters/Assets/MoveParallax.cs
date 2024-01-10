using System.Collections;
using UnityEngine;

public class MoveParallax : MonoBehaviour
{
    public float speed = 5f;
    public float respawnX = 4f;

    private Transform startTransform;
    private bool spawned = false;
    private GameObject instantiated;

    void Start()
    {
        spawned = false;
        startTransform = transform;
    }

    void Update()
    {
        if (transform.position.x > respawnX && !spawned)
        {
            InstantiateNewObject();
        }
        if (instantiated != null)
        {
            if (instantiated.transform.position.x > 25)
            {
                Destroy(this.gameObject);
            }
        }
    }

    void FixedUpdate()
    {
        MoveObject();
    }

    void MoveObject()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);

        // If you want to destroy the object when it goes off screen:
        // if (transform.position.x < -respawnX)
        // {
        //     Destroy(gameObject);
        // }
    }

    void InstantiateNewObject()
    {
        spawned = true;
        instantiated = Instantiate(this.gameObject, new Vector3(transform.position.x - 100, transform.position.y, transform.position.z), Quaternion.identity);
        
    }
    private IEnumerator DestroyClouds()
    {
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        yield return new WaitForSeconds(60f);
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        Destroy(this.gameObject);
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------

    }

}
