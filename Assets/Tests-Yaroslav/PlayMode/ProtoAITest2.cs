using NUnit.Framework;
using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;
using UnityEngine.TestTools;

public class ProtoAIPlayModeTests
{
    private GameObject protoObject;
    private GameObject childObject;
    private GameObject targetObject;

    private ProtoAI protoAI;
    private NavMeshAgent navAgent;
    private Rigidbody childRb;
    private Animator childAnimator;
    private FPSController playerController;

    private GameObject floorObject;
    private NavMeshSurface navSurface;

    [UnitySetUp] // A setup that runs for every time
    public IEnumerator Setup()
    {
        // Monster AI objects
        protoObject = new GameObject("ProtoAI_TestObject");
        navAgent = protoObject.AddComponent<NavMeshAgent>();
        protoAI = protoObject.AddComponent<ProtoAI>();

        // Child object required by the Start() method for ProtoAI
        childObject = new GameObject("ProtoChild");
        childObject.transform.SetParent(protoObject.transform);
        childRb = childObject.AddComponent<Rigidbody>();
        childAnimator = childObject.AddComponent<Animator>();

        // Target/player object
        targetObject = new GameObject("PlayerTarget");
        int playerLayer = LayerMask.NameToLayer("Player");
        targetObject.layer = playerLayer;
        targetObject.AddComponent<CharacterController>();

        // Assigning ProtoAI 
        protoAI.target = targetObject.transform;
        protoAI.player = playerController;
        protoAI.protoVisionRange = 10f;
        protoAI.monsterHealth = 20;
        protoAI.playerLayer = 1 << playerLayer;

        // Navmesh baking to allow the AI to work
        floorObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
        floorObject.transform.position = Vector3.zero;
        floorObject.transform.localScale = new Vector3(20f, 1f, 10f);
        navSurface = floorObject.AddComponent<NavMeshSurface>();
        navSurface.collectObjects = CollectObjects.All;
        navSurface.BuildNavMesh();

        // Passes one frame to allow Awake/Start method to run first
        yield return null;
    }

    [UnityTearDown] // Destroys all test objects to be recreated in the setup
    public IEnumerator TearDown()
    {
        Object.DestroyImmediate(protoObject);
        Object.DestroyImmediate(targetObject);
        Object.DestroyImmediate(floorObject);
        yield return null;
    }

    [UnityTest] // Testing if the monster stops when the player is undetected
    public IEnumerator MonsterAIStopsWhenPlayerIsUndetected()
    {
        navAgent.isStopped = false;
        protoObject.transform.position = Vector3.zero;
        targetObject.transform.position = new Vector3(30f, 0f, 0f);

        InvokePrivateMethod(protoAI, "ProtoBehavior");
        yield return null;

        Assert.IsTrue(navAgent.isStopped);
    }
    
    [UnityTest] // Testing if the monster starts chasing when the player is detected
    public IEnumerator MonsterDetectsPlayerAndStartsChasing()
    {
        navAgent.isStopped = true;
        protoObject.transform.position = Vector3.zero;
        targetObject.transform.position = new Vector3(10f, 0f, 0f);

        InvokePrivateMethod(protoAI, "ProtoBehavior");

        yield return null;

        Assert.IsFalse(navAgent.isStopped);
    }

    [UnityTest] // Testing if the monster start its basic attack and begins cooldown
    public IEnumerator MonsterDoesBasicAttackAndBeginsCooldown()
    {
        navAgent.isStopped = true;
        protoObject.transform.position = Vector3.zero;
        targetObject.transform.position = new Vector3(3f, 0f, 0f);

        Physics.SyncTransforms();

        InvokePrivateMethod(protoAI, "ProtoFollow");

        yield return null;

        bool attack = GetPrivateField<bool>(protoAI, "attackCooldown");
        Assert.IsTrue(attack);
    }
    
    [UnityTest] // Testing if the monster starts its lunge attack and begins cooldown
    public IEnumerator MonsterLungesAndBeginsCooldown()
    {
        navAgent.isStopped = false;
        protoObject.transform.position = Vector3.zero;
        targetObject.transform.position = new Vector3(16f, 0f, 0f);

        Physics.SyncTransforms();

        InvokePrivateMethod(protoAI, "ProtoFollow");

        yield return null;

        bool lunge = GetPrivateField<bool>(protoAI, "lungeCooldown");
        Assert.IsTrue(lunge);
    }

    // Helpers
    // This function is used to invoke private methods from within the specified script
    private static void InvokePrivateMethod(object target, string methodName)
    {
        // Look for methods within the specified class that are an instance and private
        MethodInfo method = target.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);

        method.Invoke(target, null);
    }

    // This function is used to get private values from within the specified script
    private static Value GetPrivateField<Value>(object target, string fieldName)
    {
        // Works roughly the same as in the previous method except we are looking for variables instead
        FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);

        return (Value)field.GetValue(target);
    }
}