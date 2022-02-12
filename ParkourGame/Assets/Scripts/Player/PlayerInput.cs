using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public static float x, z;
    float inputNormal;

    void Start()
    {
        
    }


    void Update()
    {

        Vector2 input = new Vector2 (x, z);
        inputNormal = input.normalized.magnitude;
    }
}
