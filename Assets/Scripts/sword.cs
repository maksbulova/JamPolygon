using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sword : MonoBehaviour
{
    [SerializeField] public GameObject firePars;
    
    // Start is called before the first frame update
    void Start()
    {

        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player")
        {
            GameObject dh = Instantiate(firePars);
            dh.transform.position = this.transform.position;
            dh.transform.localRotation = this.transform.localRotation;
            Destroy(dh, 1f);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if ((collision.contacts[0].otherCollider.tag == "LevelItem")|| (collision.contacts[0].otherCollider.tag == "Enemy"))
        {
            GameObject dh = Instantiate(firePars);
            dh.transform.position = this.transform.position;
            dh.transform.localRotation = this.transform.localRotation;
            Destroy(dh, 1f);
        }
    }
   
}
