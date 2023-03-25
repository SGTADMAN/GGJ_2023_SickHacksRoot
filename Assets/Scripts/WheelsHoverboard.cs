using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WheelsHoverboard : MonoBehaviour
{
    [SerializeField] Vector3 input;
    Rigidbody boardRigidbody;
    public LayerMask layerMask;
    [SerializeField] float minimumDistFromGround = 0.5f;
    [SerializeField] bool grounded;
    public float moveForce, turnTorque, gravity = 15;
    RaycastHit normalCheckHit;

    public List<AxleInfo> axleInfos; // the information about each individual axle
    public float maxMotorTorque; // maximum torque the motor can apply to wheel
    public float maxSteeringAngle; // maximum steer angle the wheel can have
    private void Start()
    {
        boardRigidbody = GetComponent<Rigidbody>();
    }
    public void HandleMovement(InputAction.CallbackContext context)
    {
        Vector2 rawInput = context.ReadValue<Vector2>();
        input = new Vector3(rawInput.x, 0, rawInput.y);
    }
    public void HandleReset(InputAction.CallbackContext context)
    {
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.Euler(Vector3.zero);
    }

    private void Update()
    {
        if (Physics.Raycast(transform.position, -transform.up, out normalCheckHit, minimumDistFromGround, layerMask))
        {
            grounded = true;
        }
        else
        {
            grounded = false;
        }
        //boardRigidbody.AddForce(input.z * (moveForce * Time.deltaTime) * transform.forward);
        //boardRigidbody.AddTorque(input.x * (turnTorque * Time.deltaTime) * transform.up);

    }
    private void FixedUpdate()
    {
        float motor = maxMotorTorque * input.z;
        float steering = maxSteeringAngle * input.x;

        foreach (AxleInfo axleInfo in axleInfos)
        {
            if (axleInfo.steering)
            {
                axleInfo.leftWheel.steerAngle = steering;
                axleInfo.rightWheel.steerAngle = steering;
            }
            if (axleInfo.motor)
            {
                axleInfo.leftWheel.motorTorque = motor;
                axleInfo.rightWheel.motorTorque = motor;
            }
        }
    }
}
[System.Serializable]
public class AxleInfo
{
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public bool motor; // is this wheel attached to motor?
    public bool steering; // does this wheel apply steer angle?
}
