using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MeinMenu : MonoBehaviour
{

    private System.Collections.IEnumerator PlayGame()
    {
        
        Debug.Log("Button clicked! Waiting 1 seconds...");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("GamePlay"); 
        GameManager.GameIsOver = false;
    }

    private System.Collections.IEnumerator Exit()
    {
        Debug.Log("Button clicked! Waiting 1 seconds...");
        yield return new WaitForSeconds(1f);
        Application.Quit();
    }

    public void PlayGameButton()
    {
        StartCoroutine(PlayGame());
    }
    public void ExitButton()
    {
        StartCoroutine(Exit());
    }
}
