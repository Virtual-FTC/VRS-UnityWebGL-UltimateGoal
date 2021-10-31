using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotateCarousel : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] Vector3 dir;
    public bool hasDuck;
    bool isSpinning;
    RobotController controller;

    [SerializeField] float baseRotateSpeed = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
    }

    // Update is called once per frame
    void Update()
    {

        if (hasDuck && isSpinning)
        {
            if(controller == null) controller = FindObjectOfType<RobotController>(); 

            float speed = controller.rightTrigger;
            transform.eulerAngles += dir * Time.deltaTime * (baseRotateSpeed + speed);

        }
    }

    private void OnTriggerStay(Collider collision)
    {

        if (collision.tag == "Spinner" && !isSpinning && hasDuck)
        {
            isSpinning = true;

        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.tag == "Spinner")
        {
            isSpinning = false;
        }
    }

    private void OnCollisionStay(Collision collision)
    {

        if (collision.collider.tag == "Duck" && !hasDuck)
        {
            hasDuck = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.tag == "Duck" && hasDuck)
        {
            hasDuck = false;
            isSpinning = false;
        }

    }

}