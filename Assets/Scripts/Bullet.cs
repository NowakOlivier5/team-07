using UnityEngine;
//I used the following references to do bullet holes. I also used it for the missile i just had to move a few things for it
//Reference: https://www.youtube.com/watch?v=glx4qolDnHg 
//for the bullets i used the same tutorial referenced on weapons. THat was my main source of learning while ahving to change things. For example the creator used singletons, but after reading about them i found out that they are not a good programing practice. So i had to avoid using them.
//References: https://www.youtube.com/watch?v=swOfmyJvb98&list=PLtLToKUhgzwm1rZnTeWSRAyx9tl8VbGUE

public class Bullet : MonoBehaviour
{
    public int bDamage;
    public GameObject BulletHolePrefab;
    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject); //I notice that it was a bit unecessary to do the destroy seperatly when we always want it to be destroyed no matter what it collides with.
        ContactPoint pointOfImpact = collision.contacts[0];

        if (collision.gameObject.CompareTag("WorldMap"))//I gave that tag to the objects in "Test Walls" and "Terrain" so the bullet dispawns when hitting those things. We could change it later no problem. 
        {
            if (BulletHolePrefab != null) //We are making sure that if there is a prefab we make the png spawn on the point of inpact.
            {
                Quaternion rotation = Quaternion.LookRotation(pointOfImpact.normal);
                GameObject bulletHole = Instantiate(BulletHolePrefab, pointOfImpact.point + pointOfImpact.normal * 0.01f, rotation);

                bulletHole.transform.SetParent(collision.transform);//So it sticks to the surface it hits.
                Destroy(bulletHole, 30f);
            }


            Destroy destructible = collision.gameObject.GetComponent<Destroy>(); //Checks for collision to the objects that have the "Destroy" script
            if (destructible != null) //If there is a scrip that makes it destructible then it applies the damage. If we do not do this it causes an alert on unity.
            {
                destructible.TakeDamage(bDamage); // After detecting collision applies the weapon damage to the object shot by substracting the values of the weapon stats.
            }


        }
        if (collision.gameObject.CompareTag("ProtoAgent")) //Same as before for the terrain but for the enemy.
        {
            ProtoAI enemy = collision.gameObject.GetComponentInParent<ProtoAI>();
            enemy.Die(bDamage);
        }
    }
}
