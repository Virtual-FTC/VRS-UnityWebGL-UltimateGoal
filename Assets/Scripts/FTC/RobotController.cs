﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.InputSystem;
using Photon.Pun;


public class RobotController : MonoBehaviour
{

    PlayerControls controls;
    private float linearVelocityX;
    private float linearVelocityY;
    private float angularVelocity;

    public bool usingJoystick = true;

    private float frontLeftWheelCmd = 0f;
    private float frontRightWheelCmd = 0f;
    private float backLeftWheelCmd = 0f;
    private float backRightWheelCmd = 0f;

    private float motorPower5;
    private float motorPower6;
    private float motorPower7;
    private float motorPower8;

    public float drivetrainGearRatio = 20f;
    public float encoderTicksPerRev = 28f;
    public float wheelSeparationWidth = 0.4f;
    public float wheelSeparationLength = 0.4f;
    public float wheelRadius = 0.0508f;
    public float motorRPM = 340.0f;

    private Rigidbody rb;

    private Transform posiiton;

    private float previousRealTime;

    [Header("Subsystem Controls")]
    public GameObject shooter;
    public GameObject intake;
    public GameObject grabber;

    public PhotonView PV;
    private ShooterControl shooterControl;
    private IntakeControl intakeControl;
    private GrabberControl grabberControl;

    private AudioManager audioManager;
    private RobotSoundControl robotSoundControl;

    private void Awake()
    {
        intakeControl = intake.GetComponent<IntakeControl>();
        controls = new PlayerControls();

        // Shooting
        controls.GamePlay.Shoot.performed += ctx => motorPower6 = 1.0f;
        controls.GamePlay.Shoot.canceled += ctx => motorPower6 = 0.0f;

        // Spinup
        controls.GamePlay.Spinup.performed += ctx => motorPower7 = ctx.ReadValue<float>();
        controls.GamePlay.Spinup.canceled += ctx => motorPower7 = 0.0f;

        //Intake
        controls.GamePlay.Intake.performed += ctx => motorPower5 = 1f;
        controls.GamePlay.Intake.canceled += ctx => motorPower5 = 0.0f;

        //Wobble
        //controls.GamePlay.Wobble.performed += ctx => motorPower8 = 0.3f;
        //controls.GamePlay.Wobble.canceled += ctx => motorPower8 = 0.0f;
        //controls.GamePlay.WobbleHigh.performed += ctx => motorPower8 = 1f;
        //controls.GamePlay.WobbleHigh.canceled += ctx => motorPower8 = 0.3f;

        //Driving Controls
        controls.GamePlay.DriveForward.started += ctx => usingJoystick = true;
        controls.GamePlay.DriveForward.performed += ctx => linearVelocityX = -1.5f*ctx.ReadValue<float>();
        controls.GamePlay.DriveForward.canceled += ctx => linearVelocityX = 0f;

        controls.GamePlay.DriveBack.started += ctx => usingJoystick = true;
        controls.GamePlay.DriveBack.performed += ctx => linearVelocityX = 1.5f*ctx.ReadValue<float>();
        controls.GamePlay.DriveBack.canceled += ctx => linearVelocityX = 0f;

        controls.GamePlay.DriveLeft.started += ctx => usingJoystick = true;
        controls.GamePlay.DriveLeft.performed += ctx => linearVelocityY = 1.5f*ctx.ReadValue<float>();
        controls.GamePlay.DriveLeft.canceled += ctx => linearVelocityY = 0f;

        controls.GamePlay.DriveRight.started += ctx => usingJoystick = true;
        controls.GamePlay.DriveRight.performed += ctx => linearVelocityY = -1.5f*ctx.ReadValue<float>();
        controls.GamePlay.DriveRight.canceled += ctx => linearVelocityY = 0f;

        controls.GamePlay.TurnLeft.started += ctx => usingJoystick = true;
        controls.GamePlay.TurnLeft.performed += ctx => angularVelocity = 6*ctx.ReadValue<float>();
        controls.GamePlay.TurnLeft.canceled += ctx => angularVelocity = 0f;

        controls.GamePlay.TurnRight.started += ctx => usingJoystick = true;
        controls.GamePlay.TurnRight.performed += ctx => angularVelocity = -6*ctx.ReadValue<float>();
        controls.GamePlay.TurnRight.canceled += ctx => angularVelocity = 0f;
    }

    private void OnEnable()
    {
        controls.GamePlay.Enable();
    }

    private void OnDisable()
    {
        controls.GamePlay.Disable();
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        robotSoundControl = GetComponent<RobotSoundControl>();

        audioManager = GameObject.Find("ScoreKeeper").GetComponent<AudioManager>();

        previousRealTime = Time.realtimeSinceStartup;
        Console.WriteLine("Started.....");

        shooterControl = shooter.GetComponent<ShooterControl>();
        shooterControl.Commands.Add(() => motorPower6 > 0, shooterControl.shooting);
        shooterControl.Commands.Add(() => motorPower7 >= 0, () =>
        {
            robotSoundControl.playShooterRev(motorPower7);
            shooterControl.setVelocity(motorPower7);
        });

        intakeControl.Commands.Add(() => motorPower5 != 0, () =>
        {
            robotSoundControl.playIntakeRev(motorPower5);
            intakeControl.setVelocity(motorPower5 * 150);
            intakeControl.deployIntake();
        });
        intakeControl.Commands.Add(() => motorPower5 == 0, () =>
        {
            robotSoundControl.playIntakeRev(motorPower5);
            intakeControl.retractIntake();
        });

        grabberControl = grabber.GetComponent<GrabberControl>();
        grabberControl.Commands.Add(() => motorPower8 > 0 && motorPower8 < 0.5, () =>
       {
           grabberControl.startGrab();
       });
        grabberControl.Commands.Add(() => motorPower8 > 0.5, () =>
        {
            grabberControl.lift();
        });
        grabberControl.Commands.Add(() => motorPower8 == 0, () =>
        {
            grabberControl.stopGrab();
        });
    }

    private void OnDestroy()
    {

    }

    private void driveRobot()
    {
        // Strafer Drivetrain Control
        if (!usingJoystick)
        {
            linearVelocityX = ((frontLeftWheelCmd + frontRightWheelCmd + backLeftWheelCmd + backRightWheelCmd) / 4) * ((motorRPM / 60) * 2 * wheelRadius * Mathf.PI);
            linearVelocityY = ((-frontLeftWheelCmd + frontRightWheelCmd + backLeftWheelCmd - backRightWheelCmd) / 4) * ((motorRPM / 60) * 2 * wheelRadius * Mathf.PI);
            angularVelocity = (((-frontLeftWheelCmd + frontRightWheelCmd - backLeftWheelCmd + backRightWheelCmd) / 3) * ((motorRPM / 60) * 2 * wheelRadius * Mathf.PI) / (Mathf.PI * wheelSeparationWidth)) * 2 * Mathf.PI;
        }
        // Apply Local Velocity to Rigid Body        
        var locVel = transform.InverseTransformDirection(rb.velocity);
        locVel.x = -linearVelocityY;
        locVel.y = -linearVelocityX;
        locVel.z = 0f;
        rb.velocity = transform.TransformDirection(locVel);
        //Apply Angular Velocity to Rigid Body
        rb.angularVelocity = new Vector3(0f, -angularVelocity, 0f);

        robotSoundControl.playRobotDrive((Mathf.Abs(linearVelocityX) + Mathf.Abs(linearVelocityY) + Mathf.Abs(angularVelocity)) / 4f);
    }

    public void setFrontLeftVel(float x)
    {
        usingJoystick = false;
        frontLeftWheelCmd = x;
    }

    public void setFrontRightVel(float x)
    {
        usingJoystick = false;
        frontRightWheelCmd = x;
    }

    public void setBackLeftVel(float x)
    {
        usingJoystick = false;
        backLeftWheelCmd = x;
    }

    public void setBackRightVel(float x)
    {
        usingJoystick = false;
        backRightWheelCmd = x;
    }

    public void setMotor5(float x)
    {
        usingJoystick = false;
        motorPower5 = x;
    }

    public void setMotor6(float x)
    {
        usingJoystick = false;
        motorPower6 = x;
    }

    public void setMotor7(float x)
    {
        usingJoystick = false;
        motorPower7 = x;
    }

    public void setMotor8(float x)
    {
        usingJoystick = false;
        motorPower8 = x;
    }


    private void FixedUpdate()
    {
        if (!Photon.Pun.PhotonNetwork.IsConnected)
        {
            driveRobot();
            shooterControl.Commands.Process();
            intakeControl.Commands.Process();
            grabberControl.Commands.Process();
        }
        else if (PV.IsMine)
        {
            driveRobot();
            shooterControl.Commands.Process();
            intakeControl.Commands.Process();
            grabberControl.Commands.Process();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Floor")
        {
            robotSoundControl.playRobotImpact();
        }
    }

    public void setStartPosition(Transform t)
    {
        posiiton = t;
    }

    public Transform getStartPosition()
    {
        return posiiton;
    }

    [PunRPC]
    public void subtractBall()
    {
        intakeControl.subtractBall();
    }

    [PunRPC]
    public void addBall()
    {
        intakeControl.addBall();
    }

    [PunRPC]
    public void resetBalls()
    {
        intakeControl.resetBalls();
    }
}
