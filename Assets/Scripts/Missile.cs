using Unity.VisualScripting;
using UnityEngine;

public class Missile : MonoBehaviour
{
    [SerializeField] float damageRadious = 30f; //The "explosion radious"
    [SerializeField] float explosionForce = 2000f; //The force that will push objects with
    public int mDamage;
    //This is mostly the same as the bullet but for a missile.
    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("Target"))
        {
            Explosion();
            Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("WorldMap"))
        {
            Explosion();
            Destroy(gameObject);
        }
        if (collision.gameObject.CompareTag("ProtoAgent"))
        {
            Explosion();
            ProtoAI enemy = collision.gameObject.GetComponent<ProtoAI>();
            enemy.Die();
        }
    }

    public void Explosion()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, damageRadious); //Makes a colider and physics on a sphere that will work as our explosion radious.
        foreach (Collider objectInRange in colliders) //Applies the same stuff to the colliders that are in range.
        {
            Rigidbody r = objectInRange.GetComponent<Rigidbody>();
            if (r != null) //If the object has a rigid body it applies the force to it if its in the raidous.
            {
                r.AddExplosionForce(explosionForce, transform.position, damageRadious);
            }
        }
    }

}
