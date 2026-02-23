using UnityEngine;

public class Bullet : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Target"))
        { //detecting collision, so in case it hits a target it destroys the bullet.
            print("hit" + collision.gameObject.name + " !");
            Destroy(gameObject);
        }
    }
}
