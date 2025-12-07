using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class Enemy : MonoBehaviour
{
    NavMeshAgent agent;
    private Animator animator;
    private GameObject player;
    public float mobDistanceRun = 4f;
    public float chaseSpeed = 5f;
    public EnemyAnimation enemyAnimation;
    private AudioSource zombieSound;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.Find("Player");
        animator = GetComponentInChildren<Animator>();
        zombieSound = GetComponent<AudioSource>();
    }

    // Update is called once per frame
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
    
    public void CheckDistanceRun(float distance)
    {
        if(distance < mobDistanceRun)
        {
            animator.enabled = true;
            zombieSound.enabled = true;
        }
    }

    // Draw run radius when the object is selected in the Scene view
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
