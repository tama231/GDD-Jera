using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static bool GameIsOver;
    [Header("References")]
    public GameObject platformPrefab;
    public GameObject temporaryPlatformPrefab;
    public GameObject loseUi;
    [Header("Jumlah Platform")]
    public int platformCount = 100;
    public int temporaryPlatformCount = 500;
    public AudioSource backgroundMusic;    
    void Start()
    {
        loseUi.SetActive(false);
        if (backgroundMusic != null)
        {
            backgroundMusic.loop = true;
            backgroundMusic.Play();
        }

        if (platformPrefab == null || (temporaryPlatformCount > 0 && temporaryPlatformPrefab == null))
        {
            Debug.LogError("Platform prefab dan Temporary Platform prefab harus diisi di Inspector.");
            return;
        }

        Vector3 spawnPosition = new Vector3();

        int regularPlatformsRemaining = platformCount;
        int temporaryPlatformsRemaining = temporaryPlatformCount;
        int totalPlatformCount = platformCount + temporaryPlatformCount;

        for (int i = 0; i < totalPlatformCount; i++)
        {
            spawnPosition.y += Random.Range(2f, 2.5f);
            spawnPosition.x = Random.Range(-5f, 5f);

            bool spawnTemporary = temporaryPlatformsRemaining > 0 &&
                (regularPlatformsRemaining == 0 ||
                Random.Range(0, regularPlatformsRemaining + temporaryPlatformsRemaining) < temporaryPlatformsRemaining);

            if (spawnTemporary)
            {
                Instantiate(temporaryPlatformPrefab, spawnPosition, Quaternion.identity);
                temporaryPlatformsRemaining--;
            }
            else
            {
                Instantiate(platformPrefab, spawnPosition, Quaternion.identity);
                regularPlatformsRemaining--;
            }
        }
    }

    private System.Collections.IEnumerator Restart()
    {
        
        Debug.Log("Button clicked! Waiting 1 seconds...");
        yield return new WaitForSeconds(1f);
        
    }
    private System.Collections.IEnumerator BackToMainMenu()
    {
        
        Debug.Log("Button clicked! Waiting 1 seconds...");
        yield return new WaitForSeconds(1f);
        
    }

    public void RestartLevel() 
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("GamePlay");
        GameIsOver = false;
    }

    public void BackToMainMenuButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public void Die()
    {
        GameIsOver = true;
        loseUi.SetActive(true);
        Time.timeScale = 0f; // Pause the game
        
    }
}

