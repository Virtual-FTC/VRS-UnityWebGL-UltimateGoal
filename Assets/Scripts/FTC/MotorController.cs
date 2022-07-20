using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class MotorController : MonoBehaviour
{
    public string motorName = "";

    public float power = 0;
    public float currentPosition = 0;

    bool mecanum = false;
    float gearRatio = 1;
    int maxRPM = 340;
    float ticksPerRev = 560;

    HingeJoint[] revJoints;
    WheelCollider[] wheelJoints;

    float prevEncoderAngle = 0;

    //Create New Motor Controller Object
    public void newMotorController(string motorName, bool mecanum, HingeJoint[] revJoints, WheelCollider[] wheelJoints, XmlAttributeCollection motorAttr)
    {
        this.motorName = motorName;
        this.mecanum = mecanum;
        this.revJoints = revJoints;
        this.wheelJoints = wheelJoints;
        gearRatio = float.Parse(motorAttr["gearRatio"].Value);
        maxRPM = int.Parse(motorAttr["maxRPM"].Value);
        ticksPerRev = float.Parse(motorAttr["encoderTicksPerRev"].Value);

        //Determines Force for Joints
        foreach(HingeJoint revJoint in revJoints)
        {
            JointMotor motor = revJoint.motor;
            motor.force = 100;
            revJoint.motor = motor;
        }
    }

    public void setPower(float power)
    {
        this.power = power;
    }

    public void setPower(string power)
    {
        float newPower = 0;
        float.TryParse(power, out newPower);
        this.power = newPower;
    }

    public int getCurrentPosition()
    {
        return (int)currentPosition;
    }    

    // Update is called once per frame
    void Update()
    {
        //Updates Visually WheelColliders and their Meshes
        for (int i = 0; i < wheelJoints.Length; i++)
        {
            float wheelTurnBy = wheelJoints[i].rpm / 60 * Time.deltaTime * gearRatio;
            Transform childMesh = wheelJoints[i].transform.GetChild(0).transform;
            childMesh.RotateAround(wheelJoints[i].transform.position, -childMesh.right, wheelTurnBy * 360);
            //wheelJoints[i].transform.GetChild(0).rotation *= Quaternion.Euler(Vector3.right * wheelTurnBy * 360);
            if (i == 0)
                currentPosition += wheelTurnBy * ticksPerRev;
        }
        //Updates Ticks
        if (wheelJoints.Length == 0 && revJoints.Length > 0)
        {
            currentPosition += (revJoints[0].angle - prevEncoderAngle) / 360 * ticksPerRev * gearRatio;
            prevEncoderAngle = revJoints[0].angle;
        }
        //Powers Joints
        float targetVel = (float)maxRPM / 360 * 60 * power / gearRatio;
        foreach (HingeJoint joint in revJoints)
        {
            JointMotor motor = joint.motor;
            motor.targetVelocity = 100 * targetVel;
            joint.motor = motor;
        }
        foreach (WheelCollider wheelJoint in wheelJoints)
            wheelJoint.motorTorque = 100 * power;
    }
}
