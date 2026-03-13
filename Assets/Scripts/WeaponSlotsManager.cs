using System;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSlotsManager : MonoBehaviour
{
    public Camera playerCamera;
    public List<GameObject> weaponSlots; //The list that is going to handle the weapon slots.
    public GameObject activeSlot;
    public float throwingForce = 10f; //default force that we will throw or throwables.
    public float fMultiplier = 0;
    public float fMCap = 2f; // To cap the multiplier
    public GameObject granadePrefab; //The granade object
    public GameObject throwableSpawn;

    public void Start()
    {
        activeSlot = weaponSlots[0]; //We start as out active weapon slot the first one on the array.
    }

    private void Update()
    {
        foreach (GameObject weaponSlots in weaponSlots)
        {
            if (weaponSlots == activeSlot) //This is mainly checking that we have correct slot on the correct active weapon. So when we shoot, we shoot only the equiped gun.
            {
                weaponSlots.SetActive(true);
            }
            else
            {
                weaponSlots.SetActive(false);
            }
        }
        //This ifs is basically how we change weapon slots, for now i made 3 to experiment.
        if (Input.GetKey(KeyCode.Alpha1))
        {
            SwitchSlot(0);
        }
        if (Input.GetKey(KeyCode.Alpha2))
        {
            SwitchSlot(1);
        }
        if (Input.GetKey(KeyCode.Alpha3))
        {
            SwitchSlot(2);
        }
        if (Input.GetKey(KeyCode.Alpha4))
        {
            SwitchSlot(3);
        }

        if (Input.GetKey(KeyCode.G))
        {
            fMultiplier += Time.deltaTime; //delta time is the time that passed since the previous frame. This is so the more we hold G the more strong is going to force that we use to throw the granade. Thats why the grande get throwend when releasing G.

            if (fMultiplier > fMCap) //If the multiplier is bigger than the cap then will force it to be the cap, that way it doesnt keep increasing the multiplier for ever.
            {
                fMultiplier = fMCap;
            }
        }
        if (Input.GetKeyUp(KeyCode.G))
        {
            ThrowThrowable(); //When we let go of the key G throws our throwable item.
            fMultiplier = 0;//We need to reset the force multiplier, because if we dont do it the grande next time will come out max force even if we didnt charge it.
        }
    }

    private void ThrowThrowable()
    {
        GameObject granade = granadePrefab; //We get the prefab so we can instantiate it.
        GameObject throwable = Instantiate(granade, throwableSpawn.transform.position, playerCamera.transform.rotation); //We instatiante it and we make it spawn relatively to the direction of the camera.

        Rigidbody r = throwable.GetComponent<Rigidbody>();
        r.AddForce(playerCamera.transform.forward * (throwingForce * fMultiplier), ForceMode.Impulse); //We choose the foward direction relative to the camera, then with the force that will change depending on how long we hold the corresponding key, we will throw further, and the forcemode impulse has to do with the way that the force behaves

        throwable.GetComponent<ThrowItems>().throwned = true; //With this we let "ThrowItems" know that the item has been thrown.
    }

    private void SwitchSlot(int slotNumber) //It receives a number and that number is the slot. 
    { //SO to avoid CRASHING in case that for some reason we decide to remove a weapon from the slots we do some check ups. It also brings all the components of the weapons so they mantain their own behaviours.
        if (activeSlot.transform.childCount > 0) //Makes sure that the slot (Parent) has a weapon (Child), and then deactivates it, because we are switching weapons, so we dont want both weapons active at the same time.
        {
            Weapon currentWeapon = activeSlot.transform.GetChild(0).GetComponent<Weapon>();
            currentWeapon.isActiveWeapon = false;
        }
        activeSlot = weaponSlots[slotNumber]; //Makes the number we selected the active slot.

        if (activeSlot.transform.childCount > 0) // Then checks again but makes the weapon on the slot that we introduced the active weapon allowing us to use the new gun that we switched to.
        {
            Weapon newWeapon = activeSlot.transform.GetChild(0).GetComponent<Weapon>();
            newWeapon.isActiveWeapon = true;
        }
    }
}
