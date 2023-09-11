using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
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
    public GameObject hunterBody_01;
    public GameObject hunterBody_02;
    public GameObject gun_01;
    public GameObject gun_02;

    private void Awake()
    {
        playerControls = new PlayerControlsScript();
        playerControls.Player.Enable();
        playerController = GetComponent<CharacterController>();
        playerControls.Player.Jump.performed += Jump;
        playerControls.Player.Look.performed += UpdateLookInput;
    }

    private void Jump(InputAction.CallbackContext context)
    {
        Jump_ServerRpc();
    }

    private void UpdateLookInput(InputAction.CallbackContext obj)
    {
        if (IsOwner)
        { 
            ProcessLook(obj.ReadValue<Vector2>());
        }
    }

    private void Start()
    {
        if (IsOwner)
        {
            hunterBody_01.gameObject.SetActive(true);
            hunterBody_02.gameObject.SetActive(false);
            gun_01.gameObject.SetActive(true);
            gun_02.gameObject.SetActive(false);
        }
        else if (IsClient)
        {
            hunterBody_01.gameObject.SetActive(false);
            hunterBody_02.gameObject.SetActive(true);
            gun_01.gameObject.SetActive(false);
            gun_02.gameObject.SetActive(true);
        }
    }

    private void Update()
    {
        playerCam.enabled = IsOwner;

        if (IsOwner)
        {
            Vector2 inputVector = playerControls.Player.Movement.ReadValue<Vector2>();
            Vector3 movementDirection = new Vector3(inputVector.x, 0, inputVector.y);
            ProcessMovement_ServerRpc(movementDirection);
        }
        else if (IsClient)
        {
            Vector2 inputVector = playerControls.Player.Movement.ReadValue<Vector2>();
            Vector3 movementDirection = new Vector3(inputVector.x, 0, inputVector.y);
            ProcessMovement_ServerRpc(movementDirection);
        }

        if(IsOwner && Input.GetKeyDown(KeyCode.Backspace))
        {
            NetworkManager.Singleton.DisconnectClient(1);   
        }
    }

    private void LateUpdate()
    {
        if(IsOwner)
        {
            Vector2 lookVector = playerControls.Player.Look.ReadValue<Vector2>();
        }
    }



    [ServerRpc]
    public void ProcessMovement_ServerRpc(Vector3 movementDirection)
    {
        playerVelocity.y += gravity * Time.deltaTime;
        if (playerController.isGrounded && playerVelocity.y < 0)
            playerVelocity.y = -2f;
        Vector3 moveInputVel = transform.TransformDirection(movementDirection) * speed;
        playerVelocity.x = moveInputVel.x;
        playerVelocity.z = moveInputVel.z;
        playerController.Move(playerVelocity * Time.deltaTime);
    }

    public void ProcessLook(Vector2 lookVector)
    {
        //xRotation -= (lookVector.y * Time.deltaTime) * ySensitivity;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);
        if(playerCam == null)
        {
            return;
        }
        playerCam.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        transform.Rotate(Vector3.up * (lookVector.x * Time.deltaTime) * xSensitivity);
        ServerUpdateRotation_ServerRpc(transform.rotation);
        ServerRotationUpdated_ClientRpc(transform.rotation);
    }

    [ClientRpc]
    private void ServerRotationUpdated_ClientRpc(Quaternion rotation)
    {
        transform.rotation = rotation;
    }

    [ServerRpc]
    private void ServerUpdateRotation_ServerRpc(Quaternion rotation)
    {
        transform.rotation = rotation;
    }

    [ServerRpc]
    public void Jump_ServerRpc()
    {
        if(playerController.isGrounded)
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -3.0f * gravity);
        }
    }
}
