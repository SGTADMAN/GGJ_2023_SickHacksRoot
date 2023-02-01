using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBaseScript : MonoBehaviour
{
    [SerializeField] Transform[] wayPointList;
    int waypointNo;
    public Transform targetWaypoint;
    public Transform lastWaypointPos;
    public Transform startingPoint;
    [SerializeField] float forwardSpeed = 10;
    [SerializeField] float rotationSpeed = 10;
    [SerializeField] float yOffset;
    [SerializeField] CinemachineVirtualCamera virtualCamera;
    private void Start()
    {
        waypointNo = 0;
        targetWaypoint = wayPointList[waypointNo];
    }
    public void Reset()
    {
        waypointNo = 0;
        targetWaypoint = wayPointList[waypointNo];
        virtualCamera.LookAt = targetWaypoint;
        lastWaypointPos = startingPoint;
    }
    private void FixedUpdate()
    {
        float step = forwardSpeed * Time.deltaTime;
        Vector3 cameraForward = virtualCamera.transform.forward;
        cameraForward.x = 0; cameraForward.z = 0;
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(targetWaypoint.position.x, targetWaypoint.position.y + yOffset, targetWaypoint.position.z), step);

        transform.forward = Vector3.RotateTowards(transform.position,
                cameraForward,
                rotationSpeed * Time.deltaTime, 0.0f);
    }

    public void UpdateWayPoint()
    {
        try
        {
            waypointNo++;
            lastWaypointPos = targetWaypoint;
            targetWaypoint = wayPointList[waypointNo];
            virtualCamera.LookAt = targetWaypoint;
        }
        catch(Exception e)
        {
            Debug.Log("Error: " + e.Message);
            lastWaypointPos = wayPointList[wayPointList.Length - 2];
            targetWaypoint = wayPointList[wayPointList.Length - 1];
            virtualCamera.LookAt = targetWaypoint;
        }
    }
}
