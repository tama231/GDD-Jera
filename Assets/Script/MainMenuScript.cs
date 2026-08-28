using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Panel Referensi")]
    public GameObject mainPanel;      // isi dengan objek "MainMenu" (di dalam Canvas)
    public GameObject settingsPanel;  // isi dengan objek "SettingsPanel"

    void Start()
    {
        mainPanel.SetActive(true);
        settingsPanel.SetActive(false);
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("GamePlay");
    }

    public void OpenSettings()
    {
        mainPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        mainPanel.SetActive(true);
    }
}