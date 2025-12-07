using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    // Start is called before the first frame update
    Rigidbody rb;
    public float speed = 12f;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = transform.right * x + transform.forward * z;
        rb.velocity = new Vector3(move.x * speed, rb.velocity.y, move.z * speed);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Finish"))
        {
            SceneManager.LoadScene("Level2");
            Cursor.lockState = CursorLockMode.Confined;
        }
        if (other.gameObject.CompareTag("Fall"))
        {
            SceneManager.LoadScene("Lose");
            Cursor.lockState = CursorLockMode.Confined;
        }
    }
}
