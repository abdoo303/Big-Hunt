using System.Collections;
using UnityEngine;
using TMPro; // Make sure to add this using statement for TextMeshPro

[RequireComponent(typeof(AudioSource))]
public class Last_NPC : MonoBehaviour
{
    // Array of dialogue lines for this NPC
    public string[] dialogueLines;

    // Array of AudioClips that correspond to each dialogue line (optional; lengths can differ)
    public AudioClip[] dialogueAudioClips;

    // Reference to the UI Text element where dialogue will be displayed
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI press;

    // Reference to the entire Dialogue Panel GameObject
    public GameObject dialoguePanel;

    // Optional Animator to trigger "Die" on the last line
    public Animator animator;

    private AudioSource audioSource;
    private Coroutine playCoroutine;
    private Collider npcCollider;

    private int currentLineIndex = 0;
    private bool playerInRange = true;
    private bool isDead = false;

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

    // Update is called once per frame
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
            // Trigger "Die" animation if an Animator is assigned
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

    private void EndDialogue()
    {
        StopPlayback();
        dialoguePanel.SetActive(false);
        currentLineIndex = 0; // Reset for next interaction
    }

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
