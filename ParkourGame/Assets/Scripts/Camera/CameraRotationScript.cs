using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotationScript : MonoBehaviour {
    public float xInput;
    public float zInput;
    public float angleOffset;

    void Update() {
        zInput = -Input.GetAxis("Horizontal");
        xInput = Input.GetAxis("Vertical");

        if (zInput == 1 && Input.GetKey(KeyCode.LeftShift)) {
            gameObject.transform.rotation = Quaternion.Euler(0, 90, angleOffset);
        }
        else if (zInput == -1 && Input.GetKey(KeyCode.LeftShift)) {
            gameObject.transform.rotation = Quaternion.Euler(0, 90, -angleOffset);
        }
        else {
            gameObject.transform.rotation = Quaternion.Euler(0, 90, 0);
        }
    }
}
