using UnityEngine;
using System.Collections.Generic;

public class FPSController : MonoBehaviour
{
    [Header("Movement")]
    public float walkingSpeed = 8f;
    public float runningSpeed = 12f;
    public float jumpSpeed = 10f;
    public float zoomFOV = 50;
    private CharacterController cc;
    private Vector3 moveDirection;
    private Camera mainCamera;

    [Header("Player")]
    public float health = 100f;

    void Start()
    {
        cc = GetComponent<CharacterController>();
        moveDirection = Vector3.zero;
        mainCamera = Camera.main;
    }

    void Update()
    {
        /* Movement */
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float curSpeedX = (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Vertical");
        float curSpeedY = (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Horizontal");
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && cc.isGrounded) moveDirection.y = jumpSpeed;
        else moveDirection.y = movementDirectionY;
        if (!cc.isGrounded) moveDirection.y -= 9.81f * Time.deltaTime;

        // Crouch
        if (Input.GetKey(KeyCode.C)) cc.height = 1f;
        else cc.height = 2f;
        mainCamera.transform.localPosition = new Vector3(0, cc.height / 2, 0);

        // Zoom
        if (Input.GetKey(KeyCode.V)) mainCamera.fieldOfView = zoomFOV;
        else mainCamera.fieldOfView = 90;

        cc.Move(moveDirection * Time.deltaTime);

        /* Player */
        if (health <= 0) Destroy(gameObject);
    }
}