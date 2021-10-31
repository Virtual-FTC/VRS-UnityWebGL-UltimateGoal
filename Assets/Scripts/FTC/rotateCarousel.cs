using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotateCarousel : MonoBehaviour
{
    [SerializeField]Vector3 dir;
    public bool hasDuck;
    bool isSpinning;

    // Start is called before the first frame update
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {
        //  transform.Rotate(0f, -50 * Time.deltaTime, 0f, Space.Self);

        if (hasDuck && isSpinning)
        {
            transform.Rotate((dir * Time.deltaTime),Space.Self);
        }
    }

    private void OnTriggerEnter(Collider collision)
    {

        if (collision.tag == "Spinner" && !isSpinning && hasDuck)
        {
            print("Collinbso");
            isSpinning = true;

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
        if(collision.collider.tag == "Duck" && hasDuck)
        {
            hasDuck = false;
            isSpinning = false;
        }
    }

}
