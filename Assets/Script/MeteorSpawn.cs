using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorSpawn : MonoBehaviour
{
    public GameObject meteorPrefab;
    public GameObject warningPrefab;
    public GameObject LoseUi;

    [Header("Timing")]
    public float warningTime = 1.5f;
    public float spawnInterval = 3f;

    [Header("Offsets dari CameraFollow")]
    public float warningOffsetY = 1f;
    public float meteorOffsetY = 3f;

    [Header("Reference")]
    public Transform cameraFollow; 

    private Camera cam;

    void Start()
    {
        LoseUi.SetActive(false);
        cam = Camera.main;
        if (cameraFollow == null)
            cameraFollow = Camera.main != null ? Camera.main.transform : null;

        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            SpawnMeteorSequence();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnMeteorSequence()
    {
        float halfWidth = cam.orthographicSize * cam.aspect;

        // X random di dalam layar
        float randomX = Random.Range(cam.transform.position.x - halfWidth, cam.transform.position.x + halfWidth);

        StartCoroutine(WarningThenMeteor(randomX));
    }

    IEnumerator WarningThenMeteor(float xPos)
    {
        float z = warningPrefab != null ? warningPrefab.transform.position.z : 0f;
        if (meteorPrefab != null) z = meteorPrefab.transform.position.z;

        float camZ = Mathf.Abs(cam.transform.position.z);

        Vector3 topWorld = cam.ViewportToWorldPoint(new Vector3(0.5f, 1f, camZ));
        float topY = topWorld.y;

        
        Vector3 warningPos = new Vector3(xPos, topY - warningOffsetY, z);
        GameObject warning = Instantiate(warningPrefab, warningPos, Quaternion.identity);

        float t = 0f;
        while (t < warningTime)
        {
            float loopTopY = cam.ViewportToWorldPoint(new Vector3(0.5f, 1f, camZ)).y;
            warning.transform.position = new Vector3(xPos, loopTopY - warningOffsetY, warning.transform.position.z);

            t += Time.deltaTime;
            yield return null;
        }

        Destroy(warning);

        // Meteor muncul dari atas (TOP viewport) saat warning selesai
        float meteorY = cam.ViewportToWorldPoint(new Vector3(0.5f, 1f, camZ)).y;
        Vector3 meteorPos = new Vector3(xPos, meteorY + meteorOffsetY, z);
        Instantiate(meteorPrefab, meteorPos, Quaternion.identity);


    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Handle collision with player
            Destroy(gameObject);
            LoseUi.SetActive(true);
            Time.timeScale = 0f; // Pause the game
        }
    }
}

