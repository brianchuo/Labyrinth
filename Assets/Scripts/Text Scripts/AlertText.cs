using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlertText : MonoBehaviour
{
    // public variables
    private float messageCreationTime; // time at which the message was displayed
    private float messageDisplayTime = 3f; // how long a message is displayed for

    // private variables
    private Text text;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        HideTextIfNecessary();
    }

    public void DisplayMessage(string message)
    {
        // show the message
        text.text = message;
        text.gameObject.SetActive(true);
        // start timer to hide it
        messageCreationTime = Time.time;
    }

    void HideTextIfNecessary()
    {
        // hide after 3 seconds, and only do this if it's currently displaying something
        if ((Time.time > (messageCreationTime + messageDisplayTime)) && text.gameObject.activeInHierarchy)
        {
            text.gameObject.SetActive(false);
        }
    }
}
