using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    // cam controls
    public float sensX = 800f; // fixed value for sensitivity
    public float sensY = 800f;
    float activeSensX; // value for sensitivity that changes based on sensitivity option slider
    float activeSensY;

    public Camera playerCam;

    float xRotation;
    float yRotation;

    // ground
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    public bool isGrounded;

    // physics
    CharacterController characterController;

    public GameObject playerBody;
    public float playerSpeed = 20f;
    public float jumpHeight = 7f;
    Vector3 velocity;
    public float gravity = -60f;

    public float dashSpeed = 15f;
    public float dashTime = 0.08f;
    public float maxDashCooldown = 0.9f;
    float dashCooldown;
    public int maxDashCharges = 2;
    public int dashCharges;

    public float slamSpeed = 200f;



    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        characterController = GetComponent<CharacterController>();

        dashCooldown = maxDashCooldown; // time to recharge 1 dash charge (decreases over time, restores 1 charge when less than 0 and dashCharges less than 2)
        dashCharges = maxDashCharges; // amount of consecutive dashes available

        setOptionMultipliers();
    }

    // Update is called once per frame
    void Update()
    {
        #region Grounded
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask); // ground check

        if (isGrounded && velocity.y < 0) 
        {
            velocity.y = -2f;
        }
        #endregion


        #region Rotation
        float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * activeSensX;
        float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * activeSensY;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90, 90);

        playerCam.transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        transform.rotation = Quaternion.Euler(0, yRotation, 0);
        #endregion


        #region Movement
        // Inputs
        float forward = Input.GetAxisRaw("Vertical"); // conversion from frames/s to m/s 
        float right = Input.GetAxisRaw("Horizontal");


        // Run
        float runForward = forward * playerSpeed * Time.deltaTime;
        float runRight = right * playerSpeed * Time.deltaTime;

        Vector3 move = transform.right * runRight + transform.forward * runForward;
        characterController.Move(move);


        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime; // freefall acceleration
        characterController.Move(velocity * Time.deltaTime);


        // Dash
        dashCooldown -= Time.deltaTime;
        if (dashCooldown < 0 && dashCharges < maxDashCharges)
        {
            dashCooldown = maxDashCooldown;
            dashCharges += 1;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && dashCharges > 0)
        {
            dashCooldown = maxDashCooldown;
            dashCharges -= 1;
            StartCoroutine(Dash());
        }


        // Slam
        if (Input.GetKeyDown(KeyCode.LeftControl) && !isGrounded)
        {
            StartCoroutine(Slam());
        }

        // if (!characterController.isGrounded)
        // {
        //     characterController.SimpleMove(new Vector3(0, -10, 0));
        // }

        #endregion

    }


    public void setOptionMultipliers()
    {
        activeSensX = sensX * OptionsManager.sensMultiplier;
        activeSensY = sensY * OptionsManager.sensMultiplier;
    }


    #region Coroutines
    // dash coroutine
    IEnumerator Dash()
    {
        float startTime = Time.time;

        float forward = Input.GetAxisRaw("Vertical") * playerSpeed * Time.deltaTime; 
        float right = Input.GetAxisRaw("Horizontal") * playerSpeed * Time.deltaTime;

        Vector3 direction = transform.forward * forward + transform.right * right;
        Vector3 move = direction.normalized * dashSpeed;

        while (Time.time < startTime + dashTime)
        {
            characterController.Move(move * Time.deltaTime * dashSpeed);

            yield return null;
        }

    }


    // slam coroutine
    IEnumerator Slam()
    {
        while (!isGrounded)
        {
            velocity.y = -slamSpeed;

            yield return null;
        }

    }
    #endregion
}


