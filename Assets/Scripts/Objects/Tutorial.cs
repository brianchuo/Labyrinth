using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    public GameObject coin;
    public Canvas popup;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // toggles popup
    public void TogglePopup()
    {
        // disable menu if it's active
        CanvasGroup canvasGroup = popup.GetComponent<CanvasGroup>();
        canvasGroup.interactable = !canvasGroup.interactable;
        canvasGroup.blocksRaycasts = !canvasGroup.blocksRaycasts;
        canvasGroup.alpha = canvasGroup.interactable ? 1f : 0f;
        Time.timeScale = canvasGroup.interactable ? 0f : 1f;

        // destroy coin if we haven't already
        if (canvasGroup.interactable)
        {
            Destroy(coin);
            GetComponent<SphereCollider>().enabled = false;
        }
    }
}
