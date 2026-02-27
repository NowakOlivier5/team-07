using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class ProtoAI : MonoBehaviour
{
    // Agent Variables
    public Transform target;
    private float closeDistance = 2;
    private float protoVisionRange = 12;

    private NavMeshAgent protoAgent; // Loads the agents navmeshagent
    private float protoDistance; // Value used to prevent agent walking over ontop of the player

    public LayerMask playerLayer; // Player layer, used by the agent to detect the player within its view range
    private bool playerVisible; // Boolean for whether the agent sees or doesnt see the player

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        // Loads the NavMeshAgent to work with the agent and disables the gravity so the navagent can move
        // properly and up stairs/obstacles
        protoAgent = GetComponent<NavMeshAgent>();
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
    }

    // Update is called once per frame
    private void Update()
    {
        DetectPlayer();
        ProtoBehavior();
    }

    // Makes a sphere around the agent and checks if the player layer is in the spheres radius
    // If true sets playerVisble boolean to true
    private void DetectPlayer()
    {
        playerVisible = Physics.CheckSphere(transform.position, protoVisionRange, playerLayer);
    }

    // Stops the agent so it doesnt move forever when not following the player
    private void ProtoStop()
    {
        protoAgent.isStopped = true;
    }

    // Function for the agent to follow the player
    private void ProtoFollow()
    {
        // Checks the distance between the agent and the player
        protoDistance = Vector3.Distance(protoAgent.transform.position, target.position);
        // If the agent is close enough, agent stops moving
        if (protoDistance < closeDistance)
        {
            protoAgent.isStopped = true;
        }
        // Otherwise, agent will continue to move towards the players current position
        else
        {
            protoAgent.isStopped = false;
            protoAgent.destination = target.position;
        }
    }

    // Turns the agents kinematic off and turns on gravity for the agent when hit by a projectile
    // Called in from the bullet script
    public void Die()
    {
        // Disables the navmesh agent
        GetComponent<NavMeshAgent>().enabled = false;

        // Gets the rigidbody component of the proto agent and disables kinematic and makes it use gravity
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.useGravity = true;

        // Stops the script
        this.enabled = false;
    }

    // Defines what the agent will do based on if the playerVisible boolean is true or false
    private void ProtoBehavior()
    {
        // If the player is not visible, the agent is told to stop
        if (!playerVisible)
        {
            ProtoStop();
        }
        // Otherwise, follow the players current position
        else if (playerVisible)
        {
            ProtoFollow();
        }
    }

    // Draws a sphere around the proto agent to visualize its view range whilst in the editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, protoVisionRange);
    }
}