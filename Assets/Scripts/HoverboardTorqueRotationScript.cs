using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HoverboardTorqueRotationScript : MonoBehaviour
{
    [SerializeField] Vector3 input;
    Rigidbody boardRigidbody;
    public LayerMask layerMask;
    [SerializeField] float minimumDistFromGround = 0.5f;
    [SerializeField] bool grounded;
    [SerializeField] GameObject[] waypoints;
    int waypointNo;
    [SerializeField] GameObject targetWaypoint;
    public float multiplier;
    public float moveForce, turnTorque, fixedMoveForce;
    [SerializeField] float gravity = 9.81f;
    public Transform[] anchors = new Transform[4];
    RaycastHit[] hits = new RaycastHit[4];
    RaycastHit normalCheckHit;
    [SerializeField] float waypointDistCheck = 100f;
    public bool fixedMovement;
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
    }
    private void FixedUpdate()
    {
        if (fixedMovement)
        {
            Vector3 lastPos = Vector3.zero;
            try
            {
                lastPos = waypoints[waypointNo - 1].transform.position;
            }
            catch
            {
                lastPos = transform.position;
            }
            Quaternion rot = Quaternion.LookRotation(targetWaypoint.transform.position - lastPos, -Vector3.Cross(lastPos, targetWaypoint.transform.position));
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, turnTorque * Time.deltaTime);
            transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.transform.position, fixedMoveForce * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetWaypoint.transform.position) <= waypointDistCheck)
            {
                waypointNo++;
                if (waypointNo >= waypoints.Length)
                {
                    waypointNo = 0;
                }
                targetWaypoint = waypoints[waypointNo];
            }
        }
        else
        {
            if (grounded)
            {
                for (int i = 0; i < 4; i++)
                {
                    ApplyForce(anchors[i], hits[i]);
                }
            }
            else
                ApplyDownwardForce();

            boardRigidbody.AddForce((moveForce * Time.deltaTime) * transform.forward);
            Quaternion q = Quaternion.FromToRotation(transform.up, Vector3.up) * transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * 0.5f);

            if (Vector3.Distance(transform.position, targetWaypoint.transform.position) <= waypointDistCheck)
            {
                waypointNo++;
                if (waypointNo >= waypoints.Length)
                {
                    waypointNo = 0;
                }
                targetWaypoint = waypoints[waypointNo];
            }

            Quaternion aimingRot = Quaternion.LookRotation(targetWaypoint.transform.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation,
                Quaternion.Euler(aimingRot.eulerAngles.x,
                aimingRot.eulerAngles.y,
                transform.rotation.eulerAngles.z),
                turnTorque * Time.deltaTime);

            if (!isUpright())
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(aimingRot.eulerAngles.x, aimingRot.eulerAngles.y, 0f), turnTorque * Time.deltaTime);
            }
        }
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
        boardRigidbody.AddForce(-Vector3.up * (gravity * Time.deltaTime), ForceMode.Acceleration);
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
