using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickToPlane : MonoBehaviour
{
    public Material stuckColor;
    // Start is called before the first frame update
    void Start()
    {
        //stuckColor = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("StickPlane") && this.GetComponent<Rigidbody>().useGravity)
        {
            Destroy(this.GetComponent<Rigidbody>());
            this.GetComponent<Renderer>().material = stuckColor;
            this.transform.eulerAngles = new Vector3(0, 0, 0);

        }
    }
}
