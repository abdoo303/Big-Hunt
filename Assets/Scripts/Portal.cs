using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Represents a victory portal that loads the win screen when the player enters.
/// Marks the successful completion of the game or current level sequence.
/// </summary>
public class Portal : MonoBehaviour
{
    /// <summary>
    /// Called when a collider enters the portal's trigger zone.
    /// Confines the cursor and loads the Win scene when the player enters the portal.
    /// </summary>
    /// <param name="other">The collider that entered the trigger zone.</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Cursor.lockState = CursorLockMode.Confined;
            SceneManager.LoadScene("Win");
        }
    }
}
