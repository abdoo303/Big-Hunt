using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : MonoBehaviour
{
    public Door door;
    bool toggled = false;
    bool playerInRange = false;
    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (toggled)
            {
                transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, 50);
                toggled = false;
                door.Toggle();
            }
            else
            {
                transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, 140);
                toggled = true;
                door.Toggle();
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
