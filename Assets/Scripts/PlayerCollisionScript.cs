using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisionScript : MonoBehaviour
{
    [SerializeField] Transform respawnPoint;
    DownhillHoverboardScript hoverboardScript;
    private void Start()
    {
        hoverboardScript = GetComponent<DownhillHoverboardScript>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            transform.position = respawnPoint.position;
            hoverboardScript.Reset();
        }
    }
}
