using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushStuff : MonoBehaviour
{
    Rigidbody rb;
    public float force = 500f;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            rb.AddForce(-Vector3.forward * force);
        }
    }
}
