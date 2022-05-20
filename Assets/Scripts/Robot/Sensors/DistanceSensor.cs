using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceSensor : MonoBehaviour//, ISensor
{
    Ray rayToSenseDistance;
    RaycastHit hit;
    Collider detectedCollider;

    public float rayLength;

    private float distanceSensed = -1;

    // Start is called before the first frame update
    void Start()
    {
        rayToSenseDistance = new Ray(transform.position, transform.forward);
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
        if (Physics.Raycast(rayToSenseDistance, out hit, rayLength))
        {
            distanceSensed = hit.distance;
            print("distance sensed: " + distanceSensed);
        }
        else
            distanceSensed = -1;
    }


    private void OnDrawGizmos()
    {
        Debug.DrawRay(rayToSenseDistance.origin, rayToSenseDistance.direction, Color.red);
    }
}
