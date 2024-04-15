using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        isPaused = true;
    }
    
    public void Resume()
    {
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void PlayGame()
    {
        SceneManager.LoadSceneAsync(1);
        Time.timeScale = 1;
    }

    public void MainM()
    {
        SceneManager.LoadSceneAsync(0);
        Time.timeScale = 1;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}