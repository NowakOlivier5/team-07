using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int bDamage;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Target")) //change taget to what ever the enemy or npc will have as tags
        { //detecting collision, so in case it hits a target it destroys the bullet.
            Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("WorldMap"))//I gave that tag to the objects in "Test Walls" and "Terrain" so the bullet dispawns when hitting those things. We could change it later no problem. 
        //If you comment this out the bullets will just bounce and then dispawn, can be nice for some things but not for a pistol.
        {
            Destroy(gameObject); //The destroy has to go first, if not the bullets will bounce anyways from any surface.
            Destroy destructible = collision.gameObject.GetComponent<Destroy>(); //Checks for collision to the objects that have the "Destroy" script
            destructible.TakeDamage(bDamage); // After detecting collision applies the weapon damage to the object shot by substracting the values of the weapon stats.

        }
        if (collision.gameObject.CompareTag("ProtoAgent")) //Same as before for the terrain but for the enemy.
        {
            Destroy(gameObject);
            ProtoAI enemy = collision.gameObject.GetComponentInParent<ProtoAI>();
            enemy.Die(bDamage);
        }
    }
}
