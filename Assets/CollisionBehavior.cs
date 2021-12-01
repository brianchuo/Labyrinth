using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollisionBehavior : MonoBehaviour
{
	// Collider cylinder;
	Canvas popupText;
    // Start is called before the first frame update
    void Start()
    {
        popupText = GetComponentInChildren<Canvas>();
        popupText.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
    	if (other.tag == "Player")
        {
    		popupText.enabled = true;
    	}
    }

    private void OnTriggerExit(Collider other)
    {
    	if (other.tag == "Player")
        {
    		popupText.enabled = false;
    	}
    }
}
