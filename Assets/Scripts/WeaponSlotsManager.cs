using System.Collections.Generic;
using UnityEngine;

public class WeaponSlotsManager : MonoBehaviour
{
    public List<GameObject> weaponSlots; //The list that is going to handle the weapon slots.
    public GameObject activeSlot;


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
        if (Input.GetKey(KeyCode.Alpha5))
        {
            SwitchSlot(4);
        }
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
