using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Explode : MonoBehaviour
{

    public float blastRadius = 8;

    void Start() {
        transform.localScale = new Vector3(1, 1, 1) * blastRadius;
    }

    void FixedUpdate()
    {
        transform.localScale *= 0.90f;
        if (Vector3.Magnitude(transform.localScale) < 0.5) {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Breakable")) {
            other.GetComponent<Break>().OnExplode();
            Destroy(other.gameObject);
        } else if (other.CompareTag("Player")) {
            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.name);
        }
    }
}
