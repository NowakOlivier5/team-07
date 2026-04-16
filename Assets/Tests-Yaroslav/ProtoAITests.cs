using NUnit.Framework;
using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TestTools;

public class ProtoAITests
{
    private GameObject protoObject;
    private GameObject childObject;
    private GameObject targetObject;

    private ProtoAI protoAI;
    private NavMeshAgent navAgent;
    private Rigidbody childRb;
    private Animator childAnimator;
    private FPSController playerController;

    [SetUp]
    public void Setup()
    {
        // Monster AI objects
        protoObject = new GameObject("ProtoAI_TestObject");
        navAgent = protoObject.AddComponent<NavMeshAgent>();
        protoAI = protoObject.AddComponent<ProtoAI>();

        // Child object required by the Start() method
        childObject = new GameObject("ProtoChild");
        childObject.transform.SetParent(protoObject.transform);
        childRb = childObject.AddComponent<Rigidbody>();
        childAnimator = childObject.AddComponent<Animator>();

        // Target/player object
        targetObject = new GameObject("PlayerTarget");
        int playerLayer = LayerMask.NameToLayer("Player");
        targetObject.layer = playerLayer;

        // Adding FPSController to player object
        playerController = targetObject.AddComponent<FPSController>();
        targetObject.AddComponent<CharacterController>();
        GameObject camObj = new GameObject("TestCamera");
        playerController.playerCamera = camObj.AddComponent<Camera>();

        // Assigning ProtoAI 
        protoAI.target = targetObject.transform;
        protoAI.player = playerController;
        protoAI.protoVisionRange = 10f;
        protoAI.monsterHealth = 20;
        protoAI.playerLayer = 1 << playerLayer;

        // Calling Start() method manually for the tests
        InvokePrivateMethod(protoAI, "Start");
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(protoObject);
        Object.DestroyImmediate(targetObject);
    }

    // Test Cases

    [Test] // Testing if the monster takes damage
    public void MonsterLosesHealth()
    {
        protoAI.monsterHealth = 20;

        protoAI.Die(5);

        Assert.AreEqual(15, protoAI.monsterHealth);
    }

    [Test] // Testing if the monster dies when health => 0
    public void MonsterDiesWhenHealthUnderZero()
    {
        protoAI.monsterHealth = 5;

        protoAI.Die(10);

        Assert.IsFalse(navAgent.enabled, "NavMeshAgent should be disabled on death");
        Assert.IsFalse(protoAI.enabled, "ProtoAI script should be disabled on death");
        Assert.IsFalse(childAnimator.enabled, "Animator should be disabled on death");
        Assert.IsFalse(childRb.isKinematic, "Rigidbody should stop being kinematic on death");
        Assert.IsTrue(childRb.useGravity, "Rigidbody gravity should be enabled on death");
    }

    [Test] // Testing that the monster is not affected by gravity, is kinematic and attack/lunge cooldowns are off on start
    public void OnStartMonsterInitialisesCorrectly()
    {
        Assert.IsTrue(childRb.isKinematic, "Child rigidbody should be kinematic on Start");
        Assert.IsFalse(childRb.useGravity, "Child rigidbody gravity should be disabled on Start");

        bool attackCooldown = GetPrivateField<bool>(protoAI, "attackCooldown");
        bool lungeCooldown = GetPrivateField<bool>(protoAI, "lungeCooldown");

        Assert.IsFalse(attackCooldown, "Attack cooldown should start false");
        Assert.IsFalse(lungeCooldown, "Lunge cooldown should start false");
    }

    [Test] // Testing if the monster detects when in range
    public void DetectPlayerWhenInRange()
    {
        protoObject.transform.position = Vector3.zero; // A position of (0, 0, 0)
        targetObject.transform.position = Vector3.one * 2f; // A position of (2f, 2f, 2f) which is within the range of a sphere with a radius of 2f at position (0, 0, 0)

        Physics.SyncTransforms(); // Syncs physics

        InvokePrivateMethod(protoAI, "DetectPlayer");

        bool playerVisible = GetPrivateField<bool>(protoAI, "playerVisible");
        Assert.IsTrue(playerVisible, "Player should be detected");
    }

    [Test] // Testing if the monster detects when out of range
    public void DoNotDetectPlayerWhenNotInRange()
    {
        protoObject.transform.position = Vector3.zero; // A position of (0, 0, 0)
        targetObject.transform.position = Vector3.one * 20f; // A position of (20f, 20f, 20f) which is outside the range of a sphere with a radius of 20f at position (0, 0, 0)

        Physics.SyncTransforms(); // Syncs the positions

        InvokePrivateMethod(protoAI, "DetectPlayer");

        bool playerVisible = GetPrivateField<bool>(protoAI, "playerVisible");
        Assert.IsFalse(playerVisible, "Player should not be detected");
    }

    // Extra functions to allow the test cases to run by accessing private methods/variables
    // This function is used to invoke private methods from within the ProtoAI script
    private static void InvokePrivateMethod(object target, string methodName)
    {
        // Look for methods within the specified class that are an instance and private
        MethodInfo method = target.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);

        method.Invoke(target, null);
    }

    // This function is used to get private values from within the ProtoAI script
    private static Value GetPrivateField<Value>(object target, string fieldName)
    {
        // Works roughly the same as in the previous method except we are looking for variables instead
        FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);

        return (Value)field.GetValue(target);
    }
}