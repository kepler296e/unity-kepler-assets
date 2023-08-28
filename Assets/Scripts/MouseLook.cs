using UnityEngine;
using System.Collections.Generic;

public class MouseLook : MonoBehaviour
{
    public float lookSpeed = 2.5f;
    public float lookLimit = 180f;
    private float rotationX;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        rotationX -= Input.GetAxis("Mouse Y") * lookSpeed;
        rotationX = Mathf.Clamp(rotationX, -lookLimit, lookLimit);
        transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.parent.transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
    }
}