using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishScript : MonoBehaviour
{
    [SerializeField] GameObject followCam, finishCam;
    DownhillHoverboardScript hoverboardScript;
    RailHoverboard railHoverboardScript;
    [SerializeField] GameObject finishUIMenu;

    private void Start()
    {
        hoverboardScript = GameObject.FindGameObjectWithTag("Player").GetComponent<DownhillHoverboardScript>();
        railHoverboardScript = GameObject.FindGameObjectWithTag("Player").GetComponent<RailHoverboard>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Contains("Player"))
        {
            followCam.SetActive(false);
            finishCam.SetActive(true);
            if (hoverboardScript != null)
            {
                hoverboardScript.stop = true;
            }
            if (railHoverboardScript != null)
            {
                railHoverboardScript.stop = true;
            }            
            finishUIMenu.SetActive(true);
            Cursor.visible = true;
        }
    }
    public void LoadNextMap(string mapName)
    {
        SceneManager.LoadScene(mapName);
        Time.timeScale = 1;
    }
}
