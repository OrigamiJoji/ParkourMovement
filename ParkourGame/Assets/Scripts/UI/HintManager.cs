using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintManager : MonoBehaviour {
    public GameObject tutorialScreen;

    public bool hintOpen;
    [SerializeField]
    private UIManager uiManager;


    void Start() {
        OpenTutorial();
    }

    void Update() {
        if(Input.anyKeyDown && hintOpen) {
            CloseTutorial();
        }
    }


    public void OpenTutorial() {
        uiManager.MenuClose();
        UIManager.paused = true;
        hintOpen = true;
        tutorialScreen.SetActive(true);
    }
    public void CloseTutorial() {
        UIManager.paused = false;
        hintOpen = false;
        tutorialScreen.SetActive(false);

    }

}
