using System;
using Unity.VisualScripting;
using UnityEngine;

public class ThrowItems : MonoBehaviour
{
    [SerializeField] float activationDelay = 2f; //Like i said before for the missile, it keeps encapsulation by keeping it private but to the same time gives us access to it on the unity editor by having it SerializeField. This is also the delay of activation of a throwable like a granade for example.
    [SerializeField] float damageRadious = 3f; //The explosion radious.
    [SerializeField] float explosionForce = 5000f;//The explosion force, this is the same as the missile, just different values.
    private float activationCountdown; //The countdown that will trigger the object after throwing it.
    private bool alreadyActive = false; //Checking if the object activated, for example if its a granade. Has it exploded yet?
    public bool throwned = false; //Has the object been throwend? 
    public int tDamage; //Thorwable Damage.

    public enum ThrowableType //Same as with the enum for the weapons. 
    {
        Granade
    }

    public ThrowableType currentType;

    private void Start()
    {
        activationCountdown = activationDelay; //Like i said before this is so it counts how much time apsses before the item needs to activate/trigger.
    }

    private void Update()
    {
        if (throwned)
        {
            activationCountdown -= Time.deltaTime;
            if (activationCountdown <= 0f && !alreadyActive) //If there is still time left and hasnt been active. then activate the object.
            {
                ActivateThrowable();
                alreadyActive = true; //So it doesnt activate more than once.
            }
        }
    }

    private void ActivateThrowable()
    {
        GetEffect(); //Gets the effect of the throwable, for example a granade would be the explosion force and the damage.
        Destroy(gameObject); //destoys the throwable after it does the effect.
    }

    private void GetEffect()
    {
        if (currentType == ThrowableType.Granade) //If the type is granade then do the effect of an explosive granade.
        {
            ExplosiveGranade();
        }
    }

    private void ExplosiveGranade()
    {//For the granade effect ill reuse the missile behaviour. this will save some time because part of it is very similar. Same goes for the damage part, the main change will be how this object works, the other is a projectile that shoots and activates on impact, this will land and go off after a few seconds.
        Collider[] colliders = Physics.OverlapSphere(transform.position, damageRadious);
        foreach (Collider objectInRange in colliders)
        {
            Rigidbody r = objectInRange.GetComponent<Rigidbody>();
            if (r != null)
            {
                r.AddExplosionForce(explosionForce, transform.position, damageRadious);
            }
            if (objectInRange.gameObject.GetComponentInParent<ProtoAI>())
            {
                objectInRange.gameObject.GetComponentInParent<ProtoAI>().Die(tDamage);
            }
            Destroy destructible = objectInRange.GetComponent<Destroy>();
            if (destructible != null)
            {
                destructible.TakeDamage(tDamage);
            }
        }
    }
}
