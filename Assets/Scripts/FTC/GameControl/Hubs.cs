using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hubs : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        transform.gameObject.GetComponent<Rigidbody>().centerOfMass = new Vector3(0.6102353f, -0.1638501f, 0.3120021f);
    }
}
