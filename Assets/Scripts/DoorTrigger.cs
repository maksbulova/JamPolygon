using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    private Door door;

    private void Start()
    {
        door = transform.parent.GetComponent<Door>();
    }

    private void OnTriggerEnter(Collider other)
    {
        door.Trigger(true, other);
    }

    private void OnTriggerExit(Collider other)
    {
        door.Trigger(false, other);

    }
}
