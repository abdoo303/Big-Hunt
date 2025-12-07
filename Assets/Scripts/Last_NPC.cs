using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// Manages the final NPC dialogue system with synchronized audio playback and animations.
/// Similar to NPC_Dialogue but triggers an "Idle" animation instead of "Die" on the last line.
/// Typically used for the last NPC encounter in the game.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class Last_NPC : MonoBehaviour
{
    /// <summary>
    /// Array of dialogue lines that this NPC will speak.
    /// Each line is displayed in sequence when the player interacts.
    /// </summary>
    public string[] dialogueLines;

    /// <summary>
    /// Array of audio clips corresponding to each dialogue line.
    /// Lengths can differ from dialogueLines. If no clip exists for a line, uses fallback timing.
    /// </summary>
    public AudioClip[] dialogueAudioClips;

    /// <summary>
    /// TextMeshPro UI element where the current dialogue line is displayed.
    /// </summary>
    public TextMeshProUGUI dialogueText;

    /// <summary>
    /// TextMeshPro UI element for the interaction prompt (e.g., "Press E").
    /// Disabled when dialogue panel is active.
    /// </summary>
    public TextMeshProUGUI press;

    /// <summary>
    /// The dialogue panel GameObject that contains the dialogue UI elements.
    /// Activated when dialogue starts and deactivated when it ends.
    /// </summary>
    public GameObject dialoguePanel;

    /// <summary>
    /// Animator component for triggering the "Idle" animation on the last dialogue line.
    /// Optional - will be auto-assigned if not set in inspector.
    /// </summary>
    public Animator animator;

    /// <summary>
    /// AudioSource component used to play dialogue audio clips.
    /// </summary>
    private AudioSource audioSource;

    /// <summary>
    /// Active coroutine playing the current dialogue line.
    /// Used to track and stop ongoing playback.
    /// </summary>
    private Coroutine playCoroutine;

    /// <summary>
    /// The NPC's collider component, disabled after the final dialogue line.
    /// </summary>
    private Collider npcCollider;

    /// <summary>
    /// Index of the currently active dialogue line.
    /// </summary>
    private int currentLineIndex = 0;

    /// <summary>
    /// Flag indicating whether the player is within interaction range.
    /// Always true in this implementation.
    /// </summary>
    private bool playerInRange = true;

    /// <summary>
    /// Flag indicating whether the NPC has completed all dialogue.
    /// Set to true after the last line, preventing further interactions.
    /// </summary>
    private bool isDead = false;

    /// <summary>
    /// Initializes the NPC dialogue system.
    /// Sets up the AudioSource, Animator, and Collider components.
    /// </summary>
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;

        // Auto-assign optional components if not set in inspector
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        npcCollider = GetComponent<BoxCollider>();
        if (npcCollider == null)
        {
            // Try children as a fallback
            npcCollider = GetComponentInChildren<BoxCollider>();
        }
    }

    /// <summary>
    /// Handles player input for dialogue interaction.
    /// Pressing 'E' starts dialogue, skips lines, or advances to the next line.
    /// </summary>
    void Update()
    {
        // Check for player input (e.g., pressing "E") to start/continue/skip dialogue
        if (playerInRange && Input.GetKeyDown(KeyCode.E) && !isDead)
        {
            if (!dialoguePanel.activeSelf)
            {
                // Start dialogue
                dialoguePanel.SetActive(true);
                press.enabled = false;
                currentLineIndex = 0;
                DisplayNextLine();
            }
            else
            {
                // If a line is currently playing, skip it and go to the next immediately.
                // If nothing is playing, start the next line (if any).
                if (playCoroutine != null)
                {
                    StopCoroutine(playCoroutine);
                    audioSource.Stop();
                    playCoroutine = null;

                    currentLineIndex++;
                    if (currentLineIndex < dialogueLines.Length)
                    {
                        DisplayNextLine();
                    }
                    else
                    {
                        EndDialogue();
                    }
                }
                else
                {
                    // Nothing active: attempt to display/play next line (this covers manual step-forward)
                    DisplayNextLine();
                }
            }
        }
    }

    /// <summary>
    /// Displays the next dialogue line if available, otherwise ends the dialogue.
    /// Stops any currently playing line before starting the next.
    /// </summary>
    private void DisplayNextLine()
    {
        if (currentLineIndex < dialogueLines.Length)
        {
            // Start playing the current line and automatically advance after its audio (or fallback duration) finishes.
            if (playCoroutine != null)
            {
                StopCoroutine(playCoroutine);
                audioSource.Stop();
                playCoroutine = null;
            }

            playCoroutine = StartCoroutine(PlayLineCoroutine(currentLineIndex));
        }
        else
        {
            // End of dialogue
            EndDialogue();
        }
    }

    /// <summary>
    /// Coroutine that plays a single dialogue line with synchronized audio.
    /// Waits for the audio to finish (or uses fallback timing) before auto-advancing.
    /// On the last line, triggers the "Idle" animation and disables the collider.
    /// </summary>
    /// <param name="index">Index of the dialogue line to play.</param>
    /// <returns>IEnumerator for coroutine execution.</returns>
    private IEnumerator PlayLineCoroutine(int index)
    {
        dialogueText.text = dialogueLines[index];

        AudioClip clip = (dialogueAudioClips != null && index < dialogueAudioClips.Length) ? dialogueAudioClips[index] : null;

        if (clip != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
            // Wait for clip to finish (account for pitch not being zero)
            float effectivePitch = Mathf.Approximately(audioSource.pitch, 0f) ? 1f : Mathf.Abs(audioSource.pitch);
            yield return new WaitForSeconds(clip.length / effectivePitch);
        }
        else
        {
            // Fallback duration when there's no clip: simple estimate based on text length
            float fallbackDuration = Mathf.Max(0.8f, dialogueLines[index].Length * 0.04f);
            yield return new WaitForSeconds(fallbackDuration);
        }

        // After sound (or fallback) finishes, handle the last-line animation/collider disabling,
        // then advance index and auto-play the next line if any.
        playCoroutine = null;

        bool wasLastLine = index == dialogueLines.Length - 1;
        if (wasLastLine)
        {
            // Trigger "Idle" animation if an Animator is assigned (different from NPC_Dialogue)
            if (animator != null)
            {
                animator.SetTrigger("Idle");
            }
            isDead = true;

            // Disable this NPC's collider to prevent further triggers/interactions
            if (npcCollider != null)
            {
                npcCollider.enabled = false;
            }

            // End dialogue after triggering death animation
            EndDialogue();
            yield break;
        }

        currentLineIndex++;
        if (currentLineIndex < dialogueLines.Length)
        {
            playCoroutine = StartCoroutine(PlayLineCoroutine(currentLineIndex));
        }
        else
        {
            EndDialogue();
        }
    }

    /// <summary>
    /// Ends the dialogue by stopping playback, hiding the dialogue panel, and resetting the line index.
    /// </summary>
    private void EndDialogue()
    {
        StopPlayback();
        dialoguePanel.SetActive(false);
        currentLineIndex = 0; // Reset for next interaction
    }

    /// <summary>
    /// Stops any active dialogue playback coroutine and audio.
    /// </summary>
    private void StopPlayback()
    {
        if (playCoroutine != null)
        {
            StopCoroutine(playCoroutine);
            playCoroutine = null;
        }

        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
}
