using NUnit.Framework;
using UnityEngine;

public class DestroyTests
{
    private GameObject testObject;
    private Destroy destroyScript;
    private Rigidbody rb;

    [SetUp]
    //the setup is needed for the tests below,
    //the test object will be used for the tests
    //and initial health is set to 5
    public void Setup()
    {
        testObject = new GameObject("TestObject");
        rb = testObject.AddComponent<Rigidbody>();
        destroyScript = testObject.AddComponent<Destroy>();

        destroyScript.rb = rb; //rigid body
        destroyScript.objectHealth = 5;//object health sets to 5
        destroyScript.isGlass = false;//glass has a seperate script not used in these tests
    }

    [TearDown]
    //resets the test object after each test is ran
    public void TearDown()
    {
        Object.DestroyImmediate(testObject);
    }

    [Test]
    //this test is for object health that is reducted when an object not
    //classed as glass is damaged
    public void ObjectHealth_TakeDamage()
    {
        destroyScript.TakeDamage(2);//object starts at 5, reduces by 2

        Assert.AreEqual(3, destroyScript.objectHealth);//expected health is 3
    }

    [Test]
    //this test is for object health to remain the same
    //after recieving 0 damage
    public void ObjectHealth_NoDamageTaken()
    {
                                    //object health starts at 5
        destroyScript.TakeDamage(0);//object takes 0 damage

        Assert.AreEqual(5, destroyScript.objectHealth);//expected health should be 5
        Assert.IsTrue(destroyScript.enabled);//script should be running
    }

    [Test]
    //this test is for how an object should handle
    //mutliple reduced damage at the same time
    public void Object_MultipleDamageTaken()
    {
                                    //starting health is 5
        destroyScript.TakeDamage(2);//2 simultanious hits
        destroyScript.TakeDamage(1);//of 2 damage and 1 damage

        Assert.AreEqual(2, destroyScript.objectHealth);//remaining health is expected 2
    }

    [Test]
    //this test is for when an object takes damage and it's health
    //goes below 0 and is destroyed, by using gravity to move the object
    //ie. once a wall is destroyed it's seperate parts will have physics move by gravity
    public void ObjectHealthBelowZero_EnableGravity()
    {
        destroyScript.TakeDamage(8);//starting health 5, damage taken for 8
                                    
        Assert.IsFalse(rb.isKinematic);//kinematic should be disabled
        Assert.IsTrue(rb.useGravity);//gravity should be enabled
        Assert.IsFalse(destroyScript.enabled);//script expected to be disabled
        Assert.AreEqual(-3, destroyScript.objectHealth);//expected health is -3
    }

    [Test]
    //this test is for when an object takes damage and it's health
    //hits 0 exactly, enables gravity to move the object
    //ie. once a wall is destroyed its' seperate parts will have physics and move by gravity
    public void ObjectHealthExactlyZero_EnableGravity()
    {
        destroyScript.objectHealth = 7;//health is specifically 
                                       //set to 7 for this test
        destroyScript.TakeDamage(7);   //and takes 7 damage

        Assert.AreEqual(0, destroyScript.objectHealth);//health is expected to be 0
        Assert.IsFalse(rb.isKinematic);//kinematic expected to be disabled
        Assert.IsTrue(rb.useGravity);//gravity is expected to be enabled
        Assert.IsFalse(destroyScript.enabled);//script expected to be disabled
    }
}
