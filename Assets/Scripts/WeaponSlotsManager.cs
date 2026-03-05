using System.Collections.Generic;
using UnityEngine;

public class WeaponSlotsManager : MonoBehaviour
{
    public static WeaponSlotsManager Instance { get; set; }
    public List<GameObject> weaponSlots;
    public GameObject activeSlot;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void Start()
    {
        activeSlot = weaponSlots[0];
    }

    private void Update()
    {
        foreach (GameObject weaponSlots in weaponSlots)
        {
            if (weaponSlots == activeSlot)
            {
                weaponSlots.SetActive(true);
            }
            else
            {
                weaponSlots.SetActive(false);
            }
        }
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
    }
    private void SwitchSlot(int slotNumber)
    {
        if (activeSlot.transform.childCount > 0)
        {
            Weapon currentWeapon = activeSlot.transform.GetChild(0).GetComponent<Weapon>();
            currentWeapon.isActiveWeapon = false;
        }
        activeSlot = weaponSlots[slotNumber];

        if (activeSlot.transform.childCount > 0)
        {
            Weapon newWeapon = activeSlot.transform.GetChild(0).GetComponent<Weapon>();
            newWeapon.isActiveWeapon = true;
        }
    }
}
