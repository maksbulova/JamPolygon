using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class potBreack : MonoBehaviour
{
    public float power;
    public float radius;
    public float upwards;
    public float lifeTime;

    // Start is called before the first frame update
    void Start()
    {
        Rigidbody rb= GetComponent<Rigidbody>();
        rb.AddExplosionForce(power, transform.position, radius, upwards);
        Destroy(this.gameObject, lifeTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
