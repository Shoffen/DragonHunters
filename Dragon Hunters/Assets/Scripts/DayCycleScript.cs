using UnityEngine;

public class DayCycleScript : MonoBehaviour
{
    public float jumpForce = 10f; // Adjust this to change the force of the jump
    private Rigidbody rb;
    public Renderer renderer;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        renderer = GetComponent<Renderer>();
    }



    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground")) // Check if the object collided with is tagged as "Ground"
        {
            Jump(); // Call the Jump function
        }
    }

    void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); // Apply an upward force to the Rigidbody
        Color randomColor = new Color(Random.value, Random.value, Random.value); // Randomly generate a color
        renderer.material.color = randomColor;
    }
}
