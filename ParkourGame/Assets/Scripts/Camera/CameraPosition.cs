using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPosition : MonoBehaviour {
    public Transform playerHead;

    public float bobbingAmplitude;
    public float walkBobbingFrequency;
    public float runBobbingFrequency;

    void Update() {
        transform.position = playerHead.position;
    }
}
