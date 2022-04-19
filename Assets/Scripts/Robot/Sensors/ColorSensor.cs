using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorSensor : MonoBehaviour//, ISensor
{
    Ray rayToSenseColor;
    RaycastHit rayCastHitForColoredObject;
    Collider detectedCollider;
    public Color colorSensed;
    public Color colorTarget;

    public LayerMask layerMask;
    public float colorSensingRayLength;
    public float sensorSensitivity;

    bool isColorSensed;

    // Start is called before the first frame update
    void Start()
    {
        rayToSenseColor = new Ray(transform.position, -transform.up);
    }

    // Update is called once per frame
    void Update()
    {
        SetRayOriginAndDirection();
        DetectColorBelow();
    }

    private void SetRayOriginAndDirection()
    {
        rayToSenseColor.origin = transform.position;
        rayToSenseColor.direction = -transform.up;
    }

    private void DetectColorBelow()
    {
        if (Physics.Raycast(rayToSenseColor, out rayCastHitForColoredObject, colorSensingRayLength, layerMask))
        {
            detectedCollider = rayCastHitForColoredObject.collider;
            if (detectedCollider.GetComponent<Renderer>() == null)
                return;
            colorSensed = detectedCollider.GetComponent<Renderer>().material.color;

            isColorSensed = CheckColorDifference();
        }
    }

    private bool CheckColorDifference()
    {
        float colorDifference = Mathf.Sqrt(Mathf.Pow((colorTarget.r - colorSensed.r), 2) + Mathf.Pow((colorTarget.g - colorSensed.g), 2) + Mathf.Pow((colorTarget.b - colorSensed.b), 2));
        // https://en.wikipedia.org/wiki/Color_difference

        if (colorDifference < sensorSensitivity)
        {
            return true;
        } else return false;
    }

    private void OnDrawGizmos()
    {
        Debug.DrawRay(rayToSenseColor.origin, rayToSenseColor.direction);
    }

    public bool IsSensed()
    {
        return isColorSensed;
    }
}
