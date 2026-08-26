using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Panel Referensi")]
    public GameObject mainPanel;
    public GameObject optionsPanel;

    [Header("Audio")]
    public AudioSource bgMusic;
    public Slider volumeSlider;

    void Start()
    {
        // Pastikan panel awal yang aktif hanya mainPanel
        mainPanel.SetActive(true);
        optionsPanel.SetActive(false);
    }

    // Dipanggil dari tombol "Play"
    public void PlayGame()
    {
        SceneManager.LoadScene("GameScene"); // ganti sesuai nama scene
    }

    // Dipanggil dari tombol "Options"
    public void OpenOptions()
    {
        mainPanel.SetActive(false);
        optionsPanel.SetActive(true);
    }

    // Dipanggil dari tombol "Back" di panel options
    public void BackToMain()
    {
        optionsPanel.SetActive(false);
        mainPanel.SetActive(true);
    }

    // Dipanggil dari slider volume
    public void SetVolume(float value)
    {
        bgMusic.volume = value;
    }

    // Dipanggil dari tombol "Quit"
    public void QuitGame()
    {
        Debug.Log("Keluar game...");
        Application.Quit();
    }
}