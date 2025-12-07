using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls first-person camera look mechanics using mouse input.
/// Locks the cursor to the game window and provides smooth camera rotation
/// with vertical angle clamping to prevent over-rotation.
/// Typically attached to the camera GameObject.
/// </summary>
public class MouseLock : MonoBehaviour
{
    /// <summary>
    /// Mouse sensitivity multiplier for look speed.
    /// Higher values result in faster camera rotation.
    /// </summary>
    public float mouseSensitivity = 5000f;

    /// <summary>
    /// Reference to the player body Transform for horizontal rotation.
    /// The camera rotates vertically while the body rotates horizontally.
    /// </summary>
    public Transform playerBody;

    /// <summary>
    /// Accumulated vertical rotation in degrees.
    /// Clamped to prevent camera flipping.
    /// </summary>
    float xRotation = 0f;

    /// <summary>
    /// Initializes the mouse look system by locking the cursor to the game window.
    /// </summary>
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    /// <summary>
    /// Updates camera rotation based on mouse input each frame.
    /// Vertical rotation (pitch) is applied to the camera with clamping between -80° and 50°.
    /// Horizontal rotation (yaw) is applied to the player body.
    /// </summary>
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 50f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
