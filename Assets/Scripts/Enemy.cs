using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

/// <summary>
/// AI-controlled enemy that chases the player using Unity's NavMesh system.
/// Activates when the player enters detection range and triggers game over on contact.
/// Features proximity-based animation and audio activation.
/// </summary>
public class Enemy : MonoBehaviour
{
    /// <summary>
    /// NavMeshAgent component for AI pathfinding and movement.
    /// </summary>
    NavMeshAgent agent;

    /// <summary>
    /// Animator component for controlling enemy animations.
    /// </summary>
    private Animator animator;

    /// <summary>
    /// Reference to the player GameObject being chased.
    /// </summary>
    private GameObject player;

    /// <summary>
    /// Detection radius - distance at which the enemy activates and starts chasing.
    /// Visualized in the Scene view with a red gizmo sphere.
    /// </summary>
    public float mobDistanceRun = 4f;

    /// <summary>
    /// Movement speed of the enemy when chasing the player.
    /// </summary>
    public float chaseSpeed = 5f;

    /// <summary>
    /// Reference to the EnemyAnimation component that controls movement state.
    /// </summary>
    public EnemyAnimation enemyAnimation;

    /// <summary>
    /// AudioSource for playing enemy sounds (e.g., zombie groans).
    /// </summary>
    private AudioSource zombieSound;

    /// <summary>
    /// Initializes the enemy by getting required components and finding the player.
    /// </summary>
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.Find("Player");
        animator = GetComponentInChildren<Animator>();
        zombieSound = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Updates enemy behavior each frame.
    /// Calculates distance to player, activates when in range, and triggers game over on contact.
    /// </summary>
    void Update()
    {
        float distance = Vector3.Distance(transform.position, player.transform.position);
        CheckDistanceRun(distance);
        agent.speed = chaseSpeed;
        if (distance < 1.5)
        {
            SceneManager.LoadScene("Lose");
            Cursor.lockState = CursorLockMode.Confined;
        }
        if (enemyAnimation.isMoving)
        {
            agent.SetDestination(player.transform.position);
        }
    }

    /// <summary>
    /// Checks if the player is within detection range and activates the enemy.
    /// Enables the animator and audio when the player gets too close.
    /// </summary>
    /// <param name="distance">Current distance between enemy and player.</param>
    public void CheckDistanceRun(float distance)
    {
        if(distance < mobDistanceRun)
        {
            animator.enabled = true;
            zombieSound.enabled = true;
        }
    }

    /// <summary>
    /// Draws a visual representation of the enemy's detection radius in the Scene view.
    /// Shows a red wire sphere with translucent fill to indicate the activation range.
    /// </summary>
    void OnDrawGizmosSelected()
    {
        // Wire sphere
        Gizmos.color = new Color(1f, 0.3f, 0.3f, 1f);
        Gizmos.DrawWireSphere(transform.position, mobDistanceRun);

        // Translucent fill
        Gizmos.color = new Color(1f, 0.3f, 0.3f, 0.08f);
        // Unity doesn't provide a filled sphere gizmo; draw many small spheres for a subtle fill
        int steps = 12;
        for (int i = 0; i < steps; i++)
        {
            float t = (float)i / steps;
            float r = mobDistanceRun * (0.5f + 0.5f * t);
            Gizmos.DrawSphere(transform.position, r * 0.02f);
        }
    }
}
