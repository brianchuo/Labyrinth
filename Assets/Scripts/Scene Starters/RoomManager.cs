using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    // public variables
    public GameObject player;
    public int roomNumber = -1; // room 1 will have number 1, starting room has number 0
    public GameObject[] spawnPoints; // possible locations where the player can spawn in

    private void Awake()
    {
        GameManager.instance.SetupRoom(this, player);
        LoadState();
    }

    public void SaveState()
    {
        // save the room's state
        // get all interactable objects
        Transform interactableObjects = UnityEngine.GameObject.Find("Interactable").transform;
        List<RoomState.ObjectState> objectStates = new List<RoomState.ObjectState>();
        // iterate through and get each object state
        List<Transform> list = new List<Transform>();
        foreach (Transform transform in interactableObjects)
        {
            list.Add(transform);
        }
        // use DFS
        while (list.Count > 0)
        {
            // get the transform
            Transform transform = list[0];
            list.RemoveAt(0);
            // add its children to the list
            foreach (Transform childTransform in transform)
            {
                list.Add(childTransform);
            }
            // add its objectState
            GameObject obj = transform.gameObject;
            RoomState.ObjectState objectState = new RoomState.ObjectState(obj.name, transform.position, transform.rotation, obj.activeSelf);
            objectStates.Add(objectState);
        }
        // create and save the room state object
        GameManager.instance.SaveRoomState(roomNumber, new RoomState(GameManager.instance.gravityIsReversed, objectStates));
    }

    public void LoadState()
    {
        // put the player at the appropriate spawnpoint (unless index is -1)
        if (GameManager.instance.spawnPointIndex != -1)
        {
            GameObject spawnPoint = spawnPoints[GameManager.instance.spawnPointIndex];
            player.transform.position = spawnPoint.transform.position;
            player.transform.rotation = spawnPoint.transform.rotation;
        }

        // get the state from GameManager
        RoomState currentState = GameManager.instance.GetRoomState(roomNumber);

        // if there is no saved state just have the scene load normally (no need for extra steps)
        if (currentState == null) { return; }

        // get all interactable objects in this scene
        Transform interactableObjects = UnityEngine.GameObject.Find("Interactable").transform;
        HashSet<string> objectNames = new HashSet<string>();
        // save all possible names in this set (so we can later see which objects have been destroyed
        List<Transform> list = new List<Transform>();
        foreach (Transform transform in interactableObjects)
        {
            list.Add(transform);
        }
        while (list.Count > 0)
        {
            // get the transform
            Transform transform = list[0];
            list.RemoveAt(0);
            // add its children to the list
            foreach (Transform childTransform in transform)
            {
                list.Add(childTransform);
            }
            // add its objectState
            objectNames.Add(transform.gameObject.name);
        }

        foreach (RoomState.ObjectState objectState in currentState.objectStates)
        {
            // remove this from the list of object names
            Debug.Log(objectState.name);
            objectNames.Remove(objectState.name);
            // apply the properties to the object
            GameObject obj = GameObject.Find(objectState.name);
            obj.transform.position = objectState.position;
            obj.transform.rotation = objectState.rotation;
            obj.SetActive(objectState.isActive);
        }

        // now go through all objects that werent in objectStates (meaning they were destroyed) and destroy them
        foreach (string name in objectNames)
        {
            Destroy(GameObject.Find(name));
        }

        // flip gravity if necessary
        if (currentState.gravityWasReversed && GameManager.instance.gravityIsReversed == false)
        {
            GameManager.instance.ReverseGravity();
        }
    }
}
