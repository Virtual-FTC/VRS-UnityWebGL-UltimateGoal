using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotateCarousel : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField]Vector3 dir;
    public bool hasDuck;
    bool isSpinning;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

    }

    // Update is called once per frame
    void Update()
    {
        //  transform.eulerAngles += new Vector3(0f, -50 * Time.deltaTime, 0f);

        if (hasDuck && isSpinning)
        {
            transform.eulerAngles += new Vector3(0f, -50 * Time.deltaTime, 0f);
        }
    }

    private void OnTriggerEnter(Collider collision)
    {

        if (collision.tag == "Spinner" && !isSpinning && hasDuck)
        {
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
