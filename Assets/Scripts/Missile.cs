using Unity.VisualScripting;
using UnityEngine;

public class Missile : MonoBehaviour
{
    [SerializeField] float damageRadious = 5f; //The default "explosion radious" Also using serialize it makes it a private variable but still being able to use it as public to ahve access throughout the unity editor. This way is more convinient to modify some of the properties of game objects.
    //Also in a future if we want, we can move this values to the weapons script if we want to make different type of explosive projectiles. But tbf I think an RPG is enough as "big boy explosive laucher" maybe some other kind in the future.
    [SerializeField] float explosionForce = 10000f; //The default force that will push objects with
    public int mDamage;
    //This is mostly the same as the bullet but for a missile.
    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("Target"))
        {
            Destroy(gameObject);
            Explosion();
        }

        if (collision.gameObject.CompareTag("WorldMap"))
        {
            Destroy(gameObject);
            Explosion();
            Destroy destructible = collision.gameObject.GetComponent<Destroy>();
            destructible.TakeDamage(mDamage);
        }
        if (collision.gameObject.CompareTag("ProtoAgent"))
        {
            Destroy(gameObject);
            Explosion();
            ProtoAI enemy = collision.gameObject.GetComponent<ProtoAI>();
            enemy.Die(mDamage);
        }
    }

    public void Explosion()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, damageRadious); //Makes a colider and physics on a sphere that will work as our explosion radious. it puts all the objects in an array/list and then with "foreach" goes item by item applying what we need.
        foreach (Collider objectInRange in colliders) //Applies the same stuff to the colliders that are in range.
        {
            Rigidbody r = objectInRange.GetComponent<Rigidbody>();
            if (r != null) //If the object has a rigid body it applies the force to the object if its in the explosion raidous.
            {
                r.AddExplosionForce(explosionForce, transform.position, damageRadious); //adds force to the objects 
            }
            if (objectInRange.gameObject.GetComponent<ProtoAI>()) //As before and the bullet and the direct impact of the missile, this is the damage of the explosion.
            {
                objectInRange.gameObject.GetComponent<ProtoAI>().Die(mDamage);
            }
            Destroy destructible = objectInRange.GetComponent<Destroy>(); //if is terrain that is in the radious, and its destructible it will take damage/break.
            if (destructible != null)
            {
                destructible.TakeDamage(mDamage);
            }
        }
    }

}
