using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityPad : MonoBehaviour
{
    // public variables
    public Material activeMaterial;
    public Material inactiveMaterial;

    // private variables
    private bool active = false;
    public float timeOfLastUse = -1f; // Used to prevent the player from glitching out when they jump on a gravity reverse pad

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Renderer>().material = inactiveMaterial;
    }

    // Update is called once per frame
    void Update()
    {
        // Only activate one time, the instance gravity becomes enabled
        if (GameManager.instance.reverseGravityEnabled && !active)
        {
            // Activate this button!
            Activate();
        }
    }

    void Activate()
    {
        GetComponent<Renderer>().material = activeMaterial;
        active = true;
    }

    public bool IsActive()
    {
        return active;
    }
}
