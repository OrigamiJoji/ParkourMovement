using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {
    [SerializeField]
    private GameObject Menu;
    public static bool paused;

    [SerializeField]
    private HintManager hintManager;

    void Start() {
        MenuClose();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape) && !paused) {
            MenuOpen();
        }
        else if (paused) {
            if (Input.GetKeyDown(KeyCode.Escape)) {
                MenuClose();
            }
        }
    }

    private void MenuOpen() {
        Menu.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0;
        paused = true;
    }

    public void MenuClose() {
        Menu.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1;
        paused = false;
    }
}
