using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public GameObject voidd;
    public GameObject meteorSpawner;

    [Header("Score UI")]
    public Text skor;

    [Header("Scoring")]
    public float scorePerUnitY = 10f;

    private float lastTargetY;
    private int score;

    private void Start()
    {
        if (target != null)
            lastTargetY = target.position.y;
        UpdateScoreUI();
    }

    private void LateUpdate()
    {
        if (target == null) return;

        float currentY = target.position.y;

        if (currentY > lastTargetY)
        {
            float deltaY = currentY - lastTargetY;
            score += Mathf.FloorToInt(deltaY * scorePerUnitY);
            lastTargetY = currentY;
            UpdateScoreUI();
        }

        if (target.position.y > transform.position.y)
        {
            Vector3 newPosition = new Vector3(transform.position.x, target.position.y, transform.position.z);
            transform.position = newPosition;
        }

        if (voidd != null)
            voidd.transform.position = new Vector3(transform.position.x, transform.position.y - 7.72f, voidd.transform.position.z);
        if (meteorSpawner != null)
            meteorSpawner.transform.position = new Vector3(transform.position.x, transform.position.y + 3.57f, meteorSpawner.transform.position.z);
    }

    private void UpdateScoreUI()
    {
        if (skor != null)
            skor.text = "Score: " + score.ToString();
    }
}
