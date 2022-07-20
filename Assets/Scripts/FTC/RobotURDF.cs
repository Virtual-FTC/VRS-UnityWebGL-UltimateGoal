using MeshProcess;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class RobotURDF : MonoBehaviour
{
    public Text motorTemplateUI;

    GameObject base_robot;
    Dictionary<string, GameObject> comps = new Dictionary<string, GameObject>();

    private void Start()
    {
        //Creates base link gameobject
        base_robot = new GameObject();
        base_robot.name = "BASE_ROBOT";
    }


    private void FixedUpdate()
    {
#if UNITY_EDITOR

        frontLeftWheelCmd = 0f;
        frontRightWheelCmd = 0f;
        backLeftWheelCmd = 0f;
        backRightWheelCmd = 0f;

        if (Input.GetKey(KeyCode.W)) //forward
        {
            frontLeftWheelCmd = 1f;
            frontRightWheelCmd = 1f;
            backLeftWheelCmd = 1f;
            backRightWheelCmd = 1f;
        }
        if (Input.GetKey(KeyCode.S)) //backward
        {
            frontLeftWheelCmd = -1f;
            frontRightWheelCmd = -1f;
            backLeftWheelCmd = -1f;
            backRightWheelCmd = -1f;
        }
        if (Input.GetKey(KeyCode.D)) //right
        {
            frontLeftWheelCmd = 1f;
            frontRightWheelCmd = -1f;
            backLeftWheelCmd = -1f;
            backRightWheelCmd = 1f;
        }
        if (Input.GetKey(KeyCode.A)) //left
        {
            frontLeftWheelCmd = -1f;
            frontRightWheelCmd = 1f;
            backLeftWheelCmd = 1f;
            backRightWheelCmd = -1f;
        }
        if (Input.GetKey(KeyCode.E)) //left
        {
            frontLeftWheelCmd = 1f;
            frontRightWheelCmd = -1f;
            backLeftWheelCmd = 1f;
            backRightWheelCmd = -1f;
        }

        if (Input.GetKey(KeyCode.Q)) //left
        {
            frontLeftWheelCmd = -1f;
            frontRightWheelCmd = 1f;
            backLeftWheelCmd = -1f;
            backRightWheelCmd = 1f;
        }
#endif
        driveRobot();
        /*
        return;
        if(rb == null)
        {
            return;
        }
        Vector3 direction = Vector3.zero; //set's current direction to none

        //Adds vectors in case that user is pressing the button

        //Normalizez direction (put's it's magnitude to 1) so object moves at same speeds in all directions
        rb.AddForce(direction.normalized * 100); //Adds direction with magnitude of "speed" to rigidbody*/
    }

#if UNITY_EDITOR
    //Call this function to start importing process (Can be button powered)
    public void importModel()
    {
        StartCoroutine(temp());
    }

    IEnumerator temp()
    {
        string directory = EditorUtility.OpenFolderPanel("Select Robot Folder", "", "");
        string[] files = Directory.GetFiles(directory);

        string fileInfo = "";
        for (int i = 1; i < files.Length; i++)
        {
            fileInfo = File.ReadAllText(files[i]);
            if (files[i].EndsWith(".stl"))
            {
                byte[] AsBytes = File.ReadAllBytes(files[i]);
                fileInfo = System.Convert.ToBase64String(AsBytes);
            }

            receiveRobotFile("{\"name\":\"" + files[i].Split('\\')[1] + "\", \"data\":\"" + fileInfo.Substring(0, Mathf.Min(fileInfo.Length, 1000)) + "\"}");
            receiveRobotFile("{\"name\":\"" + files[i].Split('\\')[1] + "\", \"data\":\"" + fileInfo.Substring(Mathf.Min(fileInfo.Length, 1000)) + "\"}");
            yield return null;
        }
        fileInfo = File.ReadAllText(files[0]);
        receiveRobotFile("{\"name\":\"" + files[0].Split('\\')[1] + "\", \"data\":'" + fileInfo + "'}");
    }
#endif

    public string lastFile = "";
    public string lastData = "";

    //Recieves Files from JAVASCRIPT
    public void receiveRobotFile(string jsonString)
    {
        JToken fileInfo = JToken.Parse(jsonString);
        Time.timeScale = 0;

        if (lastFile != "" && lastFile != fileInfo["name"].Value<string>())
        {
            readSTL(lastFile, lastData);
            lastData = "";
        }

        if (fileInfo["name"].Value<string>().EndsWith(".urdf"))
        {
            parseURDF(fileInfo["data"].Value<string>());
            lastFile = "";
            lastData = "";
        }
        else
        {
            lastFile = fileInfo["name"].Value<string>();
            lastData += fileInfo["data"].Value<string>();
        }
    }

    //Parses through all the info in an STL File
    void readSTL(string fileName, string file)
    {
        //New Link Object
        GameObject newObj = new GameObject();
        newObj.name = fileName.Substring(0, fileName.Length - 4);
        newObj.transform.parent = base_robot.transform;

        MeshRenderer rend = newObj.AddComponent<MeshRenderer>();
        rend.material.color = Color.gray;

        //Read STL File Info
        MemoryStream str = new MemoryStream(System.Convert.FromBase64String(file));
        BinaryReader br = new BinaryReader(str);

        //80 - Header
        br.ReadBytes(80); //String (May contain color)

        //4 - Number of Triangles
        uint numOfTriangles = br.ReadUInt32();

        //Creates Meshes (Limit per mesh is 65535 Vertices)
        for (int m = 0; m < (numOfTriangles / 21845) + 1; m++)
        {
            MeshFilter mesh;
            VHACD demesher;
            if (m == 0)
            {
                mesh = newObj.AddComponent<MeshFilter>();
                demesher = newObj.AddComponent<VHACD>();
            }
            else
            {
                //Can't put 2 meshFilters onto one object
                GameObject childMesh = new GameObject();
                childMesh.name = "submesh";
                childMesh.transform.parent = newObj.transform;
                MeshRenderer childRend = childMesh.AddComponent<MeshRenderer>();
                childRend.material.color = Color.gray;
                mesh = childMesh.AddComponent<MeshFilter>();
                demesher = childMesh.AddComponent<VHACD>();
            }

            int verticesToRead = (int)Mathf.Min((numOfTriangles - m * 21845) * 3, 65535);

            //Info needed for meshes
            Vector3[] normals = new Vector3[verticesToRead];
            Vector3[] vertices = new Vector3[verticesToRead];
            int[] triangles = new int[verticesToRead];

            //50 - Triangles
            for (int t = 0; t < verticesToRead / 3; t++)
            {
                //12 - Normal Vector
                for (int i = 0; i < 3; i++)
                    normals[t * 3][i] = br.ReadSingle();
                for (int i = 1; i < 3; i++)
                    normals[t * 3 + i] = normals[t * 3];
                //12 * 3 - Vertices
                for (int v = 0; v < 3; v++)
                    for (int i = 0; i < 3; i++)
                        vertices[t * 3 + v][i] = br.ReadSingle();

                for (int i = 0; i < 3; i++)
                    triangles[t * 3 + i] = t * 3 + i;

                //2 - Attribute
                br.ReadBytes(2); //May contain color
            }

            //Apply STL Mesh Info
            mesh.mesh.vertices = vertices;
            mesh.mesh.triangles = triangles;
            mesh.mesh.normals = normals;

            //Collision System (VHACD)
            if (newObj.name.StartsWith("wheel"))
                continue;
            //newObj.AddComponent<BoxCollider>();
            demesher.m_parameters.m_resolution = 50000;
            demesher.m_parameters.m_maxNumVerticesPerCH = 32;
            demesher.m_parameters.m_maxConvexHulls = 8;
            List<Mesh> convexHulls = demesher.GenerateConvexMeshes();
            foreach (Mesh convexHull in convexHulls)
            {
                MeshCollider collide = newObj.AddComponent<MeshCollider>();
                collide.sharedMesh = convexHull;
                collide.convex = true;
            }
        }

        //Creates Rigidbody and stores component for later
        if (!newObj.name.StartsWith("wheel"))
            newObj.AddComponent<Rigidbody>();

        newObj.layer = 6;
        comps.Add(newObj.name, newObj);
    }

    //Parses through all the info in a URDF File
    void parseURDF(string file)
    {
        Time.timeScale = 1;

        //Resets Scale
        base_robot.transform.rotation = Quaternion.Euler(-90, 0, 0);
        base_robot.transform.localScale = Vector3.one * .01f;

        //Stores Joints
        Dictionary<string, Object> joints = new Dictionary<string, Object>();

        //Parses XML
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(file);
        //xmlDoc.Load(file);

        XmlNode robot = xmlDoc.ChildNodes[1];
        foreach (XmlNode node in robot.ChildNodes)
        {
            //Adds Joints
            if (node.Name == "joint")
            {
                //Parent, Child, Axis, Origin, (Wheel), (Limits)

                //Find Parent and Child GameObjects
                GameObject parent;
                comps.TryGetValue(node.ChildNodes[0].Attributes["link"].Value, out parent);
                if (parent == null)
                {
                    parent = new GameObject();
                    parent.name = node.ChildNodes[0].Attributes["link"].Value;
                    parent.transform.parent = base_robot.transform;
                    parent.AddComponent<Rigidbody>();
                    comps.Add(parent.name, parent);
                }
                GameObject child;
                comps.TryGetValue(node.ChildNodes[1].Attributes["link"].Value, out child);
                if (child == null)
                {
                    child = new GameObject();
                    child.name = node.ChildNodes[1].Attributes["link"].Value;
                    child.transform.parent = base_robot.transform;
                    child.AddComponent<Rigidbody>();
                    comps.Add(child.name, child);
                }


                if (node.ChildNodes.Count < 5 || node.ChildNodes[4].Name != "wheel")
                {
                    //Hinge Joint
                    HingeJoint joint = parent.AddComponent<HingeJoint>();
                    //Origin of Joint
                    string[] origin = node.ChildNodes[3].Attributes["xyz"].Value.Split(' ');
                    joint.anchor = new Vector3(float.Parse(origin[0]), float.Parse(origin[1]), float.Parse(origin[2])) * 10;
                    //Axis of Joint
                    string[] axis = node.ChildNodes[2].Attributes["xyz"].Value.Split(' ');
                    joint.axis = new Vector3(float.Parse(axis[0]), float.Parse(axis[1]), float.Parse(axis[2]));
                    //Child to Joint
                    joint.connectedBody = child.GetComponent<Rigidbody>();

                    joints.Add(node.Attributes["name"].Value, joint);
                }
                else
                {

                    //Wheel Collider Object
                    GameObject wheelObj = new GameObject();
                    wheelObj.AddComponent<Rigidbody>();
                    wheelObj.name = "wheelcollider" + child.name.Substring(5);
                    //Position
                    base_robot.transform.rotation = Quaternion.Euler(0, 0, 0);
                    wheelObj.transform.parent = base_robot.transform;
                    string[] origin = node.ChildNodes[3].Attributes["xyz"].Value.Split(' ');
                    wheelObj.transform.position = new Vector3(float.Parse(origin[0]), float.Parse(origin[1]), float.Parse(origin[2])) * .1f;
                    base_robot.transform.rotation = Quaternion.Euler(-90, 0, 0);
                    //Rotation
                    string[] axis = node.ChildNodes[2].Attributes["xyz"].Value.Split(' ');
                    Vector3 forward = new Vector3(float.Parse(axis[0]), float.Parse(axis[1]), float.Parse(axis[2]));
                    wheelObj.transform.rotation = Quaternion.LookRotation(forward) * Quaternion.Euler(0, 90, 0);
                    //Collider
                    WheelCollider wheel = wheelObj.AddComponent<WheelCollider>();
                    wheel.radius = .5f;
                    wheel.suspensionDistance = 0;
                    wheel.wheelDampingRate = 1;

                    //Type of Wheel
                    switch (node.ChildNodes[4].Attributes["type"].Value)
                    {
                        case "mecanum":
                            WheelFrictionCurve rollers = wheel.forwardFriction;
                            rollers.asymptoteValue = 1;
                            wheel.forwardFriction = rollers;
                            rollers.stiffness = .5f;
                            wheel.sidewaysFriction = rollers;
                            if (wheelObj.transform.position.x * wheelObj.transform.position.z > 0)
                                wheelObj.transform.rotation *= Quaternion.Euler(0, -30, 0);
                            else
                                wheelObj.transform.rotation *= Quaternion.Euler(0, 30, 0);
                            break;
                    }

                    //Connects Parent and Child to Wheel
                    child.transform.parent = wheelObj.transform;
                    FixedJoint connect = wheelObj.AddComponent<FixedJoint>();
                    connect.connectedBody = parent.GetComponent<Rigidbody>();

                    wheelObj.layer = 7;
                    joints.Add(node.Attributes["name"].Value, wheel);
                }
            }
            //Adds Motors
            else if (node.Name == "motor")
            {
                //Gathers Joints
                List<HingeJoint> revJoints = new List<HingeJoint>();
                List<WheelCollider> wheelJoints = new List<WheelCollider>();
                XmlAttributeCollection motorAttr = node.ChildNodes[0].Attributes;
                bool mecanum = false;
                if (node.ChildNodes[0].Name == "powered")
                {
                    string[] jointNames = node.ChildNodes[0].Attributes["joints"].Value.Split(' ');
                    foreach (string jointName in jointNames)
                    {
                        Object newJoint = joints[jointName];
                        if (newJoint.GetType() == typeof(HingeJoint))
                        {
                            ((HingeJoint)newJoint).useMotor = true;
                            revJoints.Add((HingeJoint)newJoint);
                        }
                        else
                        {
                            //wheelJoints.Add((WheelCollider)newJoint);
                        }
                    }
                    motorAttr = node.ChildNodes[1].Attributes;
                }
                //Creates new Motor Controller
                MotorController motor = base_robot.AddComponent<MotorController>();
                motor.newMotorController(node.Attributes["name"].Value, mecanum, revJoints.ToArray(), wheelJoints.ToArray(), motorAttr);
                //Creates UI Interaction
                /*Text newMotorUI = Instantiate(motorTemplateUI);
                newMotorUI.transform.SetParent(motorTemplateUI.transform.parent);
                newMotorUI.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 30 - 30 * newMotorUI.transform.parent.childCount);
                newMotorUI.text = node.Attributes["name"].Value;
                newMotorUI.transform.GetChild(0).GetComponent<InputField>().onEndEdit.AddListener(delegate {
                    motor.setPower(newMotorUI.transform.GetChild(0).GetComponent<InputField>().text);
                });
                newMotorUI.gameObject.SetActive(true);*/
            }

            //setFrontLeftVel(1f);
            rb = base_robot.transform.GetChild(0).GetComponent<Rigidbody>();
            rb.angularDrag = 30;
            rb.useGravity = true;
        }
    }

    public void setFrontLeftVel(float power)
    {
        frontLeftWheelCmd = power;
    }

    public void setFrontRightVel(float power)
    {
        frontRightWheelCmd = power;
    }

    public void setBackLeftVel(float power)
    {
        backLeftWheelCmd = power;
    }

    public void setBackRightVel(float power)
    {
        backRightWheelCmd = power;
    }

    private float frontLeftWheelCmd = 1f;
    private float frontRightWheelCmd = 1f;
    private float backLeftWheelCmd = 1f;
    private float backRightWheelCmd = 1f;

    private float frontLeftWheelEnc = 0f;
    private float frontRightWheelEnc = 0f;
    private float backLeftWheelEnc = 0f;
    private float backRightWheelEnc = 0f;

    public float drivetrainGearRatio = 20f;
    public float encoderTicksPerRev = 28f;
    public float wheelSeparationWidth = 0.4f;
    public float wheelSeparationLength = 0.4f;
    public float wheelRadius = 0.0508f;
    public float motorRPM = 340.0f;

    private Rigidbody rb;


    private void driveRobot()
    {
        if (rb == null)
        {
            return;
        }
        // Strafer Drivetrain Control
        var linearVelocityX = ((frontLeftWheelCmd + frontRightWheelCmd + backLeftWheelCmd + backRightWheelCmd) / 1.5f) * ((motorRPM / 60) * 2 * wheelRadius * Mathf.PI);
        var linearVelocityY = ((-frontLeftWheelCmd + frontRightWheelCmd + backLeftWheelCmd - backRightWheelCmd) / 1.5f) * ((motorRPM / 60) * 2 * wheelRadius * Mathf.PI);
        var angularVelocity = (((-frontLeftWheelCmd + frontRightWheelCmd - backLeftWheelCmd + backRightWheelCmd) / 3) * ((motorRPM / 60) * 2 * wheelRadius * Mathf.PI) / (Mathf.PI * wheelSeparationWidth)) * 2 * Mathf.PI;
        // Apply Local Velocity to Rigid Body        
        var locVel = transform.InverseTransformDirection(rb.velocity);
        locVel.x = -linearVelocityY;
        locVel.z = linearVelocityX;
        locVel.y = 0f;
        rb.velocity = transform.TransformDirection(locVel);
        //Apply Angular Velocity to Rigid Body
        rb.angularVelocity = new Vector3(0f, -angularVelocity, 0f);

        if (angularVelocity == 0)
        {
            rb.freezeRotation = true;
        }
        else
        {
            rb.freezeRotation = false;
        }


        //Quaternion deltaRotation = Quaternion.Euler(new Vector3(-angularVelocity, -angularVelocity, -angularVelocity) * Time.fixedDeltaTime);
        //rb.rotation = (rb.rotation * deltaRotation);

        //Encoder Calculations 
        frontLeftWheelEnc += (motorRPM / 60) * frontLeftWheelCmd * Time.deltaTime * encoderTicksPerRev * drivetrainGearRatio;
        frontRightWheelEnc += (motorRPM / 60) * frontRightWheelCmd * Time.deltaTime * encoderTicksPerRev * drivetrainGearRatio;
        backLeftWheelEnc += (motorRPM / 60) * backLeftWheelCmd * Time.deltaTime * encoderTicksPerRev * drivetrainGearRatio;
        backRightWheelEnc += (motorRPM / 60) * backRightWheelCmd * Time.deltaTime * encoderTicksPerRev * drivetrainGearRatio;
    }


}
