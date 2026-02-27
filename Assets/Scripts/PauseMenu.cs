using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool isPaused;// Public boolean for controlling whether to stop time and other scripts

    public GameObject pauseMenu;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isPaused = false;
        Unpause();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("escape") || Input.GetKeyDown("p"))
        {
            if (isPaused)
            {
                Unpause();
                Debug.Log(isPaused);
            }
            else if (!isPaused)
            {
                Pause();
                Debug.Log(isPaused);
            }
        }
    }

    // Button functionality
    public void ResumeButton()
    {
        Unpause();
    }
    public void RestartButton()
    {
        SceneManager.LoadScene("SampleScene");
    }
    public void MainMenuButton()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void QuitButton()
    {
        Application.Quit();
    }

    // Pauses the game by stopping by detla time
    private void Pause()
    {
        isPaused = true;
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Unpauses the game
    private void Unpause()
    {
        isPaused = false;
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
