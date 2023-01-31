using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishScript : MonoBehaviour
{
    [SerializeField] GameObject followCam, finishCam;
    DownhillHoverboardScript hoverboardScript;

    private void Start()
    {
        hoverboardScript = GameObject.FindGameObjectWithTag("Player").GetComponent<DownhillHoverboardScript>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Contains("Player"))
        {
            followCam.SetActive(false);
            finishCam.SetActive(true);
            hoverboardScript.stop = true;
        }
    }
}
