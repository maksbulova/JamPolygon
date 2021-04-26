using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerController : MonoBehaviour
{

    [Header("Параметры игрока")]
    public float moveSpeed;
    public float rotateSpeed;

    protected Camera Camera;
    private Rigidbody rb;

    private void Awake()
    {
        Camera = Camera.main;
        rb = gameObject.GetComponent<Rigidbody>();

        // чтоб не опрокидывался
         rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }



    private void FixedUpdate()
    {
        // Debug.Log($"Vx: {Math.Round(rb.velocity.x, 2)}   Vy: {Math.Round(rb.velocity.y, 2)}");

        if (Mathf.Abs(rb.velocity.y) < 10)
        {
            // движение
            float vert = Input.GetAxis("Vertical");
            float hor = Input.GetAxis("Horizontal");
            Vector3 move = new Vector3(hor, 0, vert).normalized * moveSpeed * Time.fixedDeltaTime;
            move = transform.TransformDirection(move);
            rb.MovePosition(transform.position + move);

        }



        // вращение
        if (!Input.GetMouseButton(0) && !Input.GetMouseButton(1))
        {
            //float mouseHor = Input.GetAxis("Mouse X") * rotateSpeed * Time.fixedDeltaTime;
            Vector3 rotateAngle = new Vector3(0, checkMouse().x, 0);
            rb.MoveRotation(rb.rotation * Quaternion.Euler(rotateSpeed * rotateAngle * Time.deltaTime));

        }
    }
    private Vector2 checkMouse()
    {
        Vector2 direction;
        float screenPosX = Mathf.Clamp(Input.mousePosition.x, 0, Camera.pixelWidth) / Camera.pixelWidth;
        float screenPosY = Mathf.Clamp(Input.mousePosition.y, 0, Camera.pixelHeight) / Camera.pixelHeight;
        direction = new Vector2(screenPosX - .5f, screenPosY - .5f);
        return direction;
    }
}
