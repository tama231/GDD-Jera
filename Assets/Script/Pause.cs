using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject loseMenu;
    [SerializeField] private int mainMenuSceneIndex = 0;

    private void Start()
    {
        if (pauseMenu != null)
            pauseMenu.SetActive(false);
        if (loseMenu != null)
            loseMenu.SetActive(false);
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

    public void RestartGame()
    {
        Time.timeScale = 1f;
        GameManager.GameIsOver = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1f;
        GameManager.GameIsOver = false;
        SceneManager.LoadScene(mainMenuSceneIndex);
    }
}
