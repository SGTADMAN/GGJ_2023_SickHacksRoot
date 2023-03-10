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
    public float multiplier;
    public float moveForce, turnTorque;
    public Transform[] anchors = new Transform[4];
    RaycastHit[] hits = new RaycastHit[4];
    RaycastHit normalCheckHit;

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
        boardRigidbody.AddForce(input.z * (moveForce) * transform.forward);
        boardRigidbody.AddTorque(input.x * (turnTorque) * transform.up);
    }
    private void FixedUpdate()
    {
        if (grounded)
        {
            for (int i = 0; i < 4; i++)
            {
                ApplyForce(anchors[i], hits[i]);
            }
        }
        Quaternion q = Quaternion.FromToRotation(transform.up, Vector3.up) * transform.rotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * 0.5f);
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

    bool isUpright()
    {
        if (Vector3.Dot(transform.up, Vector3.down) < -0.5f)
        {
            return true;
        }
        return false;
    }
}
