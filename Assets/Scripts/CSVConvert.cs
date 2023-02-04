using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CSVConvert : MonoBehaviour
{
    [SerializeField] string csvPath = "/Misc/spline.csv";

    public Vector3[] GetSplineVerts()
    {
        string[] allLines = File.ReadAllLines(Application.dataPath + csvPath);
        Vector3[] verts = new Vector3[allLines.Length];
        for (int i = 0; i < allLines.Length; i++)
        {
            string[] splitData = allLines[i].Split(';');
            verts[i] = new Vector3(float.Parse(splitData[0]), float.Parse(splitData[2]), float.Parse(splitData[1]));           
        }
        return verts;
    }
}
