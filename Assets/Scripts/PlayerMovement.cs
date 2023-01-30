using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] Vector3 input;
    [SerializeField] Vector2 lookInput;
    [SerializeField] float moveSpeed, jumpHeight, rotationSpeed;
    [SerializeField] float laterialSlant = 2f;
    [SerializeField] float slantTime = 100f;
    float initMoveSpeed;
    CharacterController characterController;
    Camera cam;
    [SerializeField] CinemachineVirtualCamera virtualCamera;
    public bool isGrounded, jumped, running, crouched;
    public Vector3 movementVel, jumpVel;
    public float gravity = -9.81f;
    public float terminalVel = 53.0f;
    PlayerInput playerInput;
    public GameObject CinemachineCameraTarget;
    private float _cinemachineTargetPitch;
    private float _rotationVelocity;

    private void Start()
    {
        cam = FindObjectOfType<Camera>();
        characterController = GetComponent<CharacterController>();
        initMoveSpeed = moveSpeed;
        playerInput = GetComponent<PlayerInput>();
    }
    public void HandleMovement(InputAction.CallbackContext context)
    {
        Vector2 rawInput = context.ReadValue<Vector2>();
        input = new Vector3(rawInput.x, 0, rawInput.y);
    }
    public void HandleLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }
    public void HandleJump(InputAction.CallbackContext context)
    {
        input.y = context.ReadValue<float>();
    }
    public void HandleSprint(InputAction.CallbackContext context)
    {
        running = Convert.ToBoolean(context.ReadValue<float>());
    }
    public void HandleCrouch(InputAction.CallbackContext context)
    {
        crouched = Convert.ToBoolean(context.ReadValue<float>());
    }
    private void Update()
    {
        isGrounded = characterController.isGrounded;
        if (running)
            moveSpeed = initMoveSpeed * 1.5f;
        else
            moveSpeed = initMoveSpeed;
        //HandleRotation();
        //CameraRotation();
        //Movement();
        //JumpAndGravity();
    }

    private void Movement()
    {
        Vector3 cameraForward = cam.transform.forward;
        cameraForward.y = 0;
        Vector3 cameraRight = cam.transform.right;
        cameraRight.y = 0;
        Vector3 forwardMove, rightMove;
        //forwardMove = cameraRight * (input.x * Time.deltaTime * moveSpeed);
        //rightMove = cameraForward * (input.z * Time.deltaTime * moveSpeed);
        forwardMove = transform.right * input.x;
        rightMove = transform.forward * input.z;
        movementVel = forwardMove + rightMove;
        movementVel.y = 0;
        characterController.Move(movementVel.normalized * (moveSpeed * Time.deltaTime));
        virtualCamera.m_Lens.Dutch = Mathf.Lerp(virtualCamera.m_Lens.Dutch, input.x * (-laterialSlant), slantTime * Time.deltaTime);
    }

    void JumpAndGravity()
    {
        if (isGrounded)
        {
            jumpVel.y = 0;
        }
        if (input.y > 0 && isGrounded)
        {
            jumpVel.y += Mathf.Sqrt(jumpHeight * -3.0f * gravity);
        }
        jumpVel.y += gravity * Time.deltaTime;
        characterController.Move(jumpVel * Time.deltaTime);
    }

    void HandleRotation()
    {
        Vector3 targetDir = Vector3.zero;
        targetDir = cam.transform.forward;
        //targetDir += cam.transform.right;
        targetDir.Normalize();
        targetDir.y = 0;

        if (targetDir == Vector3.zero)
        {
            targetDir = transform.forward;
        }
        //transform.forward = targetDir;
        Quaternion targetRot = Quaternion.LookRotation(targetDir);
        Quaternion playerRot = Quaternion.Slerp(transform.rotation, targetRot, 1000 * Time.deltaTime);

        transform.rotation = playerRot;
        transform.forward = targetDir;
    }

    private bool IsCurrentDeviceMouse
    {
        get
        {
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
            return playerInput.currentControlScheme == "KeyboardMouse";
#else
            return false;
#endif
        }
    }
    private void CameraRotation()
    {
        // if there is an input
        if (lookInput.sqrMagnitude >= 0.01f)
        {
            //Don't multiply mouse input by Time.deltaTime
            float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

            _cinemachineTargetPitch += lookInput.y * rotationSpeed * deltaTimeMultiplier;
            _rotationVelocity = lookInput.x * rotationSpeed * deltaTimeMultiplier;

            // clamp our pitch rotation
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, -90, 90);

            // Update Cinemachine camera target pitch
            CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);

            // rotate the player left and right
            transform.Rotate(Vector3.up * _rotationVelocity);
        }
    }
    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    public float pushPower = 2.0F;

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;

        // no rigidbody
        if (body == null || body.isKinematic)
            return;

        // We dont want to push objects below us
        if (hit.moveDirection.y < -0.3f)
            return;

        // Calculate push direction from move direction,
        // we only push objects to the sides never up and down
        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);

        // If you know how fast your character is trying to move,
        // then you can also multiply the push velocity by that.

        // Apply the push
        body.velocity = pushDir * pushPower;
    }
    public void UpdateSlant(float inVal)
    {
        laterialSlant = inVal;
    }
    public void UpdateRotation(float inVal)
    {
        rotationSpeed = inVal;
    }
}
