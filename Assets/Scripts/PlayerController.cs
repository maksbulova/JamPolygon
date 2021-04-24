using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [Header("Параметры игрока")]
    public float moveSpeed;
    public float rotateSpeed;

    private Rigidbody rb;

    private void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody>();

        // чтоб не опрокидывался
        // rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }



    private void FixedUpdate()
    {

        // движение
        float vert = Input.GetAxis("Vertical");
        float hor = Input.GetAxis("Horizontal");
        Vector3 move = new Vector3(hor, 0, vert).normalized * moveSpeed * Time.fixedDeltaTime;
        move = transform.TransformDirection(move);
        rb.MovePosition(transform.position + move);


        // вращение
        if (!Input.GetMouseButton(0) && !Input.GetMouseButton(1))
        {
            float mouseHor = Input.GetAxis("Mouse X") * rotateSpeed * Time.fixedDeltaTime;
            Vector3 rotateAngle = new Vector3(0, rotateSpeed, 0);
            rb.MoveRotation(rb.rotation * Quaternion.Euler(mouseHor * rotateAngle * Time.deltaTime));

        }
    }

}
