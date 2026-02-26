using UnityEngine;

public class Bullet : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Target")) //change taget to what ever the enemy or npc will have as tags
        { //detecting collision, so in case it hits a target it destroys the bullet.
            print("hit" + collision.gameObject.name + " !");
            Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("WorldMap"))//I gave that tag to the objects in "Test Walls" and "Terrain" so the bullet dispawns when hitting those things. We could change it later no problem. 
        //If you comment this out the bullets will just bounce and then dispawn, can be nice for some things but not for a pistol.
        {
            print("hit terrain/world" + collision.gameObject.name + " !");
            Destroy(gameObject);
        }
        if (collision.gameObject.CompareTag("ProtoAgent"))
        {
            ProtoAI enemy = collision.gameObject.GetComponent<ProtoAI>();
            enemy.Die();
        }
    }
}
