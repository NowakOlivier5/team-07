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

    /*private void Update()
    {
        // Checks if the health points of the fragment is at or below 0
        // If so, enable rigibody and gravity on the fragment and disable the script
        if (isGlass == true && objectHealth <= 0)
        {
            rb.isKinematic = false;
            Destroy(gameObject); // Replace with a particle effect when the glass is destroyed
            this.enabled = false;
        }
        if (objectHealth <= 0)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            this.enabled = false;
        }
    }*/

    public void TakeDamage(int damage)
    {
        if (isGlass == true)
        {
            objectHealth = 0;
            Destroy(gameObject);
            this.enabled = false;
        }

        objectHealth -= damage;

        if (objectHealth <= 0)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            this.enabled = false;
        }
    }
    
    
    // Checks if the object with the "Bullet" tag has collided with the object. If so, reduce its health points value by 1
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet") || collision.gameObject.CompareTag("Missile"))
        {
            TakeDamage(1);
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
