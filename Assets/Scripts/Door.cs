using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [Tooltip("Movement speed in units per second.")]
    public float Speed = 5f;

    [Tooltip("How far each door panel moves from its closed position (local Z axis).")]
    public float OpenDistance = 3f;

    public Transform left;
    public Transform right;

    [Header("Audio")]
    [Tooltip("Optional AudioSource. If not assigned and a movingClip is provided, one will be added at runtime.")]
    public AudioSource audioSource;
    [Tooltip("Looping mechanical sound played while the door is moving.")]
    public AudioClip movingClip;

    private Vector3 leftClosedLocalPos;
    private Vector3 rightClosedLocalPos;
    private Vector3 leftOpenLocalPos;
    private Vector3 rightOpenLocalPos;

    private Coroutine animationCoroutine;
    private bool isOpen;

    private const float kEpsilon = 0.001f;

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

    public void Open()
    {
        StartAnimation(true);
    }

    public void Close()
    {
        StartAnimation(false);
    }

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

    // Optional helpers
    public bool IsOpen()
    {
        return isOpen;
    }

    public void Toggle()
    {
        StartAnimation(!isOpen);
    }
}
