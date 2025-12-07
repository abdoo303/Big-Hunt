using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Represents a level transition trigger that loads "Level3" when the player enters.
/// Typically used as a glowing door or portal to indicate progression to the next level.
/// </summary>
public class GlowingDoor : MonoBehaviour
{
    /// <summary>
    /// Called when a collider enters the trigger zone.
    /// Loads Level3 scene when the player touches the glowing door.
    /// </summary>
    /// <param name="other">The collider that entered the trigger zone.</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene("Level3");
        }
    }
}
