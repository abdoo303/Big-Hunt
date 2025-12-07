using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Represents a water hazard that triggers a game over when the player falls into it.
/// Loads the "WaterLose" scene to indicate death by drowning or falling into water.
/// </summary>
public class Water : MonoBehaviour
{
    /// <summary>
    /// Called when a collider enters the water trigger zone.
    /// Confines the cursor and loads the water-specific lose scene when the player touches water.
    /// </summary>
    /// <param name="other">The collider that entered the trigger zone.</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Cursor.lockState = CursorLockMode.Confined;
            SceneManager.LoadScene("WaterLose");
        }
    }
}
