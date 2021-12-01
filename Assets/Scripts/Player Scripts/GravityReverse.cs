using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;


public class GravityReverse : MonoBehaviour
{
    public GameObject cam;
    public GameObject mainCam;
    public GameObject top;
    public GameObject bottom;

    public GameObject sceneRotator;

    private Quaternion flippedRotation;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnTriggerEnter(Collider other)
    {
        // reverse gravity when we touch an ACTIVE gravity button (gravity pickup must have been collected
        if (other.gameObject.CompareTag("Gravity Button") && GameManager.instance.reverseGravityEnabled)
        {
            // make sure we didn't just use this pad in the last 0.1 seconds, otherwise we're glitching out
            if (Time.time - other.gameObject.GetComponent<GravityPad>().timeOfLastUse < 0.1) { return; }
            // if we reach here we're good to actually reverse gravity
            other.gameObject.GetComponent<GravityPad>().timeOfLastUse = Time.time;
            // This calls ReverseGravity() from this file later
            GameManager.instance.ReverseGravity();
        }
    }

    public void ReverseGravity()
    {
        //transform.Rotate(180, 0, 0, Space.Self);
        //Physics.gravity = -Physics.gravity;
        //gravityReversed = !gravityReversed;
        //sceneRotator.transform.Rotate(180, 0, 0);
        //flippedRotation = this.transform.rotation;
        //flippedRotation.eulerAngles = new Vector3(180, 0, 0);
        //this.transform.rotation = Quaternion.Lerp(transform.rotation, flippedRotation, Time.deltaTime);
        this.transform.Rotate(180, 0, 0);
        cam.transform.Rotate(180, 0, 0);
        if (GameManager.instance.gravityIsReversed)
        {
            Debug.Log("upside down");
            //cam.GetComponent<CinemachineFreeLook>().Follow = bottom.transform;
            //cam.GetComponent<CinemachineFreeLook>().LookAt = transform;

            //mainCam.transform.Rotate(0, 0 * Time.deltaTime, 180);

            cam.GetComponent<CinemachineFreeLook>().m_Orbits[0].m_Height = -0.4f;
            cam.GetComponent<CinemachineFreeLook>().m_Orbits[0].m_Radius = 2.18f;
            cam.GetComponent<CinemachineFreeLook>().m_Orbits[1].m_Height = -2.5f;
            cam.GetComponent<CinemachineFreeLook>().m_Orbits[1].m_Radius = 5.07f;
            cam.GetComponent<CinemachineFreeLook>().m_Orbits[2].m_Height = -4.5f;
            cam.GetComponent<CinemachineFreeLook>().m_Orbits[2].m_Radius = 5.12f;

        }
        else
        {
            Debug.Log("right side up");
            cam.GetComponent<CinemachineFreeLook>().Follow = transform;
            cam.GetComponent<CinemachineFreeLook>().LookAt = top.transform;


            cam.GetComponent<CinemachineFreeLook>().m_Orbits[2].m_Height = 0.4f;
            cam.GetComponent<CinemachineFreeLook>().m_Orbits[2].m_Radius = 2.18f;
            cam.GetComponent<CinemachineFreeLook>().m_Orbits[1].m_Height = 2.5f;
            cam.GetComponent<CinemachineFreeLook>().m_Orbits[1].m_Radius = 5.07f;
            cam.GetComponent<CinemachineFreeLook>().m_Orbits[0].m_Height = 4.5f;
            cam.GetComponent<CinemachineFreeLook>().m_Orbits[0].m_Radius = 5.12f;
        }
    }

}
