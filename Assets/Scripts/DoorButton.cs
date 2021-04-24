using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DoorButton : MonoBehaviour
{
    private GameObject visitor;
    public bool pressed;

    public event EventHandler OnPress;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out PlayerController player))
        {
            visitor = player.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == visitor)
        {
            visitor = null;
        }
    }


    private void Update()
    {
        if (Input.GetKeyDown("e") && visitor != null)
        {
            Debug.Log("рычаг со скрипом поддался");
            pressed = !pressed;
            OnPress(this, EventArgs.Empty);
        }
    }
}
