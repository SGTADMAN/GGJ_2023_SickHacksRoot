using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DownhillHoverboardScript : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] Vector3 input;
    [SerializeField] Vector3 rotInput;

    [Header("Variables")]
    public LayerMask layerMask;
    public float multiplier;
    public float moveForce, turnTorque, leanAmount;
    public float playerTurnTorque;
    Rigidbody boardRigidbody;

    [Header("Raycast")]
    public Transform[] anchors = new Transform[4];
    RaycastHit[] hits = new RaycastHit[4];
    RaycastHit normalCheckHit;
    [SerializeField] float minimumDistFromGround = 0.5f;
    [SerializeField] Vector3 directionOfTravel;
    [SerializeField] float forwardSpeed = 10;
    [SerializeField] float gravity = 9.81f;

    [Header("Misc")]
    [SerializeField] CinemachineVirtualCamera virtualCamera;
    public bool stop;
    [SerializeField] bool grounded;
    [SerializeField] GameObject hoverboardModel;
    [SerializeField] Animator playerAnim;

    [Header ("Waypoint Stuff")]
    [SerializeField] Transform[] wayPointList;
    int waypointNo;
    public Transform targetWaypoint;
    public Transform lastWaypointPos;
    public Transform startingPoint;
    private void Start()
    {
        boardRigidbody = GetComponent<Rigidbody>();        
        stop = false;
        waypointNo = 0;
        targetWaypoint = wayPointList[waypointNo];
        playerAnim = GetComponentInChildren<Animator>();
        directionOfTravel = (targetWaypoint.position - lastWaypointPos.position).normalized;
    }
    public void Reset()
    {
        waypointNo = 0;
        targetWaypoint = wayPointList[waypointNo];
        lastWaypointPos = startingPoint;
        stop = false;
        boardRigidbody.velocity = Vector3.zero;
        boardRigidbody.angularVelocity = Vector3.zero;
        virtualCamera.gameObject.transform.localRotation = Quaternion.Euler(0, -90, 0);
        directionOfTravel = (targetWaypoint.position - lastWaypointPos.position).normalized;
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
        boardRigidbody.AddForce(input.x * (moveForce*Time.deltaTime) * virtualCamera.transform.right);
        if (!grounded)
        {
            hoverboardModel.transform.Rotate((playerTurnTorque * Time.deltaTime) * -rotInput.z, (playerTurnTorque * Time.deltaTime) * rotInput.x, 0);
            
            if(rotInput.magnitude < 0.1)
                hoverboardModel.transform.localRotation = Quaternion.Slerp(hoverboardModel.transform.localRotation, new Quaternion(0, 180, 0, 0), Time.deltaTime * 0.1f);
        }
        else
        {
            hoverboardModel.transform.localRotation = new Quaternion(0, 180, 0, 0);
            
        }
    }
    private void FixedUpdate()
    {
        for (int i = 0; i < 4; i++)
        {
            ApplyForce(anchors[i], hits[i]);
        }


        if (!Physics.Raycast(transform.position, -transform.up, out normalCheckHit, 1f, layerMask))
        {
            if (!stop)
            {
                ApplyDownwardForce();
            }
            grounded = false;
        }
        else
        {
            grounded = true;
        }
        playerAnim.SetBool("grounded", grounded);
        
            Quaternion q = Quaternion.FromToRotation(transform.up, Vector3.up) * transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * 0.5f);


        if (Vector3.Distance(transform.position, targetWaypoint.position) < Vector3.Distance(lastWaypointPos.position, targetWaypoint.position) / 2)
        {
            try
            {
                waypointNo++;
                lastWaypointPos = targetWaypoint;
                targetWaypoint = wayPointList[waypointNo];
                directionOfTravel = (targetWaypoint.position - lastWaypointPos.position).normalized;
            }
            catch
            {
                if (wayPointList.Length == 1)
                {
                    lastWaypointPos = startingPoint;
                    targetWaypoint = wayPointList[wayPointList.Length - 1];                    
                }
                else
                {
                    lastWaypointPos = wayPointList[wayPointList.Length - 2];
                    targetWaypoint = wayPointList[wayPointList.Length - 1];                    
                }
                directionOfTravel = (targetWaypoint.position - lastWaypointPos.position).normalized;
            }            
        }

        Quaternion aimingRot = Quaternion.LookRotation(targetWaypoint.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, 
            Quaternion.Euler(aimingRot.eulerAngles.x, 
            aimingRot.eulerAngles.y, 
            transform.rotation.eulerAngles.z), 
            turnTorque * Time.deltaTime);

        //transform.LookAt(targetWaypoint, Vector3.up);

        if (stop)
        {
            boardRigidbody.velocity = Vector3.zero;
            boardRigidbody.Sleep();
        }
        else
        {
            ApplyForwardForce();
        }

        if (!isUpright())
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(aimingRot.eulerAngles.x, aimingRot.eulerAngles.y, 0f), turnTorque * Time.deltaTime);
        }

    }

    private void ApplyForwardForce()
    {
        boardRigidbody.AddForce(transform.forward * (forwardSpeed*Time.deltaTime), ForceMode.Acceleration);
    }
    private void ApplyDownwardForce()
    {
        boardRigidbody.AddForce(-Vector3.up * (gravity * Time.deltaTime), ForceMode.Acceleration);
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
