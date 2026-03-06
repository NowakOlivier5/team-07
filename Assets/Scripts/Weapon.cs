using System;
using System.Collections;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;
using NUnit.Framework;
public class Weapon : MonoBehaviour
{
    public Camera playerCamera;
    public GameObject bulletPrefab; //The bullet
    public bool isActiveWeapon;
    public Transform bulletSpawn; //The bullet spawner
    public float bulletVelocity = 25; //The default velocity of the bullet
    public float bulletLifetime = 3f; //The bullet "air time"
    public enum WeaponType //Shooting types
    {
        Single,
        Burst,
        Automatic,
        Shotgun
    }

    public bool isShooting, readyShooting;
    bool allowReset = true;
    public float delayShot = 2f; //Delay between shots.
    public float shootingSpread; //The spread of the bullets when being shot.

    //Shooting modes
    public int bulletsPerBurst = 3; //If shooting a burst of bullets it would be how many bullets per burst-
    public int currentBurst; //To work with the burst that just got shot. and not letting it behave like a full automatic.

    public WeaponType currentType; //how we are going to compare in if statements for the weapon to have the corresponding behaviours.

    public Vector3 spawnPosition; //This I might remove later, but for now is in case you guys decide to make weapons a "pick up" we need to tell the object to what position should go when picked up, so its good to be able to set that information.
    public Vector3 spawnRotation;

    private void Awake()
    {
        readyShooting = true;
        currentBurst = bulletsPerBurst; //when ther are no more bullets in that burst, means that the burst is over.
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
            else if (currentType == WeaponType.Single || currentType == WeaponType.Burst || currentType == WeaponType.Shotgun)
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
        //We said that we cant start shooting once the shooting started.    
        readyShooting = false;
        Vector3 directionOfShot = DirectionAndSpreadCal().normalized;

        //Instantiating the bullet.
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);

        //This is a pointer, points at the direction we are shooting.
        bullet.transform.forward = directionOfShot;
        //Force that shoots the bullet from spawn position (gun) in certain direction. (Foward is the blue axis on the little compass thingy).

        //Shooting the bullet
        bullet.GetComponent<Rigidbody>().AddForce(directionOfShot * bulletVelocity, ForceMode.Impulse);//"Impulse" is the way that the force will work.

        StartCoroutine(DestroyBullet(bullet, bulletLifetime)); //Removes the bullet after certain delayed applied to the bullet.

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


    public Vector3 DirectionAndSpreadCal()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))//When we shoot at something we get the direction of where the bullet has to go.
        {
            targetPoint = hit.point;
        }
        else
        {//When we shoot to nothing like the air, is how we get the flying direction of the bullet.
            targetPoint = ray.GetPoint(100);
        }

        Vector3 direction = targetPoint - bulletSpawn.position;

        //Creating the random spread of bullets. The spread varies on both axys.
        float x = UnityEngine.Random.Range(-shootingSpread, shootingSpread);
        float y = UnityEngine.Random.Range(-shootingSpread, shootingSpread);

        //Returning spread and direction
        return direction + new Vector3(x, y, 0);
    }

    private IEnumerator DestroyBullet(GameObject bullet, float bulletLifetime)
    {//Because we are using a Coroutine, we need to use a different type of metod, IEnumerator, This allows us to stop the process at a specific moment and return the part that completed or return nothing.
        yield return new WaitForSeconds(bulletLifetime); //returns after a delay.
        Destroy(bullet);
    }
}
