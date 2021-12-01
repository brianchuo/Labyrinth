using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushPull : MonoBehaviour
{
    private bool pushing;
    private bool pulling;
    private Rigidbody pushObject;

    public float pushAmount;
    public float pushRadius;

    public float throwSpeed = 10;
    private bool holding = false;
    public GameObject throwObject;
    public Transform guide;
    public Camera cam;
    public Vector3 hitPoint;

    private Vector3 target;
    private Vector3 difference;
    Vector3 direction;

    Animator playerAnimator;


    // Start is called before the first frame update
    void Start()
    {
        pulling = false;
        playerAnimator = this.gameObject.GetComponentInChildren<Animator> ();
    }

    // Update is called once per frame
    void Update()
    {
        if (holding)
        {
            throwObject.transform.position = guide.position;
            //target = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, transform.position.z));

            //difference = target - throwObject.transform.position;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100.0f))
            {
                target = hit.point;
            }
            direction = target - throwObject.transform.position;
            Debug.DrawLine(throwObject.transform.position, hit.point, Color.magenta, 1f);
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Draggable"))
        {
            pushObject = collision.gameObject.GetComponent<Rigidbody>();
        }

        if (collision.gameObject.tag.Contains("Throwable") && !holding)
        {
            throwObject = collision.gameObject;
        }


    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Draggable"))
        {
            pushObject = null;
        }

        if (collision.gameObject.tag.Contains("Throwable") && !holding)
        {
            throwObject = null;
        }
    }

    void OnPush()
    {
        if (pushObject != null)
        {
            Debug.Log(pushObject);
            //pushObject.AddExplosionForce(pushAmount, Vector3.forward, pushRadius);
            pushObject.AddForce(transform.forward * pushAmount);
            Debug.Log("push");
        }
    }

    void OnPull()
    {
        if (pushObject != null && !pulling)
        {
            Debug.Log(pushObject);
            //pushObject.transform.Translate(Time.deltaTime * pullSpeed * pullDirection);
            this.gameObject.AddComponent<FixedJoint>();
            this.GetComponent<FixedJoint>().connectedBody = pushObject;
            pulling = true;
        } else if (pulling)
        {
            Destroy(this.gameObject.GetComponent<FixedJoint>());
            pulling = false;
        }


    }

    void OnPickup()
    {
        if (!holding)
        {
            playerAnimator.Play("Pickup", 0, 0f);
            throwObject.GetComponent<Rigidbody>().useGravity = false;
            throwObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
            throwObject.transform.localRotation = transform.rotation;
            throwObject.transform.position = guide.position;
            holding = true;
            Debug.Log("pickup");
            
        } else if (holding)
        {
            throwObject.GetComponent<Rigidbody>().useGravity = true;
            //Vector3 direction = cam.ScreenPointToRay(Input.mousePosition);
            //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //RaycastHit hit;
            //if (Physics.Raycast(ray, out hit, 100.0f))
            //{
            //    Vector3 hitPoint = hit.point;
            //}
            //Vector3 direction = hitPoint - throwObject.transform.position;
            //Debug.DrawLine(throwObject.transform.position, hit.point);
            //throwObject.GetComponent<Rigidbody>().velocity = transform.forward * throwSpeed;

            ////throwObject.GetComponent<Rigidbody>().velocity = transform.forward * throwSpeed;
            ///
            //float distance = difference.magnitude;
            //Vector2 direction = difference / distance;
            //direction.Normalize();
            throwObject.transform.LookAt(target);
            //Debug.DrawLine(throwObject.transform.position, target, Color.magenta, 100f);

            //throwObject.GetComponent<Rigidbody>().velocity = throwObject.transform.forward * throwSpeed;
            throwObject.GetComponent<Rigidbody>().AddForce(direction, ForceMode.Impulse);

            throwObject = null;
            holding = false;
            playerAnimator.Play("Throw", 0, 0f);
        }
    }

}
