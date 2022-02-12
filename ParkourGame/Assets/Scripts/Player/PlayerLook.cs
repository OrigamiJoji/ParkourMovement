using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PlayerLook : MonoBehaviour {
    #region Field Of View
    [Header("~ Field Of View <3")]
    [Tooltip("Players default field of vision")]
    public float fieldOfView;
    [Tooltip("General rate of FOV change")]
    public float normalRate;
    [Tooltip("FOV when sliding down slope")]
    public float slidingFOV;
    [Tooltip("Rate that changes FOV to slidingFOV")]
    public float slidingFOVRate;
    [Tooltip("Rate that FOV returns to normal after stopping a slope slide")]
    public float slideJumpFOVRate;
    [Tooltip("FOV while sprinting")]
    public float sprintFOV;
    [Tooltip("Rate FOV changes while sprinting")]
    public float sprintFOVRate;

    private float FOV;
    #endregion
    #region Tilt
    [Header("~ Tilt <3")]
    [Tooltip("Angle player rotates while wall running")]
    public float wallRunAngle;
    [Tooltip("Rate that player tilts on a wall")]
    public float wallTiltRate;

    private float tilt;
    #endregion
    #region Sensitivity
    [Header("~ Sensitivity <3")]
    [Tooltip("Mouse X sensitivity")]
    public float sensX;
    [Tooltip("Mouse Y sensitivity")]
    public float sensY;
    private float mouseX;
    private float mouseY;
    #endregion
    #region References
    [Header("~ References <3")]
    [SerializeField]
    [Tooltip("Player Movement script")]
    PlayerMovement pm;
    [Tooltip("Player Camera (Component not GameObject")]
    public Camera playerCamera;
    [Tooltip("Player orientation object, child of player")]
    public Transform orientation;

    private float multiplier = 0.01f;
    private float xRotation;
    private float yRotation;
    #endregion
    public PostProcessVolume c_ProcessVolume;
    public ColorGrading c_ColorGrading;

    private void Awake() {
        pm.GetComponent<PlayerMovement>();
    }
    private void Start() {

        c_ColorGrading = ScriptableObject.CreateInstance<ColorGrading>();
        if (c_ProcessVolume.profile.TryGetSettings<ColorGrading>(out ColorGrading cg)) {
            c_ColorGrading = cg;
        }
    }

    private void Update() {
        if (!UIManager.paused) {
            mouseX = Input.GetAxisRaw("Mouse X");
            mouseY = Input.GetAxisRaw("Mouse Y");

            yRotation += mouseX * sensX * multiplier;
            xRotation -= mouseY * sensY * multiplier;

            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            playerCamera.transform.rotation = Quaternion.Euler(xRotation, yRotation, tilt);
            orientation.transform.rotation = Quaternion.Euler(0, yRotation, 0);
            playerCamera.fieldOfView = FOV;
        }
    }

    public void OnLeftWallTilt() {
        tilt = Mathf.Lerp(tilt, -wallRunAngle, Time.deltaTime * wallTiltRate);
    }

    public void OnRightWallTilt () {
        tilt = Mathf.Lerp(tilt, wallRunAngle, Time.deltaTime * wallTiltRate);
    }

    public void NormalTilt() {
        tilt = 0;
    }

    public void SlideFOV() {
        FOV = Mathf.Lerp(FOV, slidingFOV, Time.deltaTime * slidingFOVRate);
    }

    public void SlopeJumpFOV() {
        FOV = Mathf.Lerp(FOV, fieldOfView, Time.deltaTime * slideJumpFOVRate);
    }

    public void SprintFOV() {
        FOV = Mathf.Lerp(FOV, sprintFOV, Time.deltaTime * sprintFOVRate);
    }

    public void NormalFOV() {
        FOV = Mathf.Lerp(FOV, fieldOfView, Time.deltaTime * normalRate);
    }

    public void ReduceSaturation() {
        c_ColorGrading.saturation.Override(-90);
    }

    public void RestoreSaturation() {
        c_ColorGrading.saturation.Override(0);
    }
}
