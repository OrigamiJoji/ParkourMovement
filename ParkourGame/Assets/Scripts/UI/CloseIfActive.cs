using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CloseIfActive : MonoBehaviour
{
    public HintManager hintManager;
    public KeyCode key;
    public UnityEvent MovementFunction;
    void Update()
    {
        if(gameObject.activeSelf)
        {
            if (Input.GetKeyDown(key)) {
                gameObject.SetActive(false);
                MovementFunction?.Invoke();
                hintManager.CloseTutorial();
            }
        }

    }
}
