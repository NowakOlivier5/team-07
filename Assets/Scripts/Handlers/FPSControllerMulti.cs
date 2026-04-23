using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(CharacterController))]
public class FPSControllerMulti : NetworkBehaviour
{
    //Movement variables for player
    public Camera playerCamera;
    public float walkSpeed = 6f;
    public float runSpeed = 12f;
    public float jumpPower = 4f;
    public float gravity = 10f;
    public float lookSpeed = 2f;
    public float lookXLimit = 45f;
    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;
    public bool canMove = true;
    CharacterController characterController;

    // Other variables
    public int playerHealth = 100;

    void Start()
    {
        characterController = GetComponent<CharacterController>(); //get player component

        if (!isLocalPlayer)
        {
            if (playerCamera != null)
                playerCamera.enabled = false;

            AudioListener listener = GetComponentInChildren<AudioListener>();
            if (listener != null)
                listener.enabled = false;

            return;
        }

        SetupLocalPlayer();
        //Cursor.lockState = CursorLockMode.Locked; //lock cursor to center
        //Cursor.visible = false; //hide cursor
    }
    private void SetupLocalPlayer()
    {
        if (playerCamera != null)
            playerCamera.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (isLocalPlayer)
        {
            #region Handles Movment
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 right = transform.TransformDirection(Vector3.right);

            // shift to run
            bool isRunning = Input.GetKey(KeyCode.LeftShift);
            float curSpeedX = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Vertical") : 0;
            float curSpeedY = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Horizontal") : 0;
            float movementDirectionY = moveDirection.y;
            moveDirection = (forward * curSpeedX) + (right * curSpeedY);

            #endregion

            #region Handles Jumping
            if (Input.GetButton("Jump") && canMove && characterController.isGrounded) //can only jump while on ground and movement allowed
            {
                moveDirection.y = jumpPower;
            }
            else
            {
                moveDirection.y = movementDirectionY; //apply Y velocity
            }

            if (!characterController.isGrounded)//if airborne
            {
                moveDirection.y -= gravity * Time.deltaTime; //apply gravity
            }

            #endregion

            #region Handles Rotation
            characterController.Move(moveDirection * Time.deltaTime);

            if (canMove && !PauseMenu.isPaused)
            {
                rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
                rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
                playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
                transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
            }
            #endregion
        }
    }

    public void takeDamage(int damage)
    {
        playerHealth -= damage;

        //health never goes below 0
        if (playerHealth < 0)
        {
            playerHealth = 0;
        }
    }
}
