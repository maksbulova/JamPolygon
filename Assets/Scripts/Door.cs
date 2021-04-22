using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public enum DoorType
    {
        sensor,
        button
    }

    public DoorType type;

    [Space, Header("Параметры режима двери в супермаркете (подошел - открылась)")]
    public float DoorMaxTrigger;
    public float DoorMinTrigger;

    [Space, Header("Параметры режима рычага или кнопки (нажал - открылась)")]
    public DoorButton[] buttons;

    private GameObject visitor;
    private SphereCollider enterCollider;

    private Vector3 rotateAxis;

    public bool CheckButtons()
    {
        foreach (DoorButton button in buttons)
        {
            if (!button.pressed)
            {
                return false;
            }
        }
        return true;
    }


    private void OnValidate()
    {
        // enterCollider.radius = DoorMaxTrigger;
        enterCollider = gameObject.GetComponent<SphereCollider>();
        rotateAxis = gameObject.transform.Find("OpenAxis2").transform.position - gameObject.transform.Find("OpenAxis1").transform.position;

        switch (type)
        {
            case DoorType.sensor:
                enterCollider.enabled = true;
                break;
            case DoorType.button:
                enterCollider.enabled = false;

                foreach (DoorButton button in buttons)
                {
                    button.OnPress += Button_OnPress;
                }

                break;
            default:
                break;
        }
    }

    private void Button_OnPress(object sender, System.EventArgs e)
    {
        //throw new System.NotImplementedException();

        Debug.Log("Где-то проскрипел рычаг");

        if (CheckButtons())
        {
            // StartCoroutine(OpenDoor());
            OpenDoor();
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out PlayerController player))
        {
            visitor = player.gameObject;
            StartCoroutine(OpeningDoor());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == visitor)
        {
            visitor = null;
            StopCoroutine(OpeningDoor());
        }

    }

    
    private void OpenDoor()
    {
        Debug.Log("Дверь открылась");

        gameObject.transform.localEulerAngles = new Vector3(0, 90, 0);
        /*
        for (float i = 0; i < 90; i+=1)
        {
            // gameObject.transform.localEulerAngles = new Vector3(0, Mathf.Lerp(0, -90, i), 0);
            transform.RotateAround(rotateAxis, 1);

            yield return new WaitForFixedUpdate();
        }
        */
    }

    private IEnumerator OpeningDoor()
    {
        float dist, l, doorAngle;
        while (visitor != null)
        {
            dist = Vector3.Distance(transform.position, visitor.transform.position);

            l = Mathf.InverseLerp(DoorMaxTrigger, DoorMinTrigger, dist);
            doorAngle = Mathf.Lerp(0, -90, l);

            gameObject.transform.localEulerAngles = new Vector3(0, doorAngle, 0);

            yield return new WaitForFixedUpdate();
        }
    }

}
