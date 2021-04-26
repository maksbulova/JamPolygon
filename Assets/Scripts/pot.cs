using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pot : MonoBehaviour
{
    [SerializeField] private GameObject potBreak;
    [SerializeField] private int HP;
    int _HP;
    // Start is called before the first frame update
    void Start()
    {
        _HP = HP;

    }

    // Update is called once per frame
    void Update()
    {
        if (_HP <= 0) booom();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Damage")
        {
            Rigidbody RB = other.GetComponent<Rigidbody>();
            float hit =RB.velocity.magnitude * RB.mass;
            
           _HP -= (int)hit;
            //Debug.Log("hit = " + hit + "  HP = " + _HP);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.contacts[0].otherCollider.GetComponent < Rigidbody >() !=null)
        {
            Rigidbody RB = collision.contacts[0].otherCollider.GetComponent<Rigidbody>();
            float hit = RB.velocity.magnitude * RB.mass ;
            
            _HP -= (int)hit;
            //Debug.Log("hit = " + hit + "  HP = " + _HP);
        }
    }

    public void booom()
    {
  
        //Debug.Log("booom");
        GameObject pB = Instantiate(potBreak);
        pB.transform.position = transform.position;
        Destroy(this.gameObject);
    }
}
