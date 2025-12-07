using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    public Door door;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Object"))
        {
            door.Open();
            transform.Translate(new Vector3(0, -0.3f, 0));
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Object"))
        {
            door.Close();
            transform.Translate(new Vector3(0, 0.3f, 0));
        }
    }
}
