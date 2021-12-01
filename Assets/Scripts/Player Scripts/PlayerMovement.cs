using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using Cinemachine;

public class PlayerMovement : MonoBehaviour
{
    public float acceleration;
    public float maxSpeed;
    public float jumpSpeed = 7.5f;
    public float jumpLength;
    public float fallSpeed;
    public float rotationSpeed = 4f;
    public float jumpBuffer = 0.2f; // how long after a player stops being grounded that they can jump

    private float lastGroundedTime = 0f;

    private Rigidbody rb;
    private new Collider collider;
    //private float movementX;
    //private float movementZ;
    private bool isJumping;
    private InputActionAsset input;
    private Vector3 movement;
    //private float rotation;

    public GameObject cam;
    public Transform target;
    //private GameObject top;
    //private GameObject bottom;

    private Transform cameraMainTransform;

    Vector2 movementVector;

    public AudioSource playerAudio;
    public AudioClip[] jumpingAudioArray;

    Animator animator;


    public CharacterController controller;
    public float speed = 6f;
    public float rotationSmoothing = 0.1f;
    public float turnSmoothVelocity;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
        cameraMainTransform = cam.transform;

        animator = GetComponentInChildren<Animator>();
    }

    private void Awake()
    {
        input = GetComponent<PlayerInput>().actions;
        if (input == null)
        {
            Debug.Log("Null");
        }
        InputAction jump = input.FindAction("Player/Jump");
        jump.started += OnJumpStart;
        jump.performed += OnJumpFinish;
        jump.canceled += OnJumpFinish;
    }

    // TEMPORARY CHEAT METHODS
    void OnSceneForward()
    {
        switch (GameManager.instance.currentRoom.roomNumber)
        {
            case 0:
                GameManager.instance.ChangeScenes("Room 1", 0);
                GameManager.instance.PickUpKey(new Key(0));
                break;
            case 1:
                GameManager.instance.ChangeScenes("Room 2", 0);
                break;
            case 2:
                GameManager.instance.ChangeScenes("Room 3", 0);
                break;
            case 3:
                GameManager.instance.ChangeScenes("Room 4", 0);
                break;
            case 4:
                GameManager.instance.ChangeScenes("Room 5", 0);
                GameManager.instance.PickUpKey(new Key(1));
                GameManager.instance.spinnerEnabled = true;
                break;
            case 5:
                GameManager.instance.ChangeScenes("Win Screen", 0);
                break;
        }
    }

    void OnSceneBackward()
    {
        switch (GameManager.instance.currentRoom.roomNumber)
        {
            case 0:
                break;
            case 1:
                GameManager.instance.ChangeScenes("Starting Room", 0);
                break;
            case 2:
                GameManager.instance.ChangeScenes("Room 1", 0);
                break;
            case 3:
                GameManager.instance.ChangeScenes("Room 2", 0);
                break;
            case 4:
                GameManager.instance.ChangeScenes("Room 3", 0);
                break;
            case 5:
                GameManager.instance.ChangeScenes("Room 4", 0);
                break;
        }
    }

    private void Update()
    {
        // handle jumping
        if (IsGrounded())
        {
            if (isJumping && Time.time - lastGroundedTime > 0.1f)
            {
                isJumping = false;
            }
            lastGroundedTime = Time.time;
        }
    }

    void OnMove(InputValue movementValue)
    {

        Vector2 movementVector = movementValue.Get<Vector2>();
        movement = new Vector3(movementVector.x, 0, movementVector.y).normalized;
        //movementX = movementVector.x;
        //movementZ = movementVector.y;
        //movement = new Vector3(movementVector.x, 0, movementVector.y);

        //movementVector = movementValue.Get<Vector2>();
        //movement = movementVector.y;
        //rotation = movementVector.x;
    }

    void OnJumpStart(InputAction.CallbackContext callback)
    {
        // quick bugfix in case the rigidbody doesn't exist
        if (rb == null) { return; }

        Vector3 axis = GameManager.instance.gravityIsReversed ? Vector3.down : Vector3.up;
        bool isGrounded = IsGrounded(); // I do this so I don't need to call it twice
        if (isGrounded || (Time.time - lastGroundedTime <= jumpBuffer && !isJumping))
        {
            // make the jump happen
            rb.AddForce(axis * jumpSpeed, ForceMode.Impulse);
            isJumping = true;
            // play audio
            AudioClip jumpSFX = jumpingAudioArray[Random.Range(0, jumpingAudioArray.Length)];
            playerAudio.PlayOneShot(jumpSFX, 1f);
        }
    }

    void OnJumpFinish(InputAction.CallbackContext callback)
    {

    }

    void FixedUpdate()
    {
        if (movement.magnitude >= 0.1)
        {

            float targetAngle = Mathf.Atan2(movement.x, movement.z) * Mathf.Rad2Deg + cameraMainTransform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, rotationSmoothing);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            Vector3 direction = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            movement.y = 0;
            rb.AddForce(direction.normalized * acceleration);
            if (rb.velocity.magnitude > maxSpeed)
            {
                Vector2 velocity = new Vector2(rb.velocity.x, rb.velocity.z);
                velocity = velocity.normalized * maxSpeed;
                rb.velocity = new Vector3(velocity.x, rb.velocity.y, velocity.y);
            }
            else
            {
                Vector3 velocity = rb.velocity;
                velocity.x *= .9f;
                velocity.z *= .9f;
                rb.velocity = velocity;
            }
        }

        //float orientation, movementX, movementZ;
        //Vector3 axis;
        //if (GameManager.instance.gravityIsReversed)
        //{
        //    orientation = 180f;
        //    if (movement.z < 0)
        //    {
        //        movementZ = -movement.z;
        //    }
        //    else
        //    {
        //        movementZ = movement.z;
        //    }
        //    movementX = movement.x;
        //    axis = Vector3.up;
        //}
        //else
        //{
        //    orientation = 0f;
        //    if (movement.z < 0) {
        //        movementZ = -movement.z;
        //    } else {
        //        movementZ = movement.z;
        //    }
        //    movementX = movement.x;
        //    axis = Vector3.down;
        //}

        //Vector3 playerMovement;

        //if (cameraMainTransform.rotation.y >= 0 && cameraMainTransform.rotation.y <= 90)
        //{
        //    playerMovement = cameraMainTransform.forward * movement.z + cameraMainTransform.right * movement.x * (movement.z < 0 ? -1 : 1);
        //    playerMovement.y = 0;
        //    Debug.Log("forward");
        //}
        //else if (cameraMainTransform.rotation.y > 90 && cameraMainTransform.rotation.y <= 180)
        //{
        //    playerMovement = cameraMainTransform.right * movement.z + cameraMainTransform.forward * movement.x * (movement.z < 0 ? -1 : 1);
        //    playerMovement.y = 0;
        //    Debug.Log("left");
        //} else if (cameraMainTransform.rotation.y < 0 && cameraMainTransform.rotation.y > -90)
        //{
        //    playerMovement = -cameraMainTransform.right * movement.z + -cameraMainTransform.forward * movement.x * (movement.z < 0 ? -1 : 1);
        //    playerMovement.y = 0;
        //    Debug.Log("right");

        //} else
        //{
        //    playerMovement = -cameraMainTransform.forward * movement.z + -cameraMainTransform.right * movement.x * (movement.z < 0 ? -1 : 1);
        //    playerMovement.y = 0;
        //    Debug.Log("back");
        //}


        //Vector3 playerMovement = cameraMainTransform.forward * movement.z + cameraMainTransform.right * movement.x * (movement.z < 0 ? -1 : 1);
        //playerMovement.y = 0;
        //Vector3 playerMovement = transform.forward * movement.z + transform.right * movement.x * (movement.z < 0 ? -1 : 1);
        ////Vector3 playerMovement = transform.forward * movement.z + transform.right * movement.x;
        //playerMovement.y = 0;
        //rb.AddForce(playerMovement * acceleration);
        //if (rb.velocity.magnitude > maxSpeed) {
        //    Vector2 velocity = new Vector2(rb.velocity.x, rb.velocity.z);
        //    velocity = velocity.normalized * maxSpeed;
        //    rb.velocity = new Vector3(velocity.x, rb.velocity.y, velocity.y);
        //}

        //if (movement != Vector3.zero) {
        //    float targetAngle = Mathf.Atan2(movementX, movementZ) * Mathf.Rad2Deg + transform.eulerAngles.y;
        //    Quaternion rotation = Quaternion.Euler(orientation, targetAngle, 0f);
        //    transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);
        //} else {
        //    Vector3 velocity = rb.velocity;
        //    velocity.x *= .9f;
        //    velocity.z *= .9f;
        //    rb.velocity = velocity;
        //}

        if (Vector3.Dot(rb.velocity, Vector3.down) > 0)
        {
            rb.AddForce(Vector3.down * fallSpeed);
        }

        // account for jump buffer
        if (Time.time - lastGroundedTime <= jumpBuffer && !isJumping)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        }

        //Vector3 movement = new Vector3(movementX, 0, movementY);

        //movement = cameraMainTransform.forward * movement.z + cameraMainTransform.right * movement.x;
        //movement.y = 0;


        //rb.MovePosition(rb.position + movement * Time.deltaTime * maxSpeed);
        //rb.AddForce(movement * acceleration);


        //if (movementVector != Vector2.zero)
        //{
        //    if (GravityReverse.gravityReversed)
        //    {
        //        Debug.Log("movement gravity");

        //        if (movementY < 0)
        //        {
        //            float targetAngle = Mathf.Atan2(-movementX, movementY) * Mathf.Rad2Deg + cameraMainTransform.eulerAngles.y;
        //            Quaternion rotation = Quaternion.Euler(180f, targetAngle, 0f);
        //            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);
        //        }

        //        else
        //        {
        //            float targetAngle = Mathf.Atan2(-movementX, -movementY) * Mathf.Rad2Deg + cameraMainTransform.eulerAngles.y;
        //            Quaternion rotation = Quaternion.Euler(180f, targetAngle, 0f);
        //            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);
        //        }
        //    }
        //    else
        //    {
        //        if (movementY < 0)
        //        {
        //            float targetAngle = Mathf.Atan2(movementX, -movementY) * Mathf.Rad2Deg + cameraMainTransform.eulerAngles.y;
        //            Quaternion rotation = Quaternion.Euler(0f, targetAngle, 0f);
        //            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);
        //        }

        //        else
        //        {
        //            float targetAngle = Mathf.Atan2(movementX, movementY) * Mathf.Rad2Deg + cameraMainTransform.eulerAngles.y;
        //            Quaternion rotation = Quaternion.Euler(0f, targetAngle, 0f);
        //            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);
        //        }
        //    }

        //}

        //Vector3 movement = new Vector3(movementX, 0, movementY);


        //Vector3 playerMovement = transform.forward * movement;

        //Quaternion playerRotation = Quaternion.Euler(0, rotation * rotationSpeed, 0);

        //rb.MoveRotation(transform.rotation * playerRotation);

        //Debug.Log(transform.InverseTransformDirection(rb.velocity));
        Vector3 localVelocity = transform.InverseTransformDirection(rb.velocity);
        animator.SetFloat("Magnitude", localVelocity[2] / 10f);
        animator.SetFloat("Heading", localVelocity[0] / 5f);
    }

    void OnTriggerEnter(Collider other)
    {
        // check if we are picking up an ability
        if (other.gameObject.CompareTag("Gravity Pickup"))
        {
            // Enable gravity reversal
            GameManager.instance.EnableGravityMechanic();
            Destroy(other.gameObject);
            return;
        }

        // check if we are picking up a key
        if (other.gameObject.CompareTag("Key"))
        {
            Key key = other.gameObject.GetComponent<Key>();
            GameManager.instance.PickUpKey(key);
            Destroy(other.gameObject);
        }

        // check if we are picking up a tutorial
        if (other.gameObject.CompareTag("Tutorial"))
        {
            other.gameObject.GetComponent<Tutorial>().TogglePopup();
        }
    }

    //void OnTriggerExit(Collider other)
    //{
    //    
    //}

    //void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.collider.CompareTag("Ground") || collision.collider.tag.Contains("Ground"))
    //    {
    //        isJumping = false;
    //    }

    //}

    bool IsGrounded()
    {
        // get my variables
        float xBound = collider.bounds.extents.x;
        float yBound = collider.bounds.extents.y;
        float zBound = collider.bounds.extents.z;
        Vector3 up = GameManager.instance.gravityIsReversed ? Vector3.up : -Vector3.up;
        // get the corners
        Vector3 botLeftCorner = new Vector3(transform.position.x - xBound + 0.1f, transform.position.y, transform.position.z - zBound + 0.1f);
        Vector3 topLeftCorner = new Vector3(transform.position.x - xBound + 0.1f, transform.position.y, transform.position.z + zBound - 0.1f);
        Vector3 topRightCorner = new Vector3(transform.position.x + xBound - 0.1f, transform.position.y, transform.position.z + zBound - 0.1f);
        Vector3 botRightCorner = new Vector3(transform.position.x + xBound - 0.1f, transform.position.y, transform.position.z - zBound + 0.1f);
        // check all 4 corners (ignoring triggers)
        // bottom left
        foreach (RaycastHit hit in Physics.RaycastAll(botLeftCorner, up, yBound + 0.1f))
        {
            if (hit.collider.isTrigger == false)
            {
                return true;
            }
        }
        // top left
        foreach (RaycastHit hit in Physics.RaycastAll(topLeftCorner, up, yBound + 0.1f))
        {
            if (hit.collider.isTrigger == false)
            {
                return true;
            }
        }
        // top right
        foreach (RaycastHit hit in Physics.RaycastAll(topRightCorner, up, yBound + 0.1f))
        {
            if (hit.collider.isTrigger == false)
            {
                return true;
            }
        }
        // bottom right
        foreach (RaycastHit hit in Physics.RaycastAll(botRightCorner, up, yBound + 0.1f))
        {
            if (hit.collider.isTrigger == false)
            {
                return true;
            }
        }
        return false;
        //return Physics.Raycast(botLeftCorner, up, yBound + 0.1f) ||
        //    Physics.Raycast(topLeftCorner, up, yBound + 0.1f) ||
        //    Physics.Raycast(topRightCorner, up, yBound + 0.1f) ||
        //    Physics.Raycast(botRightCorner, up, yBound + 0.1f);
    }
}
