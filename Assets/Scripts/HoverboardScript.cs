using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HoverboardScript : MonoBehaviour
{
    [SerializeField] Vector3 input;
    Rigidbody boardRigidbody;
    public LayerMask layerMask;
    [SerializeField] float minimumDistFromGround = 0.5f;
    [SerializeField] bool grounded;
    public float multiplier;
    public float moveForce, turnTorque, rotationRate, gravity = 15;
    public Transform[] anchors = new Transform[4];
    [SerializeField] Transform rwdPusher;
    RaycastHit[] hits = new RaycastHit[4];
    RaycastHit normalCheckHit;
    [SerializeField] float uprightCheck;
    private float rotVelocity;
    [SerializeField] float turnRotAngle, turnRotSeekSpeed;
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
            boardRigidbody.AddForce(input.z * (moveForce * Time.deltaTime * boardRigidbody.mass) * transform.forward);
        }
        else
        {
            grounded = false;
        }
        Vector3 turnAmount = transform.up * rotationRate * input.x;
        turnAmount *= Time.deltaTime * boardRigidbody.mass;
        boardRigidbody.AddTorque(turnAmount);
        
    }
    private void FixedUpdate()
    {
        if (grounded)
        {
            for (int i = 0; i < 4; i++)
            {
                ApplyForce(anchors[i], hits[i]);
            }
            boardRigidbody.drag = 1.8f;
        }
        else
        {
            ApplyDownwardForce();
            boardRigidbody.drag = 0;
        }

        Quaternion q = Quaternion.FromToRotation(transform.up, Vector3.up) * transform.rotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * 0.5f);

        if (!isUpright())
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0f), 10 * Time.deltaTime);
        }

        Vector3 newRot = transform.eulerAngles;
        newRot.z = Mathf.SmoothDampAngle(newRot.z, input.x * -turnRotAngle, ref rotVelocity, turnRotSeekSpeed);
        transform.eulerAngles = newRot;
    }
    void ApplyForce(Transform anchor, RaycastHit hit)
    {
        if (Physics.Raycast(anchor.position, -anchor.up, out hit, minimumDistFromGround, layerMask))
        {
            float force = 0;
            force = Mathf.Abs(1 / (hit.point.y - anchor.position.y));
            boardRigidbody.AddForceAtPosition(transform.up * force * multiplier, anchor.position, ForceMode.Acceleration);
        }
    }
    private void ApplyDownwardForce()
    {
        boardRigidbody.AddForce(-Vector3.up * (gravity * Time.deltaTime * boardRigidbody.mass), ForceMode.Acceleration);
    }
    bool isUpright()
    {
        uprightCheck = Vector3.Dot(transform.up, Vector3.down);
        if (uprightCheck < -0.1f)
        {
            return true;
        }
        return false;
    }
}
