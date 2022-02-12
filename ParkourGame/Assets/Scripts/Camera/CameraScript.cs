using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {
    /*
    public float walkSpeed;
    public float walkRate;
    public float sprintSpeed;
    public float sprintRate;
    public float bobbingHeight;
    public float timer;

    private bool direction;
    private Vector3 cameraPos;
    private Vector3 lowPos;
    private Vector3 highPos;

    public Transform headPos;
    //Note: Gets input to trigger view bobbing; change to velocity.

    void Update() {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        GetPos();
        if (PlayerMovement.isSprinting && PlayerMovement.isGrounded && !PlayerMovement.isCrouched && (x != 0 || z != 0)) {
            if (timer < sprintRate) {
                timer += Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, SwitchPos(direction), Time.deltaTime * sprintSpeed);
            }
            else {
                timer = 0;
                direction = !direction;
            }
        }
        else if (!PlayerMovement.isSprinting && PlayerMovement.isGrounded && !PlayerMovement.isCrouched && (x != 0 || z != 0)) {
            if(timer < walkRate) {
                timer += Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, SwitchPos(direction), Time.deltaTime * walkSpeed);
            }
            else {
                timer = 0;
                direction = !direction;
            }
        }

    }

    private void GetPos() {
        cameraPos = headPos.position;
        lowPos = new Vector3(cameraPos.x, cameraPos.y - bobbingHeight, cameraPos.z);
        highPos = new Vector3(cameraPos.x, cameraPos.y + bobbingHeight, cameraPos.z);
    }

    private Vector3 SwitchPos(bool dir) {
        switch (dir) {
            case true:
                return lowPos;
            case false:
                return highPos;
        }

    }
    */
}
