
using System.Collections;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;

//I did my code following a few youtube tutorials. This is my first time working with unity for a 3d game. 
//Reference 1: https://www.youtube.com/watch?v=swOfmyJvb98&list=PLtLToKUhgzwm1rZnTeWSRAyx9tl8VbGUE
//Reference 2: https://www.youtube.com/watch?v=0ezqRpkNid8&list=PLZ1b66Z1KFKinfPc4Ny9CDjIwcHEz0zc4&index=5 
public class Weapon : MonoBehaviour
{
    public Camera WeaponCamera;
    public GameObject bulletPrefab; //The bullet
    public bool isActiveWeapon;
    public Transform bulletSpawn; //The bullet spawner
    public float bulletVelocity = 25; //The default velocity of the bullet
    public float bulletLifetime = 3f; //The bullet "air time"
    public enum WeaponType //Shooting types. An enum is a collection of constant, we use it as a list of types. And those types have properties that we modified and configurations related as how they shoot.
    {
        Single,
        Burst,
        Automatic,
        Shotgun,
        RPG
    }

    public bool isShooting, readyShooting;
    bool allowReset = true;
    public float delayShot = 2f; //Delay between shots.
    public float shootingSpread; //The spread of the bullets when being shot.

    public int damage;
    //Shooting modes
    public int bulletsPerBurst = 3; //If shooting a burst of bullets it would be how many bullets per burst-
    public int currentBurst; //To work with the burst that just got shot. and not letting it behave like a full automatic. I Tried using this for a shotgun but it doesnt put any kind of delay between shots to the point that you can spam click and it would shoot as fast you click. So ill keep it for a future because if we keep working on this after this semester we will end up making more types of weapons. 

    public WeaponType currentType; //how we are going to compare in if statements for the weapon to have the corresponding behaviours.

    private Animator animator; //The aniamtor plays the animations of the weapon after receiving the correct "trigger" 
    public int shotgunPellets = 5; //This is the amount of pellets the shotgun will shoot per shot.
    private void Awake()
    {
        readyShooting = true;
        currentBurst = bulletsPerBurst; //when ther are no more bullets in that burst, means that the burst is over.
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (isActiveWeapon) //If the weapon is not the one in hand doesnt shoot.
        {
            GetComponent<Renderer>().enabled = true;
            if (currentType == WeaponType.Automatic)
            {
                //Only shoots if holding the click
                isShooting = Input.GetKey(KeyCode.Mouse0);//GetKey is for holding the button
            }
            else if (currentType == WeaponType.Single || currentType == WeaponType.Burst || currentType == WeaponType.Shotgun || currentType == WeaponType.RPG)
            {
                isShooting = Input.GetKeyDown(KeyCode.Mouse0); //GetKeyDown when pressing only once.
            }
            if (readyShooting && isShooting && !PauseMenu.isPaused)
            {
                currentBurst = bulletsPerBurst;
                FireWeapon();
            }
        }
        else
        {
            GetComponent<Renderer>().enabled = false;
        }
    }

    public void ResetShooting() //making sure that we are allowed to shoot, and allowing to reset the shooting in case we start the process of shooting.
    {
        readyShooting = true;
        allowReset = true;
    }

    private void FireWeapon()
    {
        animator.SetTrigger("RECOIL");
        readyShooting = false;
        if (currentType != WeaponType.RPG)
        {
            if (currentType == WeaponType.Shotgun)
            {
                float startingSpread = shootingSpread;

                for (int i = 0; i < shotgunPellets; i++) //It applies this to every pellet, if we want to do more pellets we just change the number once where we declare shotgunPellets.
                {
                    Ray ray = WeaponCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); //Had issues with the shotgun. So i searched different solutions. The tutorial i was following did not go in to detail. So i used the way we spread for the rifle and pistol. But it wasnt working, it had a few issues, had to also deactivate collision from bullets with bullets because they where dispawning on contact as soon as it was shot, I just created a layer for them and deactivate them. I used the tutorial way of doing the spread, but apllied an off set. While also creaating a random pattern by modifying at random the X and Y direction fo the pellets. 
                    //Reference: https://stackoverflow.com/questions/47889977/unity-shotgun-making
                    //This person had the same problem that i was having. And someone in the comments explained why was happening what it was happening. It helped me to understand how the envirionment works
                    //Reference: https://www.reddit.com/r/Unity3D/comments/pjbqmh/bullet_spread_only_works_when_i_shoot_specific/
                    Vector3 direction = ray.direction;

                    //Like i explained when i did the general spread. This sets the main spread area on a random range from the values we set.
                    float x = Random.Range(-shootingSpread, shootingSpread);
                    float y = Random.Range(-shootingSpread, shootingSpread);

                    float shotgunOffset = 0.1f;

                    //We do add a bit of an extra offset for the shotgun because it has bigger spread and we want some level of randomness on the pellets. And then like before we handle the direction from the weapon camera transforming it right and up.
                    direction += WeaponCamera.transform.right * (x + Random.Range(-shotgunOffset, shotgunOffset));
                    direction += WeaponCamera.transform.up * (y + Random.Range(-shotgunOffset, shotgunOffset));

                    Vector3 directionOfShot = direction.normalized;

                    GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);
                    Bullet bulletDamage = bullet.GetComponent<Bullet>();
                    bulletDamage.bDamage = damage;

                    bullet.transform.forward = directionOfShot;
                    bullet.GetComponent<Rigidbody>().AddForce(directionOfShot * bulletVelocity, ForceMode.Impulse);

                    StartCoroutine(DestroyBullet(bullet, bulletLifetime));
                }
                shootingSpread = startingSpread;
            }
            else
            {
                //We said that we cant start shooting once the shooting started.    
                Vector3 directionOfShot = DirectionAndSpreadCal().normalized;

                //Instantiating the bullet.
                GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);

                Bullet bulletDamage = bullet.GetComponent<Bullet>();
                bulletDamage.bDamage = damage;

                //This is a pointer, points at the direction we are shooting.
                bullet.transform.forward = directionOfShot;
                //Force that shoots the bullet from spawn position (gun) in certain direction. (Foward is the blue axis on the little compass thingy).

                //Shooting the bullet
                bullet.GetComponent<Rigidbody>().AddForce(directionOfShot * bulletVelocity, ForceMode.Impulse);//"Impulse" is the way that the force will work.

                StartCoroutine(DestroyBullet(bullet, bulletLifetime)); //Removes the bullet after certain delayed applied to the bullet.

            }
            //The same way we check if we are allowed to start shooting we check if we are done shooting.
            if (allowReset)
            {
                Invoke("ResetShooting", delayShot);
                allowReset = false;
            }

            //Checking if we are shooting in burst
            if (currentType == WeaponType.Burst && currentBurst > 1) //Makes sure that if the weapon has still bullets to be shot in this burst reduces the amount left.
            {
                currentBurst--;
                Invoke("FireWeapon", delayShot);
            }
        }
        else
        {
            readyShooting = false;
            Vector3 directionOfShot = DirectionAndSpreadCal().normalized;
            GameObject missile = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);

            Missile missileDamage = missile.GetComponent<Missile>();
            missileDamage.mDamage = damage;

            missile.transform.forward = directionOfShot;

            missile.GetComponent<Rigidbody>().AddForce(directionOfShot * bulletVelocity, ForceMode.Impulse);

            if (allowReset)
            {
                Invoke("ResetShooting", delayShot);
                allowReset = false;
            }

        }

    }

    public Vector3 DirectionAndSpreadCal()
    {//Reference: https://www.youtube.com/watch?v=xgOJwDSARmo&list=PLtLToKUhgzwm1rZnTeWSRAyx9tl8VbGUE&index=3
        Ray ray = WeaponCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); //This is created from the "center" of the weapon camera. The values assigned are the center of the screen. 

        Vector3 finalDirection = ray.direction; //This is setting that the default shooting direction is straight and foward from the player camera(Weapon Camera is a "sub camera" of Player camera).

        //This are the random values that woudl vary from the spread we input. This generates a random offset for the shot on a square shape. 
        float x = Random.Range(-shootingSpread, shootingSpread);
        float y = Random.Range(-shootingSpread, shootingSpread);

        //Reference: https://stackoverflow.com/questions/47889977/unity-shotgun-making
        finalDirection += WeaponCamera.transform.right * x; //Transform right generates the spread change horizontaly, even though it says right it also includes left cia positive and negative values on the x axys (Positive right, negative left)
        finalDirection += WeaponCamera.transform.up * y; //Similar to transform right, but in this case up and down in the Y axys. This gets added to final direction after exiting on the "foward direction" from the ray casting.

        return finalDirection.normalized; //Adding normalize ensures us that keeps consistency.
    }

    private IEnumerator DestroyBullet(GameObject bullet, float bulletLifetime)
    {//Because we are using a Coroutine, we need to use a different type of metod, IEnumerator, This allows us to stop the process at a specific moment and return the part that completed or return nothing.
        yield return new WaitForSeconds(bulletLifetime); //returns after a delay.
        Destroy(bullet);
    }
}
