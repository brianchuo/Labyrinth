using Cinemachine.Utility;
using System;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class RollBombController : MonoBehaviour
{

    public float maxSpeed = 10f;
    public float acceleration = 5f;
    public float activationRadius = 10f;
    public float detectionRadius = 15f;
    public float hopForce = 5f;
    public float fuseLength = 10f;
    public bool rollsAway = false;
    public GameObject explosion;

    private GameObject player;
    private SphereCollider detectionArea;
    private Rigidbody rb;
    private RollBombState state = RollBombState.Inactive;
    private float activationDelay = 1f;
    private float wanderDelay;
    private Vector3 wanderDirection;
    private float wanderTime;

    // sounds
    public AudioSource bombAudio;
    public AudioClip awakeSound;
    public AudioClip explosionSound;

    public enum RollBombState {
        Inactive,
        ActivationPhase,
        PursuePlayer,
        Wander,
        Idle
    }

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        rb = GetComponent<Rigidbody>();
        foreach (SphereCollider collider in GetComponents<SphereCollider>()) {
            if (collider.isTrigger) {
                detectionArea = collider;
            }
        }
        detectionArea.radius = activationRadius;
    }

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            if (state != RollBombState.Inactive && state != RollBombState.ActivationPhase) {
                state = RollBombState.PursuePlayer;
            } else if (state == RollBombState.Inactive) {
                bombAudio.PlayOneShot(awakeSound, 1f);
                rb.isKinematic = false;
                rb.AddForce(new Vector3(0, hopForce, 0), ForceMode.Impulse);
                state = RollBombState.ActivationPhase;
                detectionArea.radius = detectionRadius;
            }
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            if (state == RollBombState.PursuePlayer) {
                state = RollBombState.Idle;
                wanderDelay = Random.Range(1f, 5f);
            }
        }
    }

    void OnCollisionEnter(Collision collision) {
        if (!collision.gameObject.CompareTag("Ground")) {
            explode();
        }
    }

    void FixedUpdate() {
        switch (state) {
            case RollBombState.Inactive:
                break;
            case RollBombState.ActivationPhase:
                activationDelay -= Time.deltaTime;
                if (activationDelay < 0) {
                    state = RollBombState.PursuePlayer;
                }
                break;
            case RollBombState.PursuePlayer:
                Vector3 displacement = player.transform.position - transform.position;
                float time = Mathf.Clamp(displacement.magnitude / rb.velocity.magnitude, 0f, 2f);
                Vector3 target = player.transform.position + (player.GetComponent<Rigidbody>().velocity * time);
                rb.AddForce((target - transform.position).normalized * Mathf.Clamp(displacement.magnitude * 2, 0f, acceleration));
                break;
            case RollBombState.Wander:
                wanderTime -= Time.deltaTime;
                if (wanderTime < 0) {
                    state = RollBombState.Idle;
                    wanderDelay = Random.Range(1f, 5f);
                } else {
                    rb.AddForce(wanderDirection * acceleration);
                }
                break;
            case RollBombState.Idle:
                rb.velocity *= 0.9f;
                wanderDelay -= Time.deltaTime;
                if (wanderDelay <= 0) {
                    state = RollBombState.Wander;
                    Vector3 direction = Random.insideUnitSphere;
                    wanderDirection = direction.ProjectOntoPlane(new Vector3(0, 1, 0)).normalized;
                    wanderTime = Random.value * 3;
                }
                break;
        }
        Vector2 speed = new Vector2(rb.velocity.x, rb.velocity.z);
        if (speed.magnitude > maxSpeed) {
            Vector2 newVel = speed.normalized * maxSpeed;
            rb.velocity = new Vector3(newVel.x, rb.velocity.y, newVel.y);
        }
    }

    private void explode() {
        bombAudio.PlayOneShot(explosionSound, 1f);
        Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
