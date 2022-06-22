using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class TouchSensor : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void updateTouchSensorData(bool touch);

    public bool isTouching;

    void Start()
    {
        isTouching = false;
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player") || other.gameObject.layer == LayerMask.NameToLayer("Ignore Raycast"))
            return;
        //print(other.gameObject);
        isTouching = true;
    }

    void OnTriggerExit(Collider other)
    {
        isTouching = false;
    }

    private void FixedUpdate()
    {
        //reports data to jslib
        DebugUI.instance?.Display($"touch sensed: " + isTouching);
#if UNITY_WEBGL && !UNITY_EDITOR
        //Debug.Log($"touch sensed: " + isTouching);
        try
        {
            updateTouchSensorData(isTouching);
        }
        catch { }
#endif
    }
}
