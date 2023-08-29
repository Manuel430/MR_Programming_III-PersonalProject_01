using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MR_PlayerScript : MonoBehaviour
{
    PlayerControlsScript playerControls;
    CharacterController playerController;
    public float speed = 5f;

    private void Start()
    {
        playerControls = new PlayerControlsScript();
        playerControls.Player.Enable();
        playerController = GetComponent<CharacterController>();
    }

    private void FixedUpdate()
    {
        ProcessMovement();
    }

    public void ProcessMovement()
    {
        Vector2 inputVector = playerControls.Player.Movement.ReadValue<Vector2>();
        Vector3 movementDirection = new Vector3(inputVector.x, 0, inputVector.y);
        playerController.Move(transform.TransformDirection(movementDirection) * speed * Time.deltaTime);
    }

    public void MovementDebug(InputAction.CallbackContext context)
    {
        Debug.Log(context);
    }
}
