using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject PausePanel;
    private bool isPaused = false;

    // Update is called once per frame
    void Update()
    {
        // Check if the Escape key is pressed
        //if (Input.GetKeyDown(KeyCode.Escape))
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (isPaused)
            {
                Continue(); // If the game is paused, continue
            }
            else
            {
                Pause(); // If the game is running, pause
            }
        }
    }

    public void Pause()
    {
        PausePanel.SetActive(true);
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        isPaused = true;
    }

    public void Continue()
    {
        PausePanel.SetActive(false);
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        isPaused = false;
    }
}
