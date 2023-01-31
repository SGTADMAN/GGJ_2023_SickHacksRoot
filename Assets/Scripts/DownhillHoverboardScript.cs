using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DownhillHoverboardScript : MonoBehaviour
{
    [SerializeField] Vector3 input, rotInput;
    Rigidbody boardRigidbody;
    public LayerMask layerMask;
    public float multiplier;
    public float moveForce, turnTorque, leanAmount;
    public Transform[] anchors = new Transform[4];
    RaycastHit[] hits = new RaycastHit[4];
    RaycastHit normalCheckHit;
    [SerializeField] float minimumDistFromGround = 0.5f;
    [SerializeField] Vector3 directionOfTravel;
    [SerializeField] float forwardSpeed = 10;
    [SerializeField] float gravity = 9.81f;
    [SerializeField] Transform[] wayPointList;
    int waypointNo;
    [SerializeField] Transform targetWaypoint;
    [SerializeField] Transform lastWaypointPos;
    [SerializeField] Transform startingPoint;
    [SerializeField] float rotationSpeed;
    [SerializeField] CinemachineVirtualCamera virtualCamera;
    public bool stop;
    [SerializeField] bool grounded;
    private void Start()
    {
        boardRigidbody = GetComponent<Rigidbody>();
        directionOfTravel = transform.forward;
        waypointNo = 0;
        targetWaypoint = wayPointList[waypointNo];
        virtualCamera.LookAt = targetWaypoint;
        stop = false;
    }
    public void Reset()
    {
        waypointNo = 0;
        targetWaypoint = wayPointList[waypointNo];
        virtualCamera.LookAt = targetWaypoint;
        lastWaypointPos = startingPoint;
        stop = false;
        boardRigidbody.velocity = Vector3.zero;
        boardRigidbody.angularVelocity = Vector3.zero;
        
    }
    public void HandleMovement(InputAction.CallbackContext context)
    {
        Vector2 rawInput = context.ReadValue<Vector2>();
        input = new Vector3(rawInput.x, 0, rawInput.y);
    }
    public void HandleRotation(InputAction.CallbackContext context)
    {
        Vector2 rawInput = context.ReadValue<Vector2>();
        rotInput = new Vector3(rawInput.x, 0, rawInput.y);
    }

    private void Update()
    {
        boardRigidbody.AddForce(input.x * (moveForce) * transform.right);
        boardRigidbody.AddTorque(-input.x * (leanAmount) * transform.forward);
        if (!grounded)
        {
            boardRigidbody.AddTorque(rotInput.x * (turnTorque) * transform.up);
            boardRigidbody.AddTorque(rotInput.z * (turnTorque) * transform.right);
        }
    }
    private void FixedUpdate()
    {
        for (int i = 0; i < 4; i++)
        {
            ApplyForce(anchors[i], hits[i]);
        }

        if (stop)
        {
            boardRigidbody.velocity = Vector3.zero;
        } 
        else
        {
            ApplyForwardForce();
            
        }
        if (!Physics.Raycast(transform.position, -transform.up, out normalCheckHit, 1f, layerMask))
        {
            ApplyDownwardForce();
            grounded = false;
        }
        else
        {
            grounded = true;
        }

        if (!isUpright() && grounded)
        {
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        }

        if (Vector3.Distance(transform.position, targetWaypoint.position) < Vector3.Distance(lastWaypointPos.position,targetWaypoint.position)/2)
        {
            try
            {
                waypointNo++;
                lastWaypointPos = targetWaypoint;
                targetWaypoint = wayPointList[waypointNo];
                virtualCamera.LookAt = targetWaypoint;
            }
            catch
            {
                lastWaypointPos = wayPointList[wayPointList.Length - 2];
                targetWaypoint = wayPointList[wayPointList.Length-1];
                virtualCamera.LookAt = targetWaypoint;
            }
        }
        transform.forward = Vector3.RotateTowards(transform.forward,
                targetWaypoint.position - transform.position,
                rotationSpeed * Time.deltaTime, 0.0f);

    }

    private void ApplyForwardForce()
    {
        boardRigidbody.AddForce(transform.forward * (forwardSpeed),ForceMode.Acceleration);
    }
    private void ApplyDownwardForce()
    {
        boardRigidbody.AddForce(-transform.up * (-gravity), ForceMode.Acceleration);
    }

    void ApplyForce(Transform anchor, RaycastHit hit)
    {
        if (Physics.Raycast(anchor.position, -anchor.up, out hit, minimumDistFromGround, layerMask))
        {
            float force = 0;
            force = Mathf.Abs(1 / (hit.point.y - anchor.position.y));
            boardRigidbody.AddForceAtPosition(transform.up * force * multiplier, anchor.position, ForceMode.Acceleration);
        }

        if (grounded)
        {
            Quaternion q = Quaternion.FromToRotation(transform.up, Vector3.up) * transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * 0.5f);
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
