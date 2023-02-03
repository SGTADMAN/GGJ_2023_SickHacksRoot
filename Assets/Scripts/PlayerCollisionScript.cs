using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisionScript : MonoBehaviour
{
    [SerializeField] Transform respawnPoint;
    DownhillHoverboardScript hoverboardScript;
    PlayerBaseScript baseScript;
    [SerializeField] AudioSource hitSound;
    private void Start()
    {
        hoverboardScript = GetComponent<DownhillHoverboardScript>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            hitSound.Play();
            hoverboardScript.Reset();
            transform.position = respawnPoint.position;            
        }
    }
}
