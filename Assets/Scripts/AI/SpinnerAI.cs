using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]

public class SpinnerAI : MonoBehaviour
{

    // enums
    enum AIStates
    {
        asleep,
        active
    }

    // private variables
    AIStates currentState;
    Animator anim;
    LineRenderer lineRenderer;
    /* Asleep state variables */
    float timePlayerFirstSeen = -1f; // the timestamp that the agent was first able to see the player
    /* Active state variables */
    float timePlayerLastSeen = -1f; // the timestamp that the agent was last able to see the player

    // public variables
    public float activationThreshold = 1f; // how long the agent needs to see the player in order to become active
    public float deactivationThreshold = 1f; // how long the agent needs to lose sight of the player in order to sleep
    public float detectionRadius = 20f;
    public float rotationSpeed = 10f; // how fast the laser rotates
    public GameObject player; // a reference to the player object
    public GameObject firingPoint; // the place where the laser comes out
    public GameObject eye; // a reference to the capsule in the center of the spinner (it's eye)

    bool killed = false;
    Animator playerAnimator;

    public AudioSource spinnerAudio;
    public AudioClip awakeAudio;
    public AudioClip asleepAudio;
    public AudioClip fireAudio;

    // Start is called before the first frame update
    void Start()
    {
        // initialize variables
        currentState = AIStates.asleep;
        anim = GetComponent<Animator>();
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        // only run next code if we are enabled
        if (GameManager.instance.spinnerEnabled == false) { return; }

        switch (currentState)
        {
            case AIStates.asleep:
                checkForActivation();
                break;
            case AIStates.active:
                checkForDeactivation();
                fireLaser();
                break;
        }

        // TO DO: Have something happen when the player is hit with the laser
        if (killed)
        {
            if (playerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 2)
            {
                GameManager.instance.ReloadScene(true);
            }
        }
    }

    // changes the state to the new one
    void changeState(AIStates newState)
    {
        currentState = newState;
        switch (newState)
        {
            case AIStates.asleep:
                anim.SetBool("isAwake", false);
                spinnerAudio.Stop();
                spinnerAudio.PlayOneShot(asleepAudio, 1f);
                clearLaser(); // get rid of the laser since we're going to sleep
                break;
            case AIStates.active:
                anim.SetBool("isAwake", true);
                spinnerAudio.PlayOneShot(awakeAudio, 1f);
                spinnerAudio.PlayDelayed(0.5f);
                timePlayerLastSeen = Time.time;
                break;
        }
    }

    // returns true if the agent can see the player, otherwise returns false
    bool canSeePlayer()
    {
        // set up raycast (I assume player and spinner are on same y position in order to find direction only)
        Vector3 origin = new Vector3(transform.position.x, player.transform.position.y, transform.position.z);
        Vector3 simplifiedPlayerPosition = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z);
        Vector3 direction = (simplifiedPlayerPosition - origin).normalized;

        // conduct raycast
        RaycastHit[] hits = Physics.RaycastAll(origin, direction, detectionRadius);

        // find closest hit
        if (hits.Length == 0) { return false; }
        RaycastHit closestHit = hits[0];
        for (int i = 1; i < hits.Length; i++)
        {
            // only consider things that aren't this spinner
            if (!hits[i].collider.gameObject.CompareTag("Spinner") && !hits[i].collider.gameObject.CompareTag("Bomb"))
            {
                if (closestHit.collider.gameObject.CompareTag("Spinner") || closestHit.distance > hits[i].distance)
                {
                    closestHit = hits[i];
                }
            }

        }
        return closestHit.collider.gameObject.CompareTag("Player");
    }

    /* Asleep State */

    // determines if we need to change to the active state
    void checkForActivation()
    {
        if (canSeePlayer())
        {
            if (timePlayerFirstSeen == -1) // if this is the first frame we've seen the player
            {
                timePlayerFirstSeen = Time.time;
            }
            else if (Time.time - timePlayerFirstSeen > activationThreshold) // see if we've exceeded the threshold
            {
                changeState(AIStates.active);
            }
        }
        else
        {
            // can't see so reset the variables
            timePlayerFirstSeen = -1;
        }
    }

    /* Active State */

    void checkForDeactivation()
    {
        // only fire if we can currently see the player
        if (!canSeePlayer())
        {
            if (timePlayerLastSeen == -1) // if this is the first frame we've lost sight of the player
            {
                timePlayerLastSeen = Time.time;
            }
            else if (Time.time - timePlayerLastSeen > deactivationThreshold) // see if we've exceeded the threshold
            {
                changeState(AIStates.asleep);
            }
        }
        else
        {
            // can't see so reset the variables
            timePlayerLastSeen = -1;
        }
    }

    void fireLaser()
    {
        // find my closest hit, this is where the laser will stop
        RaycastHit[] hits;
        hits = Physics.RaycastAll(firingPoint.transform.position, firingPoint.transform.forward * -1);
        // show the hit
        RaycastHit closest = new RaycastHit();
        for (int i = 0; i < hits.Length; i++)
        {
            if (!hits[i].collider.gameObject.CompareTag("Bomb") && (i == 0 || closest.distance > hits[i].distance))
            {
                closest = hits[i];
            }
        }
        if (hits.Length > 0 && currentState == AIStates.active)
        {
            lineRenderer.SetPosition(0, firingPoint.transform.position);
            lineRenderer.SetPosition(1, closest.point);
        }

        // determine if we killed the player
        if (closest.collider.gameObject.CompareTag("Player") && !killed)
        {
            killed = true;
            playerAnimator = closest.collider.gameObject.GetComponentInChildren<Animator> ();
            playerAnimator.Play("Death", 0, 0f);
        }

        // rotate face towards player (to hit them with the laser)
        Vector3 playerVector = (player.transform.position - transform.position).normalized;
        float angleBetween = Vector3.SignedAngle(firingPoint.transform.forward * -1, playerVector, Vector3.up);
        if (angleBetween < 0)
        {
            // rotate counter-clockwise
            // change in rotation is the current rotation (around y axis) + rotation speed * delta time
            Quaternion rotationChange = Quaternion.Euler(0f, rotationSpeed * Time.deltaTime * -1, 0f);
            eye.transform.rotation = eye.transform.rotation * rotationChange; ;
        }
        else if (angleBetween > 0)
        {
            // rotate clockwise
            Quaternion rotationChange = Quaternion.Euler(0f, rotationSpeed * Time.deltaTime, 0f);
            eye.transform.rotation = eye.transform.rotation * rotationChange; ;
        }
    }

    // stops the laser from showing
    void clearLaser()
    {
        lineRenderer.SetPosition(0, Vector3.zero);
        lineRenderer.SetPosition(1, Vector3.zero);
    }

    // Handles case where the spinner is caught in an explosion (and gets destroyed)
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Explode>())
        {
            GetComponent<Break>().OnExplode();
            Destroy(gameObject);
        }
    }
}
