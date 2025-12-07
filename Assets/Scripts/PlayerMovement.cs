using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Controls player movement using WASD/Arrow keys and handles level transitions.
/// Movement is relative to the player's current facing direction.
/// Detects finish line and fall triggers to transition between scenes.
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    /// <summary>
    /// Reference to the player's Rigidbody component used for physics-based movement.
    /// </summary>
    Rigidbody rb;

    /// <summary>
    /// Movement speed of the player in units per second.
    /// </summary>
    public float speed = 12f;

    /// <summary>
    /// Initializes the player movement system by getting the Rigidbody component.
    /// </summary>
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Updates player movement each frame based on input.
    /// Applies velocity to the Rigidbody while preserving vertical velocity (for gravity).
    /// </summary>
    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = transform.right * x + transform.forward * z;
        rb.velocity = new Vector3(move.x * speed, rb.velocity.y, move.z * speed);
    }

    /// <summary>
    /// Handles collision detection for level transitions.
    /// Detects "Finish" triggers to advance to the next level and "Fall" triggers for game over.
    /// </summary>
    /// <param name="other">The collider that entered the trigger zone.</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Finish"))
        {
            SceneManager.LoadScene("Level2");
            Cursor.lockState = CursorLockMode.Confined;
        }
        if (other.gameObject.CompareTag("Fall"))
        {
            SceneManager.LoadScene("Lose");
            Cursor.lockState = CursorLockMode.Confined;
        }
    }
}
