using UnityEngine;

public class Pause : MonoBehaviour
{
    public GameObject pauseMenu;

    private void Start()
    {
        if (pauseMenu != null)
            pauseMenu.SetActive(false);
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;

        if (pauseMenu != null)
            pauseMenu.SetActive(true);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;

        if (pauseMenu != null)
            pauseMenu.SetActive(false);
    }
}
