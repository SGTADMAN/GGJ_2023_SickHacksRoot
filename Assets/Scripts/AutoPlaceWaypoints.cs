using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoPlaceWaypoints : MonoBehaviour
{
    [Header("Waypoint Stuff")]
    [SerializeField] Transform[] wayPointList;
    [SerializeField] GameObject wayPointContainer;
    [SerializeField] Vector3[] railVerts;
    CSVConvert converter;

    private void Awake()
    {
        converter = GetComponentInChildren<CSVConvert>();
        railVerts = converter.GetSplineVerts();
        GameObject[] wayPointObj = new GameObject[railVerts.Length];
        wayPointList = new Transform[railVerts.Length];
        Matrix4x4 localToWorld = transform.localToWorldMatrix;
        for (int i = 0; i < railVerts.Length; ++i)
        {
            railVerts[i] += transform.worldToLocalMatrix.MultiplyVector(transform.position);
            wayPointObj[i] = new GameObject("wayPoint_" + i);
            Vector3 world_v = localToWorld.MultiplyVector(railVerts[i]);
            wayPointObj[i].transform.position = world_v;
            wayPointObj[i].transform.parent = wayPointContainer.transform;
            wayPointList[i] = wayPointObj[i].transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
