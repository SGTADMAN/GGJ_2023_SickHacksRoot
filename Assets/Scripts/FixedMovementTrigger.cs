using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedMovementTrigger : MonoBehaviour
{
    [SerializeField] bool endTrigger;
    HoverboardTorqueRotationScript hoverboardScript;
    Rigidbody playerRigidBody;

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        hoverboardScript = player.GetComponent<HoverboardTorqueRotationScript>();
        playerRigidBody = player.GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (endTrigger)
            {
                hoverboardScript.fixedMovement = false;
                playerRigidBody.useGravity = true;
            }
            else
            {
                hoverboardScript.fixedMovement = true;
                playerRigidBody.useGravity = false;
            }
        }
    }
}
