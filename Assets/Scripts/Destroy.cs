using UnityEngine;

public class Destroy : MonoBehaviour
{
    public Rigidbody rb;
    public int objectHealth = 5;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb.isKinematic = true;
        rb.useGravity = false;
    }

    private void Update()
    {
        if(objectHealth <= 0)
        {
            rb.isKinematic=false;
            rb.useGravity = true;
            enabled = false;
        }
        Debug.Log(objectHealth);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Bullet"))
        {
            objectHealth = objectHealth - 1;
        }
    }
}
