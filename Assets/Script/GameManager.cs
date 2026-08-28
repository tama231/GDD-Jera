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
    public AudioClip rickrollClip;
    public int lives = 3;
    private float platformSpacingMultiplier = 1f;
    private float platformWidthMultiplier = 1f;
    private int temporaryDurabilityBonus;
    void Start()
    {
        if (loseUi != null)
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
            spawnPosition.y += Random.Range(2f, 2.5f) * platformSpacingMultiplier;
            spawnPosition.x = Random.Range(-5f, 5f);

            bool spawnTemporary = temporaryPlatformsRemaining > 0 &&
                (regularPlatformsRemaining == 0 ||
                Random.Range(0, regularPlatformsRemaining + temporaryPlatformsRemaining) < temporaryPlatformsRemaining);

            if (spawnTemporary)
            {
                GameObject platform = Instantiate(temporaryPlatformPrefab, spawnPosition, Quaternion.identity);
                TemporaryPlatform temporaryPlatform = platform.GetComponent<TemporaryPlatform>();
                if (temporaryPlatform != null)
                    temporaryPlatform.SetDurabilityBonus(temporaryDurabilityBonus);
                temporaryPlatformsRemaining--;
            }
            else
            {
                GameObject platform = Instantiate(platformPrefab, spawnPosition, Quaternion.identity);
                platform.transform.localScale = new Vector3(
                    platform.transform.localScale.x * platformWidthMultiplier,
                    platform.transform.localScale.y,
                    platform.transform.localScale.z);
                regularPlatformsRemaining--;
            }
        }
    }

    public void AddPlatformDensity(float multiplier)
    {
        platformSpacingMultiplier = Mathf.Max(0.25f, platformSpacingMultiplier * multiplier);
    }

    public void ChangePlatformWidth(float multiplier)
    {
        platformWidthMultiplier = Mathf.Max(0.25f, platformWidthMultiplier * multiplier);
        Platform[] platforms = FindObjectsOfType<Platform>();
        foreach (Platform platform in platforms)
            platform.transform.localScale = new Vector3(
                platform.transform.localScale.x * multiplier,
                platform.transform.localScale.y,
                platform.transform.localScale.z);
    }

    public void AddTemporaryDurability(int amount)
    {
        temporaryDurabilityBonus += Mathf.Max(0, amount);
    }

    public void AddLife(int amount)
    {
        lives = Mathf.Max(0, lives + amount);
    }

    public void SetTemporaryRatio(float multiplier)
    {
        temporaryPlatformCount = Mathf.Max(0, Mathf.RoundToInt(temporaryPlatformCount * multiplier));
    }

    public void PlayRickroll()
    {
        if (backgroundMusic != null && rickrollClip != null)
        {
            backgroundMusic.Stop();
            backgroundMusic.clip = rickrollClip;
            backgroundMusic.loop = false;
            backgroundMusic.Play();
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
        if (lives > 0)
        {
            lives--;
            return;
        }

        GameIsOver = true;
        if (loseUi != null)
            loseUi.SetActive(true);
        Time.timeScale = 0f; // Pause the game
        
    }
}

