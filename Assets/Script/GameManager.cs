using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static bool GameIsOver;
    [Header("References")]
    public GameObject platformPrefab;
    public GameObject loseUi;
    [Header("Jumlah Platform")]
    public int platformCount = 1000;
    
    void Start()
    {
        loseUi.SetActive(false);
        Vector3 spawnPosition = new Vector3();

        for (int i = 0; i < platformCount; i++)
        {
            spawnPosition.y += Random.Range(2f, 2.5f);
            spawnPosition.x = Random.Range(-5f, 5f);
            Instantiate(platformPrefab, spawnPosition, Quaternion.identity);
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

