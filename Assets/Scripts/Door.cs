using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls a two-panel sliding door with smooth animation and audio feedback.
/// The door panels slide apart along their local Z axis when opening and close back together.
/// Supports optional looping audio that plays during door movement.
/// </summary>
public class Door : MonoBehaviour
{
    /// <summary>
    /// Movement speed of the door panels in units per second.
    /// </summary>
    [Tooltip("Movement speed in units per second.")]
    public float Speed = 5f;

    /// <summary>
    /// Distance each door panel moves from its closed position along the local Z axis.
    /// </summary>
    [Tooltip("How far each door panel moves from its closed position (local Z axis).")]
    public float OpenDistance = 3f;

    /// <summary>
    /// Transform for the left door panel.
    /// </summary>
    public Transform left;

    /// <summary>
    /// Transform for the right door panel.
    /// </summary>
    public Transform right;

    /// <summary>
    /// AudioSource component for playing movement sounds.
    /// Auto-assigned at runtime if not set and a movingClip is provided.
    /// </summary>
    [Header("Audio")]
    [Tooltip("Optional AudioSource. If not assigned and a movingClip is provided, one will be added at runtime.")]
    public AudioSource audioSource;

    /// <summary>
    /// Audio clip that loops while the door is moving.
    /// Provides mechanical sound feedback during door animation.
    /// </summary>
    [Tooltip("Looping mechanical sound played while the door is moving.")]
    public AudioClip movingClip;

    /// <summary>
    /// Cached closed position of the left door panel in local space.
    /// </summary>
    private Vector3 leftClosedLocalPos;

    /// <summary>
    /// Cached closed position of the right door panel in local space.
    /// </summary>
    private Vector3 rightClosedLocalPos;

    /// <summary>
    /// Calculated open position of the left door panel in local space.
    /// </summary>
    private Vector3 leftOpenLocalPos;

    /// <summary>
    /// Calculated open position of the right door panel in local space.
    /// </summary>
    private Vector3 rightOpenLocalPos;

    /// <summary>
    /// Active animation coroutine. Used to stop ongoing animations when needed.
    /// </summary>
    private Coroutine animationCoroutine;

    /// <summary>
    /// Current state of the door (true = open, false = closed).
    /// </summary>
    private bool isOpen;

    /// <summary>
    /// Threshold value for position comparison to determine when door has reached target.
    /// </summary>
    private const float kEpsilon = 0.001f;

    /// <summary>
    /// Initializes the door by caching panel positions and setting up the audio source.
    /// Validates that left and right transforms are assigned.
    /// </summary>
    private void Start()
    {
        if (left == null || right == null)
        {
            Debug.LogError("Door: left and right Transforms must be assigned.");
            enabled = false;
            return;
        }

        // Ensure audio source exists if a moving clip is provided.
        if (movingClip != null && audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
            }
        }

        // Cache local positions so movement is relative to each door's local space.
        leftClosedLocalPos = left.localPosition;
        rightClosedLocalPos = right.localPosition;

        // Open positions move along local Z (keeps behavior consistent regardless of world rotation).
        leftOpenLocalPos = leftClosedLocalPos + new Vector3(0f, 0f, -OpenDistance);
        rightOpenLocalPos = rightClosedLocalPos + new Vector3(0f, 0f, OpenDistance);
    }

    /// <summary>
    /// Opens the door by sliding the panels apart.
    /// </summary>
    public void Open()
    {
        StartAnimation(true);
    }

    /// <summary>
    /// Closes the door by sliding the panels together.
    /// </summary>
    public void Close()
    {
        StartAnimation(false);
    }

    /// <summary>
    /// Initiates the door animation and plays the movement sound.
    /// Stops any currently running animation before starting a new one.
    /// </summary>
    /// <param name="open">True to open the door, false to close it.</param>
    private void StartAnimation(bool open)
    {
        // Stop any existing animation and its moving sound
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
            animationCoroutine = null;
        }

        if (audioSource != null && audioSource.isPlaying && audioSource.clip == movingClip)
        {
            audioSource.Stop();
            audioSource.loop = false;
            audioSource.clip = null;
        }

        // Play looping moving sound if provided
        if (movingClip != null)
        {
            if (audioSource == null)
            {
                audioSource = GetComponent<AudioSource>();
                if (audioSource == null)
                {
                    audioSource = gameObject.AddComponent<AudioSource>();
                    audioSource.playOnAwake = false;
                }
            }

            audioSource.clip = movingClip;
            audioSource.loop = true;
            audioSource.Play();
        }

        animationCoroutine = StartCoroutine(AnimateDoors(open));
    }

    /// <summary>
    /// Coroutine that animates the door panels to their target positions.
    /// Uses smooth linear movement and stops audio when animation completes.
    /// </summary>
    /// <param name="open">True to open the door, false to close it.</param>
    /// <returns>IEnumerator for coroutine execution.</returns>
    private IEnumerator AnimateDoors(bool open)
    {
        isOpen = open;

        Vector3 leftTarget = open ? leftOpenLocalPos : leftClosedLocalPos;
        Vector3 rightTarget = open ? rightOpenLocalPos : rightClosedLocalPos;

        // Move using MoveTowards for a steady mechanical motion.
        while ((left.localPosition - leftTarget).sqrMagnitude > kEpsilon * kEpsilon ||
               (right.localPosition - rightTarget).sqrMagnitude > kEpsilon * kEpsilon)
        {
            left.localPosition = Vector3.MoveTowards(left.localPosition, leftTarget, Speed * Time.deltaTime);
            right.localPosition = Vector3.MoveTowards(right.localPosition, rightTarget, Speed * Time.deltaTime);
            yield return null;
        }

        // Snap to final positions to avoid tiny residual error.
        left.localPosition = leftTarget;
        right.localPosition = rightTarget;

        // Stop moving sound
        if (audioSource != null && audioSource.isPlaying && audioSource.clip == movingClip)
        {
            audioSource.Stop();
            audioSource.loop = false;
            audioSource.clip = null;
        }

        animationCoroutine = null;
    }

    /// <summary>
    /// Returns whether the door is currently open.
    /// </summary>
    /// <returns>True if the door is open, false if closed.</returns>
    public bool IsOpen()
    {
        return isOpen;
    }

    /// <summary>
    /// Toggles the door between open and closed states.
    /// </summary>
    public void Toggle()
    {
        StartAnimation(!isOpen);
    }
}
