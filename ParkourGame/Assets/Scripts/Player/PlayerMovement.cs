using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerLook))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerMovement : MonoBehaviour {

    /// <summary>
    /// Notes:
    /// 
    /// ledge climb - 
    /// hookshot - 
    /// </summary>

    #region Movement Variables

    [Header("~ Movement <3")]
    [Tooltip("Normal speed of player")]
    public float walkSpeed;
    [Tooltip("Speed of player when sprinting")]
    public float sprintSpeed;
    [Tooltip("Rate that player goes from walking to sprinting")]
    public float acceleration;
    [Tooltip("Speed player walks while crouched")]
    public float crouchSpeed;
    [Tooltip("Speed of player while mid-air")]
    public float airSpeed;



    [HideInInspector]
    public float moveSpeed;
    private float x, z;
    private Vector3 moveDir;
    private Vector3 slopeDir;

    private bool isGrounded;
    private bool isCrouched;
    private bool isWalking;
    private bool isJumping;
    private bool isSliding;
    private bool isOnSlope;

    private Vector3 characterScale;
    private Vector3 crouchScale;
    #endregion
    #region Slow Time
    public float slowAssist;
    public bool isSlowed;
    public float slowDuration;
    [Tooltip("Amount of time regenerated per second")]
    public float timeRegen;
    [Tooltip("Amount of time it costs to start a time slow")]
    public float intialTimeCost;
    [Tooltip("Amount of time it costs to slow time for 1 real second")]
    public float timeCost;
    #endregion
    #region Slide Variables

    [Header("~ Sliding <3")]
    [Tooltip("Amount of force player gains while sliding")]
    public float slideForce;
    [Tooltip("Current force required to initiate a slide")]
    public float slideReq;
    [Tooltip("Reduction of the current angle amount of force added to player")]
    public float slopeSlideSpeedModifier;
    [Tooltip("Angle the player must be on to initiate a slope slide")]
    public float slopeSlideReq;
    [Tooltip("Amount of force player can add while slope sliding to adjust direction")]
    public float slopeSlideAdj;
    [Tooltip("Raycast distance above head to check if player can uncrouch")]
    public float uncrouchDist;

    private Vector3 slopeSlideDir;
    #endregion
    #region Drag Variables

    [Header("~ Drag <3")]
    [Tooltip("Simulated friction")]
    public float groundDrag;
    [Tooltip("Amount of drag player has mid-air")]
    public float airDrag;
    [Tooltip("Amount of drag player has while sliding, leave same as groundDrag if unwanted")]
    public float slideDrag;
    public float moveMultiplier;
    #endregion
    #region Jumping

    [Header("~ Jumping <3")]
    [Tooltip("Force player gains while jumping")]
    public float jumpForce;
    [Tooltip("Force player gains while high jumping")]
    public float highJumpForce;
    [Tooltip("EXTRA force player gains while jumping from a slope")]
    public float slopeJumpForce;
    [Tooltip("Extra gravity added to player, leave at 0 for none")]
    public float synthGravity;

    public bool highJumpReady;

    #endregion
    #region Ground Detection

    [Header("~ Ground Detection <3")]
    [Tooltip("Set to ground layer, set ground layer on all walkable objects")]
    public LayerMask whatIsGround;
    [Tooltip("Distance player must be from ground to be 'grounded'")]
    public float groundCheckRadius;
    [Tooltip("Distance player must be from ground to be 'sloping'")]
    public float slopeDist;

    private RaycastHit slopeHit;
    private Vector3 castPos;
    #endregion
    #region Wall Running

    [Header("~ Wall Running <3")]

    [Tooltip("Set to wall layer, set wall layer on all wall runnable objects")]
    public LayerMask whatIsWall;
    [Tooltip("Simulated gravity on wall")]
    public float wallGravity;
    [Tooltip("Force player gains when jumping from wall")]
    public float wallReleaseForce;
    [Tooltip("Distance player must be from wall to initate wall run")]
    public float floorDist;
    [Tooltip("Distance player must be from wall to be considered on wall")]
    public float wallDist;

    private bool wallOnLeft;
    private bool wallOnRight;

    private RaycastHit leftWallRaycast;
    private RaycastHit rightWallRaycast;
    #endregion
    #region Ledge Grab

    public bool canGrabLedge;
    public Animator pa;
    #endregion
    #region Grapple

    public float grappleDist;
    public LayerMask whatCanGrapple;
    public LayerMask playerMask;
    public float grappleSpeed;
    public bool isGrappling;
    public bool grappleOnCooldown;

    private Vector3 grapplePos;
    public bool isLookingAtGrappleTarget;

    public delegate void HUDManagement(float duration);
    public static event HUDManagement GrappleCooldown;
    #endregion
    #region Reference
    [Header("~ References <3")]
    [Tooltip("Player Rigidbody")]
    public Rigidbody rb;
    [Tooltip("PlayerLook Script")]
    public PlayerLook pl;
    [Tooltip("Orientation object; child of player")]
    public Transform orientation;
    [Tooltip("FeetPosition; child of player")]
    public Transform feetPos;
    [Tooltip("HeadPosition; child of player")]
    public Transform headPos;
    public CapsuleCollider body;
    public GameObject playerCamera;
    public LineRenderer grapplingHook;

    #endregion
    #region Awake, Start
    private void Awake() {
        pa.GetComponent<Animator>();
        rb.GetComponent<Rigidbody>();
        pl.GetComponent<PlayerLook>();
    }
    private void Start() {
        characterScale = gameObject.transform.localScale;
        crouchScale = new Vector3(characterScale.x, characterScale.y / 2, characterScale.z);
        playerMask = ~playerMask;
        slowDuration = 100;
    }
    #endregion
    #region Update, FixedUpdate

    void Update() {
        z = Input.GetAxisRaw("Horizontal");
        x = Input.GetAxisRaw("Vertical");
        moveDir = x * orientation.forward + z * orientation.right;
        slopeDir = Vector3.ProjectOnPlane(moveDir, slopeHit.normal);
        slopeSlideDir = Vector3.ProjectOnPlane(orientation.forward, slopeHit.normal);
        SlowTime();

        CheckIfGrounded();
        JumpCheck();
        DragControl();
        SprintControl();
        WallCheck();
        isOnSlope = OnSlope();
        Grapple();


        if (Input.GetKeyDown(KeyCode.LeftControl)) {
            gameObject.transform.localScale = crouchScale;
            SlideControl();
        }
        else if (Input.GetKeyUp(KeyCode.LeftControl)) {
            StartCoroutine(Uncrouch());
        }

    }

    private void FixedUpdate() {
        Grappling();
        ApplyMovement();
        ApplyLocalGravity();
    }
    #endregion
    #region Gravity
    private void ApplyLocalGravity() {
        if (!isGrounded) {
            rb.AddForce(Vector3.down * synthGravity * Time.deltaTime);
        }
    }
    #endregion
    #region Movement
    /// <summary>
    /// Changes movement speeds and directions depending on current player conditions
    /// </summary>
    private void ApplyMovement() {

        if (isGrounded && !isCrouched && !isSliding && CurrentSlopeAngle() < slopeSlideReq && !isGrappling) { //normal walk
            //Debug.Log("1");
            rb.AddForce(moveDir.normalized * moveSpeed * moveMultiplier, ForceMode.Acceleration);
        }
        else if (isGrounded && isOnSlope && CurrentSlopeAngle() > slopeSlideReq && isCrouched && !isGrappling) { //slope slide
            //Debug.Log("2");
            pl.SlideFOV();
            rb.AddForce(Vector3.down * slopeSlideSpeedModifier * moveMultiplier, ForceMode.Acceleration);
        }
        else if (isGrounded && isOnSlope && !isCrouched && !isGrappling) { //slope walk
            //Debug.Log("3");
            rb.AddForce(slopeDir.normalized * moveSpeed * moveMultiplier, ForceMode.Acceleration);
        }
        else if (isGrounded && isCrouched && !isSliding && CurrentSlopeAngle() < slopeSlideReq && !isGrappling) {//crouch walk
            //Debug.Log("4");
            rb.AddForce(moveDir.normalized * crouchSpeed * moveMultiplier, ForceMode.Acceleration);
        }
        else if (!isGrounded && !isOnSlope && !isSliding && !isGrappling) {//in air
            //Debug.Log("5");
            rb.AddForce(moveDir.normalized * airSpeed * moveMultiplier, ForceMode.Acceleration);
        }

    }
    #endregion
    #region Drag
    /// <summary>
    /// Sets drag depending if player is mid-air or grounded
    /// </summary>
    private void DragControl() {
        if (isGrounded && !isSliding) {
            rb.drag = groundDrag;
        }
        else if (isGrounded && isSliding) {
            rb.drag = slideDrag;
        }
        else if (!isGrounded) {
            rb.drag = airDrag;
        }
    }
    #endregion
    #region Jumping
    /// <summary>
    /// Determines if player has pressed spacebar, and is eligible for a jump
    /// </summary>
    private void JumpCheck() {

        if (Input.GetKey(KeyCode.Space) && isGrounded && isCrouched && CurrentSlopeAngle() < slopeSlideReq && !UIManager.paused) {
            if (HighJumpControl() != null) {
                StartCoroutine(HighJumpControl());
            }
        }
        else if (Input.GetKeyUp(KeyCode.Space) && isGrounded && !isOnSlope && isCrouched && highJumpReady && !UIManager.paused) {
            HighJump();
        }
        else if (Input.GetKeyDown(KeyCode.Space) && isGrounded && CurrentSlopeAngle() > slopeSlideReq && isCrouched && !UIManager.paused) {
            pl.SlopeJumpFOV();
            SlopeJump();
        }
        else if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isCrouched && CurrentSlopeAngle() < slopeSlideReq && !UIManager.paused) {
            Jump();
        }
    }
    private void SlopeJump() {
        Debug.Log("Slope Jump");
        float currentVel = Vector3.Dot(rb.velocity, orientation.forward);
        rb.velocity = Vector3.zero;
        rb.AddForce(CurrentSlopeNormal() * currentVel * slopeJumpForce, ForceMode.Impulse);
    }
    IEnumerator HighJumpControl() {
        while (Input.GetKey(KeyCode.Space) && x == 0 && z == 0) {
            yield return new WaitForSeconds(0.5f);
            highJumpReady = true;
            yield return null;
        }
        highJumpReady = false;
        yield break;
    }

    private void HighJump() {
        Debug.Log("High Jump");
        rb.AddForce(Vector3.up * highJumpForce, ForceMode.Impulse);
        isGrounded = false;
        highJumpReady = false;
        isJumping = true;
    }
    /// <summary>
    /// Adds a jumping force, removed the isGrounded attribute from player
    /// </summary>

    public void Jump() {
        Debug.Log("Jump");
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isGrounded = false;
        isJumping = true;
    }
    #endregion
    #region Crouching/Sliding

    IEnumerator Uncrouch() {
        while (!Input.GetKey(KeyCode.LeftControl)) {
            if (!Physics.Raycast(headPos.transform.position, Vector3.up, uncrouchDist)) {
                Debug.DrawRay(headPos.transform.position, Vector3.up, Color.magenta);
                gameObject.transform.localScale = characterScale;
                isCrouched = false;
            }
            yield return null;
        }
        yield break;
    }
    private void SlideControl() {
        isCrouched = true;
        if (rb.velocity.magnitude > slideReq && !isOnSlope) {
            ApplySlide();
        }
        else { return; }
    }

    private void StopCrouching() {
        isCrouched = false;
    }
    /// <summary>
    /// Sets the player's size to the crouchScale, then calls ApplySlide
    /// </summary>
    private void ApplySlide() {
        if (isGrounded) {
            isSliding = true;
            isCrouched = false;
            Vector3 currentVelocity = rb.velocity;
            rb.AddForce(currentVelocity * slideForce, ForceMode.Impulse);
            StartCoroutine(StopSliding());
        }
    }

    /// <summary>
    /// if control is held 
    /// </summary>
    /// <returns></returns>
    IEnumerator StopSliding() {
        while (Input.GetKey(KeyCode.LeftControl)) {
            if (rb.velocity.magnitude < 5 && CurrentSlopeAngle() == 0) {
                isSliding = false;
                isCrouched = true;
                Debug.Log("Sliding stopped (via mag) at " + Time.time);
                yield break;
            }
            yield return null;
        }
        isCrouched = true;
        isSliding = false;
        StartCoroutine(Uncrouch());
        Debug.Log("Sliding stopped (via ctrl) at " + Time.time);
        yield break;
    }

    #endregion
    #region Checks

    private Vector3 CurrentSlopeNormal() {
        if (Physics.Raycast(castPos, Vector3.down, out RaycastHit hit, slopeDist, whatIsGround)) {
            return hit.normal;
        }
        return Vector3.zero;
    }
    private float CurrentSlopeAngle() {
        if (Physics.Raycast(castPos, Vector3.down, out RaycastHit hit, slopeDist, whatIsGround)) {
            Debug.DrawRay(castPos, Vector3.down, Color.red);
            float angle = 180 - Vector3.Angle(hit.normal, -transform.up);
            return angle;
        }
        return 0f;
    }

    private bool OnSlope() {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, slopeDist, whatIsGround)) {
            Debug.DrawRay(transform.position, Vector3.down, Color.cyan);
            if (slopeHit.normal != Vector3.up) {
                return true;
            }
            else { return false; }
        }
        else { return false; }
    }

    private void CheckIfGrounded() {
        castPos = feetPos.position;
        if (Physics.CheckSphere(castPos, groundCheckRadius, whatIsGround)) {
            isGrounded = true;
        }
        else { isGrounded = false; }
    }
    #endregion
    #region Sprinting

    private void SprintControl() {
        if (Input.GetKey(KeyCode.LeftShift) && isGrounded) {
            moveSpeed = Mathf.Lerp(moveSpeed, sprintSpeed, acceleration * Time.deltaTime);
        }
        else { moveSpeed = Mathf.Lerp(moveSpeed, walkSpeed, acceleration * Time.deltaTime); }

        if (Input.GetKey(KeyCode.LeftShift) && !isCrouched) {
            pl.SprintFOV();
        }
        else if (!isCrouched) { pl.NormalFOV(); }
    }

    #endregion
    #region Wall Running
    private bool CanWallRun() {
        return !Physics.Raycast(transform.position, Vector3.down, floorDist);
    }
    private void WallCheck() {
        wallOnLeft = Physics.Raycast(transform.position, -orientation.right, out leftWallRaycast, wallDist, whatIsWall);
        wallOnRight = Physics.Raycast(transform.position, orientation.right, out rightWallRaycast, wallDist, whatIsWall);

        if (CanWallRun()) {
            if (wallOnLeft) {
                StartWallRun();
            }
            else if (wallOnRight) {
                StartWallRun();
            }
            else {
                StopWallRun();
            }
        }
        else {
            StopWallRun();
        }

    }
    void StartWallRun() {
        rb.useGravity = false;
        rb.AddForce(Vector3.down * wallGravity, ForceMode.Force);
        if (wallOnLeft) {
            pl.OnLeftWallTilt();
        }
        else if (wallOnRight) {
            pl.OnRightWallTilt();
        }

        if (Input.GetKeyDown(KeyCode.Space)) {
            if (wallOnLeft) {
                Vector3 wallRunJumpDirection = transform.up + leftWallRaycast.normal;
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                rb.AddForce(wallRunJumpDirection * wallReleaseForce * 100, ForceMode.Force);
            }
            else if (wallOnRight) {
                Vector3 wallRunJumpDirection = transform.up + rightWallRaycast.normal;
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                rb.AddForce(wallRunJumpDirection * wallReleaseForce * 100, ForceMode.Force);
            }
        }
    }

    void StopWallRun() {
        rb.useGravity = true;
        pl.NormalTilt();
    }
    #endregion
    #region Ledge Climb
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Ledge")) {
            canGrabLedge = true;
        }
    }

    private void LedgeGrab() {
        if (canGrabLedge) {
            pa.SetTrigger("Grab");
        }
    }
    #endregion
    #region Grapple

    private void Grapple() {
        grapplingHook.SetPosition(0, gameObject.transform.position);
        bool grappleInFrontofPlayer = false;
        if(Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, grappleDist, whatCanGrapple)) {
            grappleInFrontofPlayer = true;
        };
        Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit2, grappleDist, playerMask);
        if (hit2.collider == hit.collider && grappleInFrontofPlayer) {
            if (Input.GetMouseButtonDown(0) && !isGrappling && !grappleOnCooldown) {
                grapplePos = hit.point;
                AttachGrapple();
            }
            isLookingAtGrappleTarget = true;
        }
        else { isLookingAtGrappleTarget = false; }
    }

    private void AttachGrapple() {
        grapplingHook.SetPosition(1, grapplePos);
        isGrappling = true;
        grapplingHook.enabled = true;
    }

    private void DetachGrapple() {
        grapplingHook.enabled = false;
        isGrappling = false;
        grappleOnCooldown = true;
        StartCoroutine(EndGrappleCooldown());
    }

    IEnumerator EndGrappleCooldown() {
        yield return new WaitForSeconds(0.5f);
        grappleOnCooldown = false;
        yield break;
    }

    private void Grappling() {
        Vector3 grappleDir = grapplePos - transform.position;
        float grappleDistance = Vector3.Distance(transform.position, grapplePos);
        if (grapplePos == Vector3.zero) {
            DetachGrapple();
        }
        else if (isGrappling && grapplePos != Vector3.zero) {

            rb.velocity = grappleDir * grappleSpeed;
            if (grappleDistance < 2) {
                DetachGrapple();
                GrappleCooldown?.Invoke(0.5f);
            }
        }
    }

    #endregion
    #region Time Slow

    private void SlowTime() {
        if (Input.GetMouseButton(1) && slowDuration > intialTimeCost && !UIManager.paused) {
            if (!isSlowed) { //Start
                slowDuration -= intialTimeCost;
                Time.timeScale = 0.25f;
                isSlowed = true;
                pl.ReduceSaturation();

            } //Update
            if (slowDuration >= 0) {
                slowDuration -= Time.unscaledDeltaTime * timeCost;
            }
        }

        else {
            if (isSlowed) {
                Time.timeScale = 1;
                isSlowed = false;
                pl.RestoreSaturation();
            }
            if (slowDuration < 100) {
                slowDuration += Time.deltaTime * timeRegen;
            }
        }
    }

    #endregion

    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(castPos, groundCheckRadius);
    }
}
