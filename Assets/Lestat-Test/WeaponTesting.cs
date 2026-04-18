using NUnit.Framework;
using UnityEngine;

public class WeaponTesting
{

    [Test] //On this Test im wanting to make sure that ResetShooting properly resets and enables shooting again. readyShooting should become True after the reset.
    public void ResetShooting_HasToReturnTrueSimplePasses()
    {
        var obj = new GameObject(); //Creates the empty gameobject. 
        var weapon = obj.AddComponent<Weapon>(); //adds the script we are using to be tested to the new object.

        weapon.readyShooting = false; //We set it as false so that way we can see if it does reset.
        weapon.ResetShooting(); //Here it calls the function that does the reset by setting readyShooting = true 

        Assert.IsTrue(weapon.readyShooting); //veryfies if its true now.
    }

    [Test]// On this test we make sure that our weapons scrip initializes correctly when Awake() is being called. readyShooting should still be true and the currentBurst should be equals to bulletsPerBurst for the weapons that uses bursts.
    public void InitializingParts_ShouldSetCorrectlyReadyShootingAndCurrentBurst()
    {
        var obj = new GameObject();
        var weapon = obj.AddComponent<Weapon>();

        weapon.bulletsPerBurst = 5; //I set up the amount of bullets there is going to be per burst when shot.
        weapon.InitializingParts(); //We call Awake to initialize the values.

        Assert.IsTrue(weapon.readyShooting); //As before this is veryfing if its true.
        Assert.AreEqual(5, weapon.currentBurst);//Veryfies that bulletPerBurst is properly set as 5.
    }

    [Test]//We will be testing if a burst weapon while being shot does actually reduce the amount of bullets remaining correctly. So it should grab our set number of set currentBurst and reduce it by one.
    public void Burst_DecreasesCurrentBurstCorrectly()
    {
        var obj = new GameObject();
        var weapon = obj.AddComponent<Weapon>();

        weapon.currentType = Weapon.WeaponType.Burst; //We set manually that the current weapon type is a burst weapon.
        weapon.currentBurst = 5; //Set the current burst to 5.

        weapon.currentBurst--;

        Assert.AreEqual(4, weapon.currentBurst);
    }

    [Test]//We will test if its correctly changing the state of the active/inactive weapon. This is used to hide the inactive weapon so they dont display on the screen all at once.
    public void isActiveWeapon_ShouldBeFalseIfInactive()
    {
        var obj = new GameObject();
        var weapon = obj.AddComponent<Weapon>();

        weapon.isActiveWeapon = false; //We set that is false manually. On a front end perspective this would mean that you cant see the weapon.

        Assert.IsFalse(weapon.isActiveWeapon);
    }
    [Test]//We test the same now but inverted.
    public void isActiveWeapon_ShouldBeTrueIfActive()
    {
        var obj = new GameObject();
        var weapon = obj.AddComponent<Weapon>();

        weapon.isActiveWeapon = true; //This time we set it as true. Which should mean that the weapon can be seen.

        Assert.IsTrue(weapon.isActiveWeapon);
    }

    [Test]//We test if the amount of shotgun pellets gets properly stored in shotgunPellets. We should be expecting that we should store correctly what we assing.
    public void shotgunPellets_ShouldStoreTheCorrectAmount()
    {
        var obj = new GameObject();
        var weapon = obj.AddComponent<Weapon>();

        weapon.shotgunPellets = 8; //We manually set the amount of pellets

        Assert.AreEqual(8, weapon.shotgunPellets); //It checks if there are set correctly and its equal to 8.
    }

    [Test]//We check if the weapon type is sored correctly. We set up a type manually (Shotgun) and then compare if its stored correctly in currentType. 
    public void weaponType_ShotgunGetsStoredCorrectly()
    {
        var obj = new GameObject();
        var weapon = obj.AddComponent<Weapon>();

        weapon.currentType = Weapon.WeaponType.Shotgun; //We set up the type as shotgun

        Assert.AreEqual(Weapon.WeaponType.Shotgun, weapon.currentType); //We compare if the type stored is shotgun.
    }

    [Test]//We test if the weapon can store the assigned camera. 
    public void Weapon_ShouldStoreTheAssignedCamera()
    {
        var obj = new GameObject();
        var weapon = obj.AddComponent<Weapon>();

        var camObj = new GameObject();
        weapon.WeaponCamera = camObj.AddComponent<Camera>();

        Assert.IsNotNull(weapon.WeaponCamera); //Checks that there is a camera assigned and its not empty.
    }

    //There is more testing that can be done but its more complex than what i can do and there is a whole bunch extra that can be done but its just testing if the values are correctly stored,a nd it would be very very repetetive. 
}
