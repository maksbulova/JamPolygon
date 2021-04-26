using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class swordTarget : MonoBehaviour
{
    
    [SerializeField] float WeaponLong;
    [SerializeField] float offSideMoveMarker;
    [SerializeField] GameObject Effects;

    protected Rigidbody Rigidbody;
    protected Camera Camera;
    GameObject Player;
    protected Vector3 standartShift;
    Vector3 StrikePoint;
    

    private void Awake()
    {
        Player = GameObject.FindWithTag("Player");
        standartShift = transform.position-Player.transform.position;
    }
    // Start is called before the first frame update
    void Start()
    {
        
        Rigidbody = GetComponent<Rigidbody>();
        Camera = Camera.main;
        
        
    }

    // Update is called once per frame
    void Update()
    {

        float z1;
        if (Input.GetAxis("Fire2") != 0)
        {
           
            if (Effects != null) Effects.SetActive(true);
            z1 = (Mathf.Sin( Mathf.Max(Mathf.Abs(checkMouseStart().x), Mathf.Abs(checkMouseStart().y)))
                + Mathf.Cos( Mathf.Min(Mathf.Abs(checkMouseStart().x), Mathf.Abs(checkMouseStart().y))))
                ;
                
            StrikePoint = new Vector3(checkMouseStart().x * offSideMoveMarker, checkMouseStart().y * offSideMoveMarker + .6f, z1 ) * WeaponLong ;
            StrikePoint = Player.transform.position + Player.transform.TransformDirection(StrikePoint); 
            Rigidbody.MovePosition(StrikePoint);

        }
        else
        {
            
            if (Effects != null) Effects.SetActive(false);
            z1 = .5f;
            StrikePoint = Player.transform.position + Player.transform.TransformDirection(standartShift);
            Rigidbody.MovePosition(StrikePoint);

        }
    }

    private Vector2 checkMouseStart()
    {
        Vector2 direction;
        float screenPosX = Mathf.Clamp(Input.mousePosition.x, 0, Camera.pixelWidth) / Camera.pixelWidth;
        float screenPosY = Mathf.Clamp(Input.mousePosition.y, 0, Camera.pixelHeight) / Camera.pixelHeight;
        direction = new Vector2(screenPosX - .5f, screenPosY - .5f);
        return direction;
    }
    /* если захочется стену кирпичей
    private void testGraph(Vector3 pos, float LifeTime)
    {
        GameObject q1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        q1.transform.localPosition = pos;
        Destroy(q1.GetComponent<BoxCollider>());
        Destroy(q1, LifeTime);
    }
    */

}
