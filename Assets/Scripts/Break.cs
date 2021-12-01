using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Break : MonoBehaviour
{
    public GameObject particles;

    public void OnExplode() {
        Instantiate(particles, transform.position, Quaternion.identity);
    }
}
