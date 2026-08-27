using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Pause : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public static bool isPaused;

    void Start()
    {
        pauseMenuUI.SetActive(false);
        isPaused = false;
    }

    void Update()
    {
        if (Input.GetKeyDown("e"))
        {
            PauseGame();
            if (isPaused == true)
            {
                Resume();
            }
        }
    }
    private System.Collections.IEnumerator Resume()
    {
        
        Debug.Log("Button clicked! Waiting 1 seconds...");
        yield return new WaitForSeconds(1f);
        
    }
    public void ResumeButton()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f; // Resume the game
        isPaused = false;
    }

    public void PauseGame()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f; // Pause the game
        isPaused = true;
    }
}
