using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CSVConvert : MonoBehaviour
{

    [SerializeField] string csvPath = "spline";

    public Vector3[] GetSplineVerts()
    {
        TextAsset txt = (TextAsset)Resources.Load(csvPath);
        List<string> lines = new List<string>(txt.text.Split(System.Environment.NewLine));
        lines.RemoveAt(lines.Count - 1);
        string[] allLines = lines.ToArray();
        
        Vector3[] verts = new Vector3[allLines.Length];
        for (int i = 0; i < allLines.Length; i++)
        {
            string preSpilt = allLines[i];
            preSpilt.Replace(System.Environment.NewLine, "");
            string[] splitData = preSpilt.Split(';');
            verts[i] = new Vector3(float.Parse(splitData[0]), float.Parse(splitData[2]), float.Parse(splitData[1]));           
        }
        return verts;
    }
}
