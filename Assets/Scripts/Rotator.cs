using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    // variables
    Vector3 startingPosition;
    
    private void Start()
    {
        startingPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // spin
        //transform.Rotate(new Vector3(0, 60, 0) * Time.deltaTime);
        transform.Rotate(gameObject.transform.up * Time.deltaTime * 60);
        // bob up and down
        transform.position = startingPosition + new Vector3(0f, Mathf.Sin(2f * Time.time) * 0.2f, 0f);
    }
}
