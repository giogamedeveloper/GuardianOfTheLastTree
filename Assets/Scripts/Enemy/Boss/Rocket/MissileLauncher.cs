using System;
using UnityEngine;

public class MissileLauncher : MonoBehaviour
{
    public GameObject missilePrefab; // Prefab del misil
    public Transform spawnPoint; // Punto desde donde se disparan
    public int numberOfMissiles = 3;
    public float spawnRadius = 1f;
    public float parabolaHeight = 3f;
    bool playerFound = false;


    void Update()
    {
        if (!playerFound)
            Launch();
    }

    public void Launch()
    {
        float detectionRadius = 20f;
        Vector3 origin = spawnPoint.position;

        Collider[] hitColliders = Physics.OverlapSphere(origin, detectionRadius);
        Vector3 playerPos = Vector3.zero;

        foreach (var collider in hitColliders)
        {
            if (collider.CompareTag("Player"))
            {
                playerPos = collider.transform.position;
                playerFound = true;
                break;
            }
        }

        if (!playerFound)
        {
            Debug.LogWarning("No se detectó ningún jugador dentro del radio");
            return;
        }

        for (int i = 0; i < numberOfMissiles; i++)
        {
            Vector3 spawnOffset = new Vector3(
                Mathf.Cos(i * Mathf.PI * 2 / numberOfMissiles),
                0,
                Mathf.Sin(i * Mathf.PI * 2 / numberOfMissiles)
            ) * spawnRadius;

            Vector3 spawnPos = spawnPoint.position + spawnOffset;
            GameObject missileGO = Instantiate(missilePrefab, spawnPos, Quaternion.identity);
            MissileBoss missile = missileGO.GetComponent<MissileBoss>();

            // Usa la posición detectada
            missile.Initialize(spawnPos, playerPos, parabolaHeight);
        }
    }
}
