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
    private PlayeerController player;
    private float platformSpacingMultiplier = 1f;
    private float platformWidthMultiplier = 1f;
    private int temporaryDurabilityBonus;
    void Start()
    {
        player = FindObjectOfType<PlayeerController>();

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

        Vector3 spawnPosition = player != null
            ? new Vector3(player.transform.position.x, player.transform.position.y - 1f, 0f)
            : new Vector3();

        List<Vector3> placedPlatformPositions = new List<Vector3>();
        bool nextTemporaryOnRight = true;

        int regularPlatformsRemaining = platformCount;
        int temporaryPlatformsRemaining = temporaryPlatformCount;
        int totalPlatformCount = platformCount + temporaryPlatformCount;

        for (int i = 0; i < totalPlatformCount; i++)
        {
            spawnPosition.y += Random.Range(1.6f, 2.3f) * platformSpacingMultiplier;
            float baseX = player != null ? player.transform.position.x : 0f;

            bool spawnTemporary = temporaryPlatformsRemaining > 0 &&
                (regularPlatformsRemaining == 0 ||
                Random.Range(0, regularPlatformsRemaining + temporaryPlatformsRemaining) < temporaryPlatformsRemaining);

            float horizontalOffset;
            if (spawnTemporary)
            {
                float preferredSide = nextTemporaryOnRight ? 3.0f : -3.0f;
                horizontalOffset = preferredSide + Random.Range(-0.7f, 0.7f);
                nextTemporaryOnRight = !nextTemporaryOnRight;
            }
            else
            {
                horizontalOffset = Random.Range(-2.5f, 2.5f);
            }

            Vector3 candidatePosition = new Vector3(
                Mathf.Clamp(baseX + horizontalOffset, -5.5f, 5.5f),
                spawnPosition.y,
                0f);

            int attempt = 0;
            while (attempt < 25)
            {
                bool tooCloseToExisting = false;
                foreach (Vector3 existing in placedPlatformPositions)
                {
                    float distanceX = Mathf.Abs(existing.x - candidatePosition.x);
                    float distanceY = Mathf.Abs(existing.y - candidatePosition.y);
                    if (distanceX < (spawnTemporary ? 1.6f : 1.2f) && distanceY < 0.9f)
                    {
                        tooCloseToExisting = true;
                        if (spawnTemporary)
                        {
                            float preferredSide = nextTemporaryOnRight ? 3.0f : -3.0f;
                            candidatePosition.x = Mathf.Clamp(baseX + preferredSide + Random.Range(-0.8f, 0.8f), -5.5f, 5.5f);
                        }
                        else
                        {
                            candidatePosition.x = Mathf.Clamp(baseX + Random.Range(-2.5f, 2.5f), -5.5f, 5.5f);
                        }
                        candidatePosition.y += 0.15f;
                        break;
                    }
                }

                if (!tooCloseToExisting)
                    break;

                attempt++;
            }

            if (spawnTemporary)
            {
                GameObject platform = Instantiate(temporaryPlatformPrefab, candidatePosition, Quaternion.identity);
                TemporaryPlatform temporaryPlatform = platform.GetComponent<TemporaryPlatform>();
                if (temporaryPlatform != null)
                    temporaryPlatform.SetDurabilityBonus(temporaryDurabilityBonus);
                placedPlatformPositions.Add(candidatePosition);
                temporaryPlatformsRemaining--;
            }
            else
            {
                GameObject platform = Instantiate(platformPrefab, candidatePosition, Quaternion.identity);
                platform.transform.localScale = new Vector3(
                    platform.transform.localScale.x * platformWidthMultiplier,
                    platform.transform.localScale.y,
                    platform.transform.localScale.z);
                placedPlatformPositions.Add(candidatePosition);
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

