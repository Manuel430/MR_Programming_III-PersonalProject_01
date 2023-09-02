using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class MR_PlayerScript : NetworkBehaviour
{
    PlayerControlsScript playerControls;
    CharacterController playerController;
    public float speed = 10f;
    Vector3 playerVelocity;
    
    public float gravity = -9.8f;
    public float jumpHeight = 3f;

    public Camera playerCam;
    float xRotation = 0f;
    public float xSensitivity = 30f;
    public float ySensitivity = 30f;

    private NetworkVariable<Vector2> playerLookInput = new NetworkVariable<Vector2>();


    private void Awake()
    {
        playerControls = new PlayerControlsScript();
        playerControls.Player.Enable();
        playerController = GetComponent<CharacterController>();
        playerControls.Player.Jump.performed += Jump;
        playerControls.Player.Look.performed += UPdateLookInput;
        playerLookInput.OnValueChanged += ServerLookInputUpdated;
    }

    private void ServerLookInputUpdated(Vector2 previousValue, Vector2 newValue)
    {
        ProcessLook(newValue);
    }


    [ServerRpc]
    void UpdateLookInput_ServerRpc(Vector2 input)
    {
        playerLookInput.Value = input;
    }

    private void UPdateLookInput(InputAction.CallbackContext obj)
    {
        UpdateLookInput_ServerRpc(obj.ReadValue<Vector2>());
    }


    private void Update()
    {
        playerCam.enabled = IsOwner;
    }

    private void FixedUpdate()
    {
        if(IsOwner)
        {
            Vector2 inputVector = playerControls.Player.Movement.ReadValue<Vector2>();
            Vector3 movementDirection = new Vector3(inputVector.x, 0, inputVector.y);
            ProcessMovement_ServerRpc(movementDirection);
        }
    }

    private void LateUpdate()
    {
        return;
        if(IsOwner)
        {
            Vector2 lookVector = playerControls.Player.Look.ReadValue<Vector2>();
            //ProcessLook_ServerRpc(lookVector);
        }
    }

    public override void OnNetworkSpawn()
    {

    }
    [ServerRpc]
    public void ProcessMovement_ServerRpc(Vector3 movementDirection)
    {
        
        //Vector3 movementDirection = new Vector3(inputVector.x, 0, inputVector.y);
       
        playerVelocity.y += gravity * Time.deltaTime;
        if (playerController.isGrounded && playerVelocity.y < 0)
            playerVelocity.y = -2f;
        Debug.Log(playerVelocity);
        Vector3 moveInputVel = transform.TransformDirection(movementDirection) * speed;
        playerVelocity.x = moveInputVel.x;
        playerVelocity.z = moveInputVel.z;
        playerController.Move(playerVelocity * Time.deltaTime);
    }

    public void ProcessLook(Vector2 lookVector)
    {
        
        xRotation -= (lookVector.y * Time.deltaTime) * ySensitivity;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);
        playerCam.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        transform.Rotate(Vector3.up * (lookVector.x * Time.deltaTime) * xSensitivity);
    }

    public void Jump(InputAction.CallbackContext context)
    {
        Debug.Log("Jump: " + context);
        if(playerController.isGrounded)
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -3.0f * gravity);
        }
    }

    public void MovementDebug(InputAction.CallbackContext context)
    {
        Debug.Log(context);
    }
}
