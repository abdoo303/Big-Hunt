using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the movement state of enemy characters.
/// This component tracks whether an enemy is currently moving and provides methods to control that state.
/// </summary>
public class EnemyAnimation : MonoBehaviour
{
    /// <summary>
    /// Flag indicating whether the enemy is currently in a moving state.
    /// Used by Enemy component to determine if the enemy should chase the player.
    /// </summary>
    public bool isMoving = false;

    /// <summary>
    /// Activates the enemy's movement state.
    /// Called to trigger the enemy to start chasing the player.
    /// </summary>
    public void StartMove()
    {
        isMoving = true;
    }
}
