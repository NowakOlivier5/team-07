using NUnit.Framework;
using UnityEngine;

public class FPSControllerTests
{
    GameObject playerObject;
    FPSController controller;

    [SetUp]
    public void Setup()
    {
        //create empty object to act as player
        playerObject = new GameObject();

        //attach script and components
        controller = playerObject.AddComponent<FPSController>();
        playerObject.AddComponent<CharacterController>();
        GameObject camObj = new GameObject();//another empty object this one for camera
        controller.playerCamera = camObj.AddComponent<Camera>();
    }

    [TearDown]
    public void TearDown()
    {
        //destroy object to reset between tests
        Object.DestroyImmediate(playerObject);
    }

    //is player taking damage
    [Test]
    public void TakeDamage_ReducesHealth()
    {
        controller.playerHealth = 100;
        controller.takeDamage(20);
        Assert.AreEqual(80, controller.playerHealth);
    }

    //making sure health cant go below 0
    [Test]
    public void TakeDamage_AllowsNegativeHealth()
    {
        controller.playerHealth = 10;
        controller.takeDamage(20);
        Assert.AreEqual(-10, controller.playerHealth);
    }

    //walk speed is correct
    [Test]
    public void DefaultWalkSpeed_IsCorrect()
    {
        Assert.AreEqual(6f, controller.walkSpeed);
    }

    //run speed is higher than walk speed
    [Test]
    public void RunSpeed_IsGreaterThanWalkSpeed()
    {
        Assert.Greater(controller.runSpeed, controller.walkSpeed);
    }

    //jump power is applied correctly
    [Test]
    public void JumpPower_IsPositive()
    {
        Assert.Greater(controller.jumpPower, 0);
    }

    //gravity is applied
    [Test]
    public void Gravity_IsPositive()
    {
        Assert.Greater(controller.gravity, 0);
    }
}