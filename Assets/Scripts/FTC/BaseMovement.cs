using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMovement : MonoBehaviour
{
    Dictionary<string, MotorController> motorControllers = new Dictionary<string, MotorController>();

    // Start is called before the first frame update
    void Start()
    {
        foreach(MotorController controller in GetComponentsInChildren<MotorController>())
            motorControllers.Add(controller.motorName, controller);
    }

    // Update is called once per frame
    void Update()
    {
        motorControllers["frontLeft"].setPower(-Input.GetAxis("Vertical") + Input.GetAxis("Horizontal") + Input.GetAxis("Rotate"));
        motorControllers["frontRight"].setPower(Input.GetAxis("Vertical") + Input.GetAxis("Horizontal") + Input.GetAxis("Rotate"));
        motorControllers["backLeft"].setPower(-Input.GetAxis("Vertical") - Input.GetAxis("Horizontal") + Input.GetAxis("Rotate"));
        motorControllers["backRight"].setPower(Input.GetAxis("Vertical") - Input.GetAxis("Horizontal") + Input.GetAxis("Rotate"));

        motorControllers["ringShooter"].setPower(Input.GetButton("Fire1") ? 1 : 0);
        motorControllers["ringCollection"].setPower(Input.GetButton("Fire2") ? 1 : 0);
    }
}
