using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [Range(2, 10)]
    public float DoorTriggerRadius;

    private GameObject visitor;


    private void Awake()
    {
        gameObject.GetComponent<SphereCollider>().radius = DoorTriggerRadius;
    }

    private void OnValidate()
    {
        gameObject.GetComponent<SphereCollider>().radius = DoorTriggerRadius;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out PlayerController player))
        {
            visitor = player.gameObject;
            StartCoroutine(DoorOpening());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == visitor)
        {
            StopCoroutine(DoorOpening());
            visitor = null;
        }

    }


    private IEnumerator DoorOpening()
    {
        float dist, l, doorAngle;
        while (true)
        {
            dist = Vector3.Distance(transform.position, visitor.transform.position);

            l = Mathf.InverseLerp(DoorTriggerRadius, DoorTriggerRadius-2, dist);
            doorAngle = Mathf.Lerp(0, -90, l);

            gameObject.transform.localEulerAngles = new Vector3(0, doorAngle, 0);

            yield return new WaitForFixedUpdate();
        }
    }

}
