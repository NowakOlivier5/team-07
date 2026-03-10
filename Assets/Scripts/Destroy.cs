using UnityEngine;

public class Destroy : MonoBehaviour
{
    // Variables
    public Rigidbody rb;
    public bool isGlass;
    public int objectHealth = 5;

    void Start()
    {
        // Makes sure the object stays static initially
        rb.isKinematic = true;
        rb.useGravity = false;
    }

    private void Update()
    {
        // Checks if the health points of the fragment is at or below 0
        // If so, enable rigibody and gravity on the fragment and disable the script
        if (objectHealth <= 0)
        {
            rb.isKinematic=false;
            rb.useGravity = true;
            this.enabled = false;
        }
    }

    // Checks if the object with the "Bullet" tag has collided with the object. If so, reduce its health points value by 1
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            objectHealth--;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ProtoAgent"))
        {
            objectHealth = 0;
        }
    }
}
