using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interactive lever that toggles a connected door when the player presses 'E'.
/// The lever rotates between two positions (50° and 140° on Z axis) to represent its state.
/// </summary>
public class Lever : MonoBehaviour
{
    /// <summary>
    /// Reference to the Door that this lever controls.
    /// </summary>
    public Door door;

    /// <summary>
    /// Current toggle state of the lever (false = down, true = up).
    /// </summary>
    bool toggled = false;

    /// <summary>
    /// Flag indicating whether the player is within interaction range.
    /// </summary>
    bool playerInRange = false;

    /// <summary>
    /// Checks for player interaction input and toggles the lever/door state.
    /// </summary>
    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (toggled)
            {
                // Rotate lever to down position (50 degrees on Z axis)
                transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, 50);
                toggled = false;
                door.Toggle();
            }
            else
            {
                // Rotate lever to up position (140 degrees on Z axis)
                transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, 140);
                toggled = true;
                door.Toggle();
            }
        }
    }
    /// <summary>
    /// Called when a collider enters the lever's trigger zone.
    /// Sets playerInRange to true if the collider is tagged as "Player".
    /// </summary>
    /// <param name="other">The collider that entered the trigger zone.</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }
    /// <summary>
    /// Called when a collider exits the lever's trigger zone.
    /// Sets playerInRange to false if the collider is tagged as "Player".
    /// </summary>
    /// <param name="other">The collider that exited the trigger zone.</param>
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
