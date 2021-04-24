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

    private GameObject rotateAxis;

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

    private void Awake()
    {
        rotateAxis = GameObject.Find("RotateAxis");

    }

    private void OnValidate()
    {
        // enterCollider.radius = DoorMaxTrigger;
        enterCollider = transform.Find("DoorTrigger").GetComponent<SphereCollider>();
        

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
        // throw new System.NotImplementedException();

        Debug.Log("Где-то проскрипел рычаг");

        if (CheckButtons())
        {
            StartCoroutine(OpenDoor());
        }
    }


    public void Trigger(bool enter, Collider other)
    {
        if (enter)
        {
            if (other.TryGetComponent<PlayerController>(out PlayerController player))
            {
                visitor = player.gameObject;
                StartCoroutine(OpeningDoor());
            }

        }
        else
        {
            if (other.gameObject == visitor)
            {
                visitor = null;
                StopCoroutine(OpeningDoor());
            }

        }

    }


    private IEnumerator OpenDoor()
    {
        Debug.Log("Дверь что-то делает");

        for (float i = 0; i < 1; i+=0.01f)
        {
            rotateAxis.transform.localEulerAngles = new Vector3(0, Mathf.Lerp(0, -90, i), 0);

            yield return new WaitForFixedUpdate();
        }
    }

    private IEnumerator OpeningDoor()
    {
        float dist, l, doorAngle;
        while (visitor != null)
        {
            dist = Vector3.Distance(transform.position, visitor.transform.position);

            l = Mathf.InverseLerp(DoorMaxTrigger, DoorMinTrigger, dist);
            doorAngle = Mathf.Lerp(0, -90, l);

            rotateAxis.transform.localEulerAngles = new Vector3(0, doorAngle, 0);

            yield return new WaitForFixedUpdate();
        }
    }

}
