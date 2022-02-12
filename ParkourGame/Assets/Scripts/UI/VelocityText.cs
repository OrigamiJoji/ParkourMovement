using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class VelocityText : MonoBehaviour {
    public Rigidbody rb;
    public PlayerMovement pm;
    public TextMeshProUGUI currentVelText;
    public TextMeshProUGUI highestVelText;
    public Slider timerSlider; 
    public Slider grappleCooldownSlider;
    private float highestVelocity;
    public GameObject grappleIcon;
    public GameObject highJumpIcon;

    float grappleTimer;
    private void Start() {
        PlayerMovement.GrappleCooldown += OnGrappleCooldown;
        grappleTimer = 0;
    }
    void Update() {
        if (rb.velocity.magnitude > highestVelocity) {
            highestVelocity = rb.velocity.magnitude;
        }

        currentVelText.text = rb.velocity.magnitude.ToString("F2");
        highestVelText.text = highestVelocity.ToString("F2");
        timerSlider.value = pm.slowDuration;

        if (grappleTimer >= 0) {
            grappleCooldownSlider.gameObject.SetActive(true);
            grappleTimer -= Time.deltaTime;
            grappleCooldownSlider.value = grappleTimer * 100;
        }
        else { grappleCooldownSlider.gameObject.SetActive(false); }

        if(pm.isLookingAtGrappleTarget) {
            grappleIcon.SetActive(true);
        }
        else { grappleIcon.SetActive(false); }

        if(pm.highJumpReady) {
            highJumpIcon.SetActive(true);
        }
        else { highJumpIcon.SetActive(false); }
    }

    private void OnGrappleCooldown(float duration) {
        grappleTimer = duration;
    }
}
