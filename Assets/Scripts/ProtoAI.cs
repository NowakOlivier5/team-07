using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class ProtoAI : MonoBehaviour
{
    // Agent Variables
    public Transform target;
    private float closeDistance = 4;
    public float protoVisionRange;
    public int monsterHealth;
    private bool attackCooldown;
    private bool lungeCooldown;

    private NavMeshAgent protoAgent; // Loads the agents navmeshagent
    private Animator animator; // Loads the animator component
    private float protoDistance; // Value used to prevent agent walking over ontop of the player

    public LayerMask playerLayer; // Player layer, used by the agent to detect the player within its view range
    public FPSController player;
    private bool playerVisible; // Boolean for whether the agent sees or doesnt see the player

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        // Loads the NavMeshAgent to work with the agent and disables the gravity so the navagent can move
        // properly and up stairs/obstacles
        protoAgent = GetComponent<NavMeshAgent>();
        Rigidbody rb = GetComponentInChildren<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        rb.isKinematic = true;
        rb.useGravity = false;

        attackCooldown = false;
        lungeCooldown = false;
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
            if (attackCooldown == false)
            {
                StartCoroutine(ProtoAttack());
            }
        }
        // Otherwise, agent will continue to move towards the players current position
        else
        {
            protoAgent.isStopped = false;
            protoAgent.destination = target.position;
            animator.SetBool("isAttacking", false);
        }

        // If the player is within the defined range and the cooldown is off, tells the monster to lunge
        if (protoDistance <= 22 && protoDistance >= 14)
        {
            if (lungeCooldown == false)
            {
                StartCoroutine(ProtoLunge());
            }
        }
    }

    // Determines when the monster can attack and deal damage
    IEnumerator ProtoAttack()
    {
        attackCooldown = true; // Acts as a cooldown for the basic attack, otherwise it would deal damage every frame which is not what we want
        animator.SetBool("isAttacking", true); // Queues the attack animation

        yield return new WaitForSeconds(0.2f); // Slight delay of the attack to sync up with the animation of the claws hitting the player

        // Checks if the player is still in close proxmity where it would then deal damage
        if (protoDistance < closeDistance)
        {
            player.takeDamage(1);
        }

        yield return new WaitForSeconds(0.2f);

        attackCooldown = false; // Reset the attack cooldown allowing the function to run again
    }

    // Handles when the animatons for lunging play and whether if the player was hit. Also in charge of giving the monster a cooldown on its lunge
    IEnumerator ProtoLunge()
    {
        lungeCooldown = true;
        animator.SetBool("isChasing", false);
        //animator.SetBool("isLunging", true);
        animator.Play("Armature_JumpAttack");

        yield return new WaitForSeconds(0.25f);

        protoAgent.speed = 50;
        protoAgent.acceleration = 400;
        protoAgent.angularSpeed = 10;

        yield return new WaitForSeconds(0.2f);

        if (protoDistance < closeDistance + 3)
        {
            player.takeDamage(10);
        }

        yield return new WaitForSeconds(0.1f);

        protoAgent.speed = 12;

        yield return new WaitForSeconds(0.2f);

        protoAgent.acceleration = 20;
        protoAgent.angularSpeed = 300;

        animator.SetBool("isChasing", true);
        //animator.SetBool("isLunging", false);

        yield return new WaitForSeconds(10f);

        lungeCooldown = false;
    }

    // Turns the agents kinematic off and turns on gravity for the agent when hit by a projectile
    // Called in from the bullet script
    public void Die(int damage)
    {
        // Reduces the monster health when hit
        monsterHealth -= damage;
        if (monsterHealth <= 0)
        {
            // Disables the navmesh agent
            GetComponent<NavMeshAgent>().enabled = false;

            // Gets the rigidbody component of the proto agent and disables kinematic and makes it use gravity
            Rigidbody rb = GetComponentInChildren<Rigidbody>();
            rb.isKinematic = false;
            rb.useGravity = true;

            // Stops the script
            animator.enabled = false;
            this.enabled = false;
        }
    }

    // Defines what the agent will do based on if the playerVisible boolean is true or false
    private void ProtoBehavior()
    {
        // If the player is not visible, the agent is told to stop
        if (!playerVisible)
        {
            ProtoStop();
            animator.SetBool("isChasing", false);
        }
        // Otherwise, follow the players current position
        else if (playerVisible)
        {
            ProtoFollow();
            animator.SetBool("isChasing", true);
        }
    }

    // Draws a sphere around the proto agent to visualize its view range whilst in the editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, protoVisionRange);
    }
}