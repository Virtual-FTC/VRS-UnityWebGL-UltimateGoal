using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMove : MonoBehaviour
{
    Rigidbody rigidbody;
    public float forceCoefficient;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            rigidbody.AddForce(1 * forceCoefficient, 0, 0);
        }
        if (Input.GetKey(KeyCode.S))
        {
            rigidbody.AddForce(-1 * forceCoefficient, 0, 0);
        }
        if (Input.GetKey(KeyCode.A))
        {
            rigidbody.AddForce(0, 0, 1 * forceCoefficient);
        }
        if (Input.GetKey(KeyCode.D))
        {
            rigidbody.AddForce(0, 0, -1 * forceCoefficient);
        }
    }
}
