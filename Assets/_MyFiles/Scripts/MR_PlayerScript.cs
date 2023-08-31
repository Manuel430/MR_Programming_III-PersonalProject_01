using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class MR_PlayerScript : MonoBehaviour
{
    PlayerControlsScript playerControls;
    CharacterController playerController;
    public float speed = 10f;
    Vector3 playerVelocity;
    public bool isGrounded;
    public float gravity = -9.8f;
    public float jumpHeight = 3f;

    public Camera playerCam;
    float xRotation = 0f;
    public float xSensitivity = 30f;
    public float ySensitivity = 30f;

    private void Awake()
    {
        playerControls = new PlayerControlsScript();
        playerControls.Player.Enable();
        playerController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        isGrounded = playerController.isGrounded;
    }

    private void FixedUpdate()
    {
        ProcessMovement();
    }

    private void LateUpdate()
    {
        ProcessLook();
    }

    public void ProcessMovement()
    {
        Vector2 inputVector = playerControls.Player.Movement.ReadValue<Vector2>();
        Vector3 movementDirection = new Vector3(inputVector.x, 0, inputVector.y);

        playerVelocity.y += gravity * Time.deltaTime;
        if (isGrounded && playerVelocity.y < 0)
            playerVelocity.y = -2f;

        playerController.Move(transform.TransformDirection(movementDirection) * speed * Time.deltaTime);
    }

    public void ProcessLook()
    {
        Vector2 lookVector = playerControls.Player.Look.ReadValue<Vector2>();
        xRotation -= (lookVector.y * Time.deltaTime) * ySensitivity;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);
        playerCam.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        transform.Rotate(Vector3.up * (lookVector.x * Time.deltaTime) * xSensitivity);
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            Debug.Log("Jump: " + context);
            if(isGrounded)
            {
                playerVelocity.y = Mathf.Sqrt(jumpHeight * -3.0f * gravity);
            }
        }
    }

    public void MovementDebug(InputAction.CallbackContext context)
    {
        Debug.Log(context);
    }
}
