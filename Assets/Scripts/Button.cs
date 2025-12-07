using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a pressure-activated button that controls a door.
/// The button physically depresses when stepped on and opens a connected door,
/// then returns to its original position when the pressure is removed.
/// </summary>
public class Button : MonoBehaviour
{
    /// <summary>
    /// Reference to the Door component that this button controls.
    /// </summary>
    public Door door;

    /// <summary>
    /// Called when a collider enters the button's trigger zone.
    /// Opens the door and visually depresses the button when a Player or Object steps on it.
    /// </summary>
    /// <param name="other">The collider that entered the trigger zone.</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Object"))
        {
            door.Open();
            // Visually depress the button by moving it down 0.3 units
            transform.Translate(new Vector3(0, -0.3f, 0));
        }
    }

    /// <summary>
    /// Called when a collider exits the button's trigger zone.
    /// Closes the door and returns the button to its original position when the Player or Object leaves.
    /// </summary>
    /// <param name="other">The collider that exited the trigger zone.</param>
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Object"))
        {
            door.Close();
            // Return the button to its original position by moving it up 0.3 units
            transform.Translate(new Vector3(0, 0.3f, 0));
        }
    }
}
