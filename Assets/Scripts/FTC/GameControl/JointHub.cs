using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JointHub : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.gameObject.GetComponent<Rigidbody>().centerOfMass = new Vector3(-0.009305031f, -0.1457442f, -1.195465f);
    }
}
