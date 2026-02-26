using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class ProtoAI : MonoBehaviour
{
    // Agent Variables
    public Transform target;
    public float closeDistance;
    public float protoVisionRange;

    private NavMeshAgent protoAgent;
    private float protoDistance;

    public LayerMask playerLayer;
    private bool playerVisible;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        // Loads the NavMeshAgent to work with the agent
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

    private void DetectPlayer()
    {
        playerVisible = Physics.CheckSphere(transform.position, protoVisionRange, playerLayer);
    }

    private void ProtoStop()
    {
        protoAgent.isStopped = true;
    }

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

    private void ProtoBehavior()
    {
        if (!playerVisible)
        {
            ProtoStop();
        }
        else if (playerVisible)
        {
            ProtoFollow();
        }
    }

    // Draws a sphere around the proto agent to visualize its view/patrol range
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, protoVisionRange);
    }
}