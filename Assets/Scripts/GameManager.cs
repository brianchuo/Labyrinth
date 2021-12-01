using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // instance
    public static GameManager instance = null;

    // player variables
    public Canvas canvas;
    public Canvas gameMenu;
    private GameObject player;

    // ability variables
    public bool reverseGravityEnabled = false;
    public bool gravityIsReversed = false;
    public bool spinnerEnabled = false;

    // keys
    bool[] keys = { false, false };
    int tempKey = -1; // used to know the player picked up a key but hasn't escaped the room with it yet

    // room states
    private int TOTAL_ROOMS = 9;
    RoomState[] roomStates;
    public RoomManager currentRoom;
    public int spawnPointIndex = -1; // the index of the spawn point to use, if -1, then it uses where the player object is located in the scene editor

    // player stats tracker
    private float timeStarted = 0f;
    private int numDeaths = 0;
    private int numResets = 0;

    private void Awake()
    {
        // make sure the game manager and its canvas exists through the whole project
        DontDestroyOnLoad(this.gameObject);
        DontDestroyOnLoad(canvas);
        DontDestroyOnLoad(gameMenu);
        // create instance (if necessary)
        if (instance == null)
        {
            // create the singleton instance
            instance = this;
        }
        else if (instance != this)
        {
            // destroy this game manager since it isn't the singleton
            Destroy(gameObject);
        }

        // set up variables
        roomStates = new RoomState[TOTAL_ROOMS];
        timeStarted = Time.time;
    }

    /* 
     * =========================================
     *               Room Methods
     * =========================================
     */

    // sets the player reference for the scene
    public void SetupRoom(RoomManager roomManager, GameObject player)
    {
        this.currentRoom = roomManager;
        this.player = player;
    }

    public void SaveRoomState(int roomNumber, RoomState roomState)
    {
        this.roomStates[roomNumber] = roomState;
    }

    public RoomState GetRoomState(int roomNumber)
    {
        return this.roomStates[roomNumber];
    }

    public void ChangeScenes(string newScene, int destinationSpawnPointIndex)
    {
        // save room state
        currentRoom.SaveState();
        // save key if we have one
        if (tempKey != -1)
        {
            keys[tempKey] = true;
            tempKey = -1;
        }
        // change gravity back to normal if it's reversed before we leave this scene
        if (gravityIsReversed)
        {
            ReverseGravity();
        }
        // use save the spawn point index for the next scene
        spawnPointIndex = destinationSpawnPointIndex;
        // actually change the scene
        Debug.Log(newScene);
        SceneManager.LoadScene(newScene);
    }

    public void ReloadScene(bool playerDidDie)
    {
        // if playerDidDie is true, they died, otherwise, it's a reset, so update stats
        if (playerDidDie) { numDeaths++; }
        else { numResets++; }

        // reloads the currently active scene (used for deaths & resets)
        Scene currentScene = SceneManager.GetActiveScene();

        // handle specific scene-reset logic based on room
        switch (currentScene.name)
        {
            case "Starting Room":
                RemoveKey(0);
                break;
            case "Room 4":
                RemoveKey(1);
                break;
        }

        SceneManager.LoadScene(currentScene.name);

        // disable menu if it's active
        CanvasGroup gameMenuCanvasGroup = gameMenu.GetComponent<CanvasGroup>();
        if (gameMenuCanvasGroup.interactable)
        {
            gameMenuCanvasGroup.interactable = false;
            gameMenuCanvasGroup.blocksRaycasts = false;
            gameMenuCanvasGroup.alpha = 0f;
        }
    }

    /* 
     * =========================================
     *               Key Methods
     * =========================================
     */

    // updates the game state to show that the passed in key has been picked up
    public void PickUpKey(Key key)
    {
        tempKey = key.keyNumber;
        DisplayMessage("Key Acquired!");

        // specific key logic
        switch (key.keyNumber) {
            case 1:
                spinnerEnabled = true;
                break;
            default:
                break;
        }
    }

    public void RemoveKey(int keyNumber)
    {
        tempKey = -1;

        // specific key logic
        switch (keyNumber)
        {
            case 1:
                spinnerEnabled = keys[1];
                break;
            default:
                break;
        }
    }

    public bool PlayerHasKey(int keyNumber)
    {
        return (keys[keyNumber] || keyNumber == tempKey);
    }

    /* 
     * =========================================
     *              Gravity Methods
     * =========================================
     */

    public void EnableGravityMechanic()
    {
        reverseGravityEnabled = true;
        DisplayMessage("Gravity Pickup Acquired!");
    }

    public void ReverseGravity()
    {
        // global changes necessary
        Physics.gravity = -Physics.gravity;
        gravityIsReversed = !gravityIsReversed;
        // call the player's gravity reversal method
        player.GetComponent<GravityReverse>().ReverseGravity();
    }

    /* 
     * =========================================
     *              Canvas Methods
     * =========================================
     */

    /* Message Displaying Methods */
    public void DisplayMessage(string message)
    {
        AlertText textMesh = canvas.GetComponentInChildren<AlertText>(true);
        if (textMesh != null)
        {
            textMesh.DisplayMessage(message);
        }
    }

    /*
     * =========================================
     *         Variable Getters / Setters
     * =========================================
     */

    public int getNumDeaths()
    {
        return numDeaths;
    }

    public int getNumResets()
    {
        return numResets;
    }

    public float getTimeElapsed()
    {
        return Time.time - timeStarted;
    }
}
