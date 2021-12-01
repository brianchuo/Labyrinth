using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{

    public float rotationSpeed = 10;
    public Transform target;
    public Transform player;

    float mouseX;
    float mouseY;

    public Transform wall;
    public float zoom = 2f;

    public float distance;

    public float minDist = .5f;
    public float maxDist = 12f;



    // Start is called before the first frame update
    void Start()
    {

    }

    //private void Awake()
    //{
    //    dollyDir = transform.localPosition.normalized;
    //    distance = transform.localPosition.magnitude;
    //}

    //private void Update()
    //{
    //    Vector3 desiredCameraPos = transform.parent.TransformPoint(dollyDir * maxDist);
    //    RaycastHit hit;

    //    if (Physics.Linecast(transform.position, desiredCameraPos, out hit))
    //    {
    //        distance = Mathf.Clamp(hit.distance, minDist, maxDist);
    //    }
    //    else
    //    {
    //        distance = maxDist;
    //    }
    //    transform.localPosition = Vector3.Lerp(transform.localPosition, dollyDir * distance, (Time.deltaTime * 2));

    //    //Obstructed();
    //}

    // Update is called once per frame
    void FixedUpdate()
    {

        if (Input.GetKey(KeyCode.LeftShift))
        {
            //mouseX += Input.GetAxis("Mouse X") * rotationSpeed;
            //mouseY += Input.GetAxis("Mouse Y") * rotationSpeed;
            //mouseY = Mathf.Clamp(mouseY, -35, 60);

            //target.rotation = Quaternion.Euler(mouseY, mouseX, 0);

            target.transform.Rotate(new Vector3(Input.GetAxis("Mouse Y") * rotationSpeed, -Input.GetAxis("Mouse X") * rotationSpeed, 0));
            mouseX = target.transform.rotation.eulerAngles.x;
            mouseY = target.transform.rotation.eulerAngles.y;
            target.transform.rotation = Quaternion.Euler(mouseX, mouseY, 0);
        }

        transform.LookAt(target);

        Obstructed();
    }

    //void Obstructed()
    //{
    //    RaycastHit hit;
    //    wall.gameObject.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
    //    if (Physics.Raycast(transform.position, target.position - transform.position, out hit, 5.5f))
    //    {
    //        if (hit.collider.gameObject.tag != "Player")
    //        {
    //            wall = hit.transform;
    //            wall.gameObject.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;

    //            //if (Vector3.Distance(wall.position, transform.position) >= 3f && Vector3.Distance(transform.position, target.position) >= 1f)
    //            //{
    //            //    transform.Translate(Vector3.forward * zoom * Time.deltaTime);
    //            //}
    //            //else
    //            //{
    //            //    wall.gameObject.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;

    //            //    if (Vector3.Distance(transform.position, target.position) < 4.5f)
    //            //    {
    //            //        transform.Translate(Vector3.back * zoom * Time.deltaTime);
    //            //    }
    //            //}
    //        }
    //    }
    //}

    void Obstructed()
    {
        distance = Vector3.Distance(transform.position, target.position);
        RaycastHit hit;
        //try
        //{
        //    wall.gameObject.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        //}
        //catch
        //{
        //    wall = target;
        //    wall.gameObject.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        //}
        if (Physics.Raycast(transform.position, target.position - transform.position, out hit, 10.5f))
        {
            Debug.Log("first hit");
            if (hit.collider.gameObject.name != "Player")
            {
                Debug.Log("hit not player");
                wall = hit.transform;
                Debug.DrawLine(transform.position, hit.point, Color.blue, 2f);
                Debug.Log(hit.collider.gameObject.name);

                if (Vector3.Distance(wall.position, transform.position) >= 3f && Vector3.Distance(transform.position, target.position) >= 1f)
                {
                    //wall.gameObject.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
                    transform.Translate(Vector3.forward * zoom * (Time.deltaTime * 4));
                }
            }
        }
        if (Vector3.Distance(transform.position, target.position) < 11f)
        {
            Debug.Log("zoomed in");
            //Vector3 viewed = this.GetComponent<Camera>().WorldToViewportPoint(target.position);
            //if (viewed.x > 0 && viewed.x < 1 && viewed.y > 0 && viewed.y < 1 && viewed.z > 0)
            //{
            Vector3 currPosition = transform.position;
            Quaternion currRotation = transform.rotation;
            Vector3 newPosition = currPosition + (currRotation * new Vector3(0, 0, -zoom));
            //newTransform.Translate(Vector3.back * zoom);

            if (Physics.Raycast(newPosition, target.position - newPosition, out hit, 10.5f))
            {
                if (hit.collider.gameObject.name != "Player")
                {
                    Debug.DrawLine(transform.position, hit.point, Color.magenta, 2f);
                    Debug.Log("still obstructed");
                }
                else
                {
                    Debug.Log("zoom out");
                    transform.Translate(Vector3.back * zoom * (Time.deltaTime * 3));
                    //transform.position = Vector3.Lerp(transform.position, newPosition, (Time.deltaTime * 2));

                }
            }
            else
            {
                Debug.Log("zoom out");
                transform.Translate(Vector3.back * zoom * (Time.deltaTime * 2));
                //transform.position = Vector3.Lerp(transform.position, newPosition, (Time.deltaTime * 2));

            }
            //}
        }
    }


    //void OnTriggerEnter(Collider collider)
    //{
    //    Debug.Log("collide");
    //    transform.Translate(Vector3.forward * zoom * (Time.deltaTime * 2));
    //}

    //void OnTriggerExit(Collider collider)
    //{
    //    Debug.Log("exit");
    //    //transform.Translate(Vector3.back * zoom * (Time.deltaTime * 2));
    //}
}
