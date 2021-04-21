using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum ControllType
    {
        racing,
        fps,
        tps
    }

    [Header("Параметры игрока")]
    public ControllType controllType;
    public float moveSpeed, rotateSpeed;

    [Header("Параметры управления"), Space]
    public bool mouseVertical;
    public bool moveWorldAxis;

    private void Awake()
    {
        gameObject.GetComponent<Renderer>().material.color = Color.red;
    }

    private void FixedUpdate()
    {
        switch (controllType)
        {
            case ControllType.racing:
                RaceControll();
                break;
            case ControllType.fps:
                FpsControll();
                break;
            case ControllType.tps:
                TpsControll();
                break;
            default:
                break;
        }
    }

    private void RaceControll()
    {
        float vert = Input.GetAxis("Vertical") * moveSpeed * Time.fixedDeltaTime;
        float hor = Input.GetAxis("Horizontal") * rotateSpeed * Time.fixedDeltaTime;

        transform.Translate(vert, 0, 0);
        transform.Rotate(0, hor, 0);
    }

    private void FpsControll()
    {
        float vert = Input.GetAxis("Vertical") * moveSpeed * Time.fixedDeltaTime;
        float hor = - Input.GetAxis("Horizontal") * moveSpeed * Time.fixedDeltaTime;

        float mouseVert = 0;
        if (mouseVertical)
        {
            mouseVert = Input.GetAxis("Mouse Y") * rotateSpeed * Time.fixedDeltaTime;
        }
        float mouseHor = Input.GetAxis("Mouse X") * rotateSpeed * Time.fixedDeltaTime;

        Space space = moveWorldAxis ? Space.World : Space.Self;

        transform.Translate(vert, 0, hor, space);
        transform.Rotate(0, mouseHor, mouseVert);
    }

    private void TpsControll()
    {
        float vert = Input.GetAxis("Vertical") * moveSpeed * Time.fixedDeltaTime;
        float hor = Input.GetAxis("Horizontal") * moveSpeed * Time.fixedDeltaTime;

        Space space = moveWorldAxis ? Space.World : Space.Self;

        transform.Translate(hor, 0, vert, space);

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, hitInfo: out RaycastHit hit, maxDistance: 100);

        Vector3 dir = (hit.point - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(dir);
        // transform.LookAt(hit.point);

        Debug.DrawRay(transform.position, hit.point, Color.red);
    }
}
