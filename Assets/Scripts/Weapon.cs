using System;
using System.Collections;
using Unity.Mathematics;
using UnityEditor.Rendering;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public float bulletVelocity = 25;
    public float bulletLifetime = 3f;

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
