using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WinScreen : MonoBehaviour
{
    // variables
    public TextMeshProUGUI messageText;
    public TextMeshProUGUI submessageText;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI deathsText;
    public TextMeshProUGUI restartsText;
    public Button menuButton;
    public Button quitButton;
    private float timeAppeared = 0f;
    bool[] fadingStarted = { false, false, false, false, false, false };

    // Start is called before the first frame update
    void Start()
    {
        timeAppeared = Time.time;
        int elapsedTime = (int)GameManager.instance.getTimeElapsed();
        int minutes = elapsedTime / 60;
        int seconds = elapsedTime % 60;
        timeText.text = "Time: " + minutes + ":" + seconds.ToString("00");
        deathsText.text = "Deaths: " + GameManager.instance.getNumDeaths();
        restartsText.text = "Resets: " + GameManager.instance.getNumResets();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - timeAppeared > 0.5 && !fadingStarted[0])
        {
            StartCoroutine(FadeTextToFullAlpha(3f, messageText));
            fadingStarted[0] = true;
        }
        else if (Time.time - timeAppeared > 1.25 && !fadingStarted[1])
        {
            StartCoroutine(FadeTextToFullAlpha(3f, submessageText));
            fadingStarted[1] = true;
        }
        else if (Time.time - timeAppeared > 2.25 && !fadingStarted[2])
        {
            StartCoroutine(FadeTextToFullAlpha(3f, timeText));
            fadingStarted[2] = true;
        }
        else if (Time.time - timeAppeared > 2.75 && !fadingStarted[3])
        {
            StartCoroutine(FadeTextToFullAlpha(3f, deathsText));
            fadingStarted[3] = true;
        }
        else if (Time.time - timeAppeared > 3.25 && !fadingStarted[4])
        {
            StartCoroutine(FadeTextToFullAlpha(3f, restartsText));
            fadingStarted[4] = true;
        }
        else if (Time.time - timeAppeared > 3.75 && !fadingStarted[5])
        {
            StartCoroutine(FadeTextToFullAlpha(3f, menuButton.GetComponentInChildren<TextMeshProUGUI>()));
            StartCoroutine(FadeTextToFullAlpha(3f, quitButton.GetComponentInChildren<TextMeshProUGUI>()));
            fadingStarted[5] = true;
        }
    }

    // allows me to fade in text
    public IEnumerator FadeTextToFullAlpha(float t, TextMeshProUGUI i)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 0);
        while (i.color.a < 1.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a + (Time.deltaTime / t));
            yield return null;
        }
    }

    public void GoToMenu()
    {
        // destroy persisted gameManager objects
        Destroy(GameManager.instance.canvas);
        Destroy(GameManager.instance.gameMenu);
        // destroy gameManager instance
        Destroy(GameManager.instance.gameObject);
        GameManager.instance = null;
        SceneManager.LoadScene("StartMenu");
    }
}
