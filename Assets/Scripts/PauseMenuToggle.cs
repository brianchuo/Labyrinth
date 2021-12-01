using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class PauseMenuToggle : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    private GameObject mainCanvas;
    private GameObject controlsCanvas;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        mainCanvas = transform.Find("main_panel").gameObject;
        controlsCanvas = transform.Find("controls_panel").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (canvasGroup.interactable)
            {
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
                canvasGroup.alpha = 0f;
                Time.timeScale = 1f;
                showMain();
            }
            else
            {
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
                canvasGroup.alpha = 1f;
                Time.timeScale = 0f;
            }
        }
    }

    public void showControls()
    {
        mainCanvas.SetActive(false);
        controlsCanvas.SetActive(true);
    }

    public void showMain()
    {
        mainCanvas.SetActive(true);
        controlsCanvas.SetActive(false);
    }
}
