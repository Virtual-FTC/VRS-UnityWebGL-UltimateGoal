using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class IMUSensor : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void updateIMUSensorData(float x,float y, float z, float angularX, float angularY, float angularZ,float positionX,float positionY,float positionZ);

    private Rigidbody rb;

    private Vector3 lastVelocity = Vector3.zero;

    void Awake()
    {
        rb = transform.GetComponentInParent<Rigidbody>();
        Debug.Log(rb.gameObject.name);
    }
    void FixedUpdate()
    {
        Vector3 acceleration = (rb.velocity - lastVelocity) / Time.fixedDeltaTime;
        
        Vector3 angularVelocity = rb.angularVelocity;

        Vector3 pos = rb.position;
        #if UNITY_EDITOR
        DebugUI.instance?.Display($"Acceleration x:{acceleration.x} y:{acceleration.y} z:{acceleration.z}  \n Angular Velocity x:{angularVelocity.x}, y:{angularVelocity.y}, z:{angularVelocity.z} ");
        #endif
        #if UNITY_WEBGL && !UNITY_EDITOR
        //Debug.Log($"distance sensed: " + distanceSensed);
        try
        {
            updateIMUSensorData(acceleration.x,acceleration.y,acceleration.z,angularVelocity.x,angularVelocity.y,angularVelocity.z,pos.x,pos.y,pos.z);
        }
        catch { }
        #endif
    }

}
