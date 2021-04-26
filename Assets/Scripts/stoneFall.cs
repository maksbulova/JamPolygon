using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stoneFall : MonoBehaviour
{
    [SerializeField] float fallPeriod;
    [SerializeField] GameObject[] fallItemPrefabs;
    [SerializeField] float fallItemlifeTime;
    [SerializeField] float maxTorgeItem;

    private float fallZoneRadius;
    private float time;
    // Start is called before the first frame update
    void Start()
    {
        fallZoneRadius = GetComponentInChildren<CapsuleCollider>().bounds.size.x;
        time = Time.time+ fallPeriod;
    }

    // Update is called once per frame
    void Update()
    {
        if (time <= Time.time)
        {
            GameObject fallItem;
            fallItem = Instantiate(fallItemPrefabs[(int)Random.Range(0, fallItemPrefabs.Length)], transform, true);
            float x = Random.Range(0, fallZoneRadius)- fallZoneRadius/2;
            float z = Random.Range(0, 360) ;
            fallItem.transform.localPosition =  new Vector3(x * Mathf.PI * Mathf.Cos(z)/4, 0, x*Mathf.PI *Mathf.Sin(z)/4);
            Debug.Log(x + "    " + z);
            if (fallItem.GetComponent<Rigidbody>() == null) fallItem.AddComponent<Rigidbody>();
            Rigidbody fallItemRB = fallItem.GetComponent<Rigidbody>();
            fallItemRB.mass = 500;
            fallItemRB.useGravity = true;
            fallItemRB.AddTorque(new Vector3(Random.Range(0, maxTorgeItem), 0, Random.Range(0, maxTorgeItem)), ForceMode.VelocityChange);
            Destroy(fallItem.gameObject, fallItemlifeTime);
            time = Time.time + fallPeriod;
        }

    }
}
