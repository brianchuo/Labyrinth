using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameReset : MonoBehaviour
{
    public string roomName;
    public void ResetLevel()
    {
        //SceneManager.LoadScene(roomName);
        GameManager.instance.ReloadScene(false);
    }
}
