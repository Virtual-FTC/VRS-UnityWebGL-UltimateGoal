using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class DistanceSensor : MonoBehaviour//, ISensor
{
    [DllImport("__Internal")]
    private static extern void updateDistanceSensorData(float r, float distance);

    Ray rayToSenseDistance;
    RaycastHit hit;

    [Tooltip("value in centimeters")]
    public float rayLength;

    private float distanceSensed = -1;
    private float convertedRayLength;
    private static float fieldScaleFactor = 200f;//field appears to be at half scale (value should be 100f at full scale)

    // Start is called before the first frame update
    void Start()
    {
        rayToSenseDistance = new Ray(transform.position, transform.forward);
        convertedRayLength = rayLength / fieldScaleFactor;
    }

    // Update is called once per frame
    void Update()
    {
        SetRayOriginAndDirection();
        DetectObjectInFront();
    }

    private void SetRayOriginAndDirection()
    {
        rayToSenseDistance.origin = transform.position;
        rayToSenseDistance.direction = transform.forward;

    }

    private void DetectObjectInFront()
    {
        if (Physics.Raycast(rayToSenseDistance, out hit, convertedRayLength))
        { 
            distanceSensed = hit.distance * fieldScaleFactor;
            print(hit.transform + "distance sensed: " + distanceSensed);
        }
        else
            distanceSensed = -1;
    }


    private void OnDrawGizmos()
    {
        Debug.DrawRay(rayToSenseDistance.origin, rayToSenseDistance.direction * convertedRayLength, Color.red);
    }

    private void FixedUpdate()
    {
        //reports data to jslib
        DebugUI.instance?.Display($"distance sensed: " + distanceSensed);
#if UNITY_WEBGL && !UNITY_EDITOR
        //Debug.Log($"distance sensed: " + distanceSensed);
        try
        {
            updateDistanceSensorData(distanceSensed, convertedRayLength);
        }
        catch { }
#endif
    }
}
