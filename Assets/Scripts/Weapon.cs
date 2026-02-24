using System;
using System.Collections;
using Unity.Mathematics;
using UnityEditor.Rendering;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    //public Camera playerCamera;
    public GameObject bulletPrefab; //The bullet
    public Transform bulletSpawn; //The bullet spawner
    public float bulletVelocity = 25; //The default velocity of the bullet
    public float bulletLifetime = 3f; //The bullet "air time"
    public enum WeaponType
    {
        Single,
        Burst,
        Automatic
    }

    public bool isShooting, readyShooting;
    bool allowReset = true;
    public float delayShot = 2f; //Delay between shots.
    public float shootingSpread; //The spread of the bullets when being shot.

    //Shooting modes
    public int bulletsPerBurst = 3; //If shooting a burst of bullets it would be how many bullets per burst-
    public int currentBurst; //To work with the burst that just got shot. and not letting it behave like a full automatic.

    public WeaponType currentType;

    private void Awake()
    {
        readyShooting = true;
        currentBurst = bulletsPerBurst; //when ther are no more bullets in that burst, means that the burst is over.
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            FireWeapon();
        }
    }

    private void FireWeapon()
    {
        //Instantiating the bullet.
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);
        //Shooting the bullet
        bullet.GetComponent<Rigidbody>().AddForce(bulletSpawn.forward.normalized * bulletVelocity, ForceMode.Impulse); //Force that shoots the bullet from spawn position (gun) in certain direction. (Foward is the blue axis on the little compass thingy). "Impulse" is the way that the force will work.

        StartCoroutine(DestroyBullet(bullet, bulletLifetime)); //Removes the bullet after certain delayed applied to the bullet.
    }

    private IEnumerator DestroyBullet(GameObject bullet, float bulletLifetime)
    {//Because we are using a Coroutine, we need to use a different type of metod, IEnumerator, This allows us to stop the process at a specific moment and return the part that completed or return nothing.
        yield return new WaitForSeconds(bulletLifetime); //returns after a delay.
        Destroy(bullet);
    }
}
