using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    // private variables
    private Vector3 destinationPosition;
    private Quaternion destinationRotation;
    private GameObject door; // the actual door (what's inside the door frame)
    private Material activeDoorMaterial; // the color of the door when we aren't locked

    // public variables
    public bool isLocked = false; // whether the door will require a key to go through
    public int keyNumber = -1; // the number of the key that unlocks it (leave as -1 if the door isn't locked)
    public string destinationScene;
    public int destinationSpawnPointIndex = -1; // the index of the spawn point to use, if -1, then it uses where the player object is located in the scene editor
    public Material lockedDoorMaterial; // the color of the door when it's locked

    private void Start()
    {
        // save the material we use when it's active
        foreach (Transform child in transform)
        {
            if (child.gameObject.name == "door")
            {
                door = child.gameObject;
                activeDoorMaterial = child.GetComponent<Renderer>().material;
            }
        }

        // change material of door if it's locked
        if (isLocked)
        {
            door.GetComponent<Renderer>().material = lockedDoorMaterial;
            transform.Find("lock").gameObject.SetActive(true);
        } else
        {
            transform.Find("lock").gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        // check to see if we're still locked
        if (isLocked && GameManager.instance.PlayerHasKey(keyNumber))
        {
            isLocked = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("ah!");
        Debug.Log(collision.collider.gameObject.tag);
        // we only care about a player touching the door
        if (collision.collider.gameObject.CompareTag("Player") == false) { return; }
        // alert player if the door is locked
        if (isLocked && !GameManager.instance.PlayerHasKey(keyNumber))
        {
            GameManager.instance.DisplayMessage("This door is locked, try to find a key!");
            return;
        }
        Debug.Log("test");
        // change to new scene
        GameManager.instance.ChangeScenes(destinationScene, destinationSpawnPointIndex);
    }
}
