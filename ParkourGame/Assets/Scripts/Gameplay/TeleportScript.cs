using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportScript : MonoBehaviour
{
    public GameObject player;
    private Rigidbody playerRB;
    public Transform destination;

    private void Awake() {
        playerRB = player.GetComponent<Rigidbody>();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.R)) {
            player.transform.position = destination.position;
            player.transform.rotation = transform.rotation;
            playerRB.velocity = Vector3.zero;

        }
    }
    private void OnTriggerEnter(Collider collision) {
        if (collision.gameObject.CompareTag("Player")) {
            player.transform.position = destination.position;
            player.transform.rotation = transform.rotation;
            playerRB.velocity = Vector3.zero;
        }
    }

}
