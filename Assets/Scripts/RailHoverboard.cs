using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RailHoverboard : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] Vector3 input;
    [SerializeField] Vector3 rotInput;

    [Header("Variables")]
    public LayerMask layerMask;
    public float multiplier;
    public float moveForce, turnTorque, leanAmount;
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

    [Header("Waypoint Stuff")]
    [SerializeField] Transform[] wayPointList;
    int waypointNo;
    public Transform targetWaypoint;
    public Transform lastWaypointPos;
    public Transform startingPoint;
    [SerializeField] GameObject wayPointContainer;
    [SerializeField] Vector3[] railVerts;
    CSVConvert converter;
    [SerializeField] GameObject rail;

    private void Awake()
    {
        converter = FindObjectOfType<CSVConvert>();
        railVerts = converter.GetSplineVerts();
        GameObject[] wayPointObj = new GameObject[railVerts.Length];
        wayPointList = new Transform[railVerts.Length];
        Matrix4x4 localToWorld = transform.localToWorldMatrix;
        for (int i = 0; i < railVerts.Length; ++i)
        {
            railVerts[i] = Quaternion.AngleAxis(-90, Vector3.up) * railVerts[i];
            railVerts[i] *= 20f;
            railVerts[i] *= 6;
            railVerts[i] *= 0.1f;
            wayPointObj[i] = new GameObject("wayPoint_" + i);
            Vector3 world_v = localToWorld.MultiplyVector(railVerts[i]);
            wayPointObj[i].transform.position = world_v;
            wayPointObj[i].transform.parent = wayPointContainer.transform;
            wayPointList[i] = wayPointObj[i].transform;
        }
    }
    private void Start()
    {
        targetWaypoint = wayPointList[0];
    }
    public void Reset()
    {

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
        //transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        if (!stop)
        {
            //Vector3 forward = transform.forward;
            //transform.forward = Vector3.RotateTowards(transform.forward, targetWaypoint.position - transform.position, turnTorque * Time.deltaTime, 0f);
            Quaternion rot = Quaternion.LookRotation(targetWaypoint.position - lastWaypointPos.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, turnTorque * Time.deltaTime);
            //transform.LookAt(targetWaypoint, Vector3.up);
            transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.position, forwardSpeed * Time.deltaTime);
            //transform.position = Vector3.Lerp(transform.position, targetWaypoint.position, forwardSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetWaypoint.position) < 0.1f)
            {
                waypointNo++;
                if (waypointNo >= wayPointList.Length)
                    waypointNo = wayPointList.Length - 1;

                targetWaypoint = wayPointList[waypointNo];

                if (waypointNo == 1)
                    lastWaypointPos = startingPoint;
                else
                    lastWaypointPos = wayPointList[waypointNo-1];
            }
        }
    }
}
